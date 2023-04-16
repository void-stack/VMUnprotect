#nullable enable
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VMUnprotect.Runtime;

/// <summary>
///     This class is used to inject the custom runtime into the target assembly.
///     We'll use this class to patch the target assembly, injecting our methods to VMProtect Dispatcher / Call Handle /
///     Other methods.
/// </summary>
public static class Unprotect
{
    public static void VMDispatcher(ref object args, ref int methodOffset)
    {
        var logger = SimpleLogger.GetLogger();

        string callingMethod = Utils.GetCallingMethod("method_61");

        object[]? objectArray = args as object[] ?? null;
        string argsString = Utils.FormatObject(objectArray);

        if (callingMethod == "EDBAECB7.B5AE7F33")
            callingMethod = "VMCall";

        logger.Dispatcher($"[{methodOffset}]{callingMethod}({argsString}))");
    }

    public static void Initialize()
    {
        var logger = SimpleLogger.GetLogger();
        logger.VMPCall("This assembly was patched by https://github.com/void-stack/VMUnprotect!");
    }

    #region Utils

    private static class Utils
    {
        public static string FormatObject(object? obj, int depth = 0)
        {
            if (obj == null)
                return "null";

            var type = obj.GetType();

            if (type == typeof(string))
                return $"@\"{obj}\"";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.UInt64:
                    return $"{obj}";
                case TypeCode.Object:
                    if (type == typeof(UIntPtr))
                        return $"0x{((UIntPtr)obj).ToUInt64():X}";
                    if (type == typeof(IntPtr))
                        return $"0x{((IntPtr)obj).ToInt64():X}";

                    if (obj is IEnumerable enumerable)
                    {
                        string typeName = type.Name;

                        switch (obj)
                        {
                            case IDictionary dict:
                            {
                                var pairs = dict.Keys.Cast<object>().Zip(dict.Values.Cast<object>(),
                                    (k, v) => $"{FormatObject(k, depth + 1)}: {FormatObject(v, depth + 1)}");
                                return $"new {typeName} {{ {string.Join(", ", pairs)} }}";
                            }
                            case IList list:
                            {
                                var items = list.Cast<object>().Select(e => FormatObject(e, depth + 1));
                                return $"new {typeName} {{ {string.Join(", ", items)} }}";
                            }
                            default:
                            {
                                var elements = enumerable.Cast<object>().Select(e => FormatObject(e, depth + 1));
                                return $"new {typeName} {{ {string.Join(", ", elements)} }}";
                            }
                        }
                    }

                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var fieldValues = fields.Select(f => $"{f.Name} = {FormatObject(f.GetValue(obj), depth + 1)}");
                    return $"{type.Name} => {{ {string.Join(", ", fieldValues)} }}";
                default:
                    return "???";
            }
        }


        /// https://stackoverflow.com/questions/171970/how-can-i-find-the-method-that-called-the-current-method
        /// <summary>
        ///     Returns the call that occurred just before the the method specified.
        /// </summary>
        /// <param name="methodAfter">The named method to see what happened just before it was called. (case sensitive)</param>
        /// <returns>The method name.</returns>
        public static string GetCallingMethod(string methodAfter)
        {
            string str = "";

            try
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? Array.Empty<StackFrame>();

                for (int i = 0; i < st.FrameCount - 1; i++)
                {
                    if (!frames[i].GetMethod().Name.Equals(methodAfter))
                        continue;

                    if (frames[i + 1].GetMethod().Name.Equals(methodAfter)) // ignores overloaded methods.
                        continue;

                    str = frames[i + 1].GetMethod().ReflectedType.FullName + "." + frames[i + 1].GetMethod().Name;
                    break;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return str;
        }
    }

    #endregion

    #region Logger

    /// <summary>
    ///     We're going to use this class to log all traces to a file.
    /// </summary>
    public class SimpleLogger
    {
        private const string FileExt = ".log";

        // Make singleton
        private static SimpleLogger? _instance;

        private readonly string _datetimeFormat;
        private readonly object _fileLock = new();
        private readonly string _logFilename;

        /// <summary>
        ///     Initiate an instance of SimpleLogger class constructor.
        ///     If log file does not exist, it will be created automatically.
        /// </summary>
        private SimpleLogger()
        {
            _datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            _logFilename = Assembly.GetExecutingAssembly().GetName().Name + FileExt;

            // Log file header line
            string logHeader = _logFilename + " is created.";
            if (!File.Exists(_logFilename))
                WriteLine(DateTime.Now.ToString(_datetimeFormat) + " " + logHeader);
        }

        public static SimpleLogger GetLogger()
        {
            if (_instance is null)
                _instance = new SimpleLogger();

            return _instance;
        }

        public void CallHandler(string text)
        {
            WriteFormattedLog(LogLevel.CallHandler, text);
        }

        public void VMPCall(string text)
        {
            WriteFormattedLog(LogLevel.VMPCall, text);
        }

        public void Dispatcher(string text)
        {
            WriteFormattedLog(LogLevel.Dispatcher, text);
        }

        private void WriteLine(string text, bool append = false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            lock (_fileLock)
            {
                using var writer = new StreamWriter(_logFilename, append, Encoding.UTF8);
                writer.WriteLine(text);
            }
        }

        private void WriteFormattedLog(LogLevel level, string text)
        {
            string pretext = level switch
            {
                LogLevel.Dispatcher => DateTime.Now.ToString(_datetimeFormat) + " [VMDispatcher] ",
                LogLevel.CallHandler => DateTime.Now.ToString(_datetimeFormat) + " [VMCallHandler] ",
                LogLevel.VMPCall => DateTime.Now.ToString(_datetimeFormat) + " [VMCall] ",
                _ => ""
            };

            WriteLine(pretext + text, true);
        }

        [Flags]
        private enum LogLevel
        {
            Dispatcher,
            CallHandler,
            VMPCall
        }
    }

    #endregion
}
