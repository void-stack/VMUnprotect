using HarmonyLib;
using System.Globalization;
using System.Reflection;
using VMUnprotect.Utils;

namespace VMUnprotect
{
    /// <summary>
    ///     Works as Middle Man to make life easier
    /// </summary>
    internal static class MiddleMan
    {
        /// <summary>
        ///     This function manipulate can manipulate, log actual invokes from virtualized VMP functions.
        /// </summary>
        public static void VmpMethodLogger(
            object obj,
            BindingFlags? bindingFlags,
            Binder binder,
            ref object[] parameters,
            CultureInfo culture,
            MethodBase methodBase,
            ref object returnValue)
        {
            ConsoleLogger.Warn("MethodName: {0}", methodBase.Name);
            ConsoleLogger.Warn("FullDescription: {0}", methodBase.FullDescription());
            ConsoleLogger.Warn("MethodType: {0}", methodBase.GetType());
            if (obj != null) ConsoleLogger.Warn("obj: {0}", obj.GetType());


            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                ConsoleLogger.Warn("Parameter ({1}) [{0}]: ({2})", i, parameter.GetType(), parameter);
            }

            ConsoleLogger.Warn("MDToken: {0}", methodBase.MetadataToken);
            ConsoleLogger.Warn("Returns: {0}", returnValue);

            if (returnValue != null)
                ConsoleLogger.Warn("Return type: {0}\n", returnValue.GetType());
        }
    }
}