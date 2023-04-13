using System;

namespace VMUnprotect.Runtime
{
    /// <summary>
    ///     We're going to use this class to log all traces to a file.
    /// </summary>
    public static class FileLogger
    {

    }

    /// <summary>
    ///     This class is used to inject the custom runtime into the target assembly.
    ///     We'll use this class to patch the target assembly, injecting our methods to VMProtect Dispatcher / Call Handle /
    ///     Other methods.
    /// </summary>
    public static class Unprotect
    {
        public static void Initialize()
        {
            Console.WriteLine("This assembly was patched by https://github.com/void-stack/VMUnprotect!");
        }
    }
}
