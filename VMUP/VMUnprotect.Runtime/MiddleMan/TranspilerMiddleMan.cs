/*using HarmonyLib;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Helpers;

namespace VMUnprotect.Runtime.MiddleMan
{
    public class TranspilerMiddleMan : Params, ITranspilerMiddleMan
    {
        public TranspilerMiddleMan(Context ctx, ILogger logger) : base(ctx, logger) { }

        /// <summary>
        ///     This function manipulate can manipulate, log actual invokes from virtualized VMP functions.
        /// </summary>
        public object Log(
            object obj,
            BindingFlags? bindingFlags,
            Binder binder,
            ref object[] parameters,
            CultureInfo culture,
            MethodBase methodBase) {
            // THIS IS OLD METHOD!!! Check Methods/MiddleMan.cs
            // Invoke the method and get return value.
            var returnValue = methodBase.Invoke(obj, parameters);

            // TODO: Add option to disable this because can cause bugs and can be broken easily
            var trace = new StackTrace();
            var frame = trace.GetFrame(5); // <--
            var method = frame.GetMethod();

            Logger.Warn($"VMP Method: {method.FullDescription()}");

            Logger.Warn("MethodName: {0}", methodBase.Name);
            Logger.Warn("FullDescription: {0}", methodBase.FullDescription());
            Logger.Warn("MethodType: {0}", methodBase.GetType());

            if (obj is not null)
                Logger.Warn("obj: {0}", obj.GetType());

            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                Logger.Warn("Parameter ({1}) [{0}]: ({2})", i, parameter.GetType(), parameter);
            }

            Logger.Warn("MDToken: {0}", methodBase.MetadataToken);
            Logger.Warn("Returns: {0}", returnValue ?? "null");

            return returnValue ?? null;
        }
    }

    public interface ITranspilerMiddleMan
    {
        public object Log(
            object obj,
            BindingFlags? bindingFlags,
            Binder binder,
            ref object[] parameters,
            CultureInfo culture,
            MethodBase methodBase);
    }
}*/
