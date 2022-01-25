using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using VMUnprotect.Utils;

namespace VMUnprotect.Methods
{
    /// <summary>
    ///     Works as Middle Man to make life easier
    /// </summary>
    internal static class MiddleMan
    {
        public static void Prefix(ref object __instance, ref object obj, ref object[] parameters, ref object[] arguments)
        {
            var virtualizedMethodName = new StackTrace().GetFrame(7).GetMethod();
            var method = (MethodBase) __instance;

            ConsoleLogger.Warn("\tVMP MethodName: {0} (MDToken {1:X4})", virtualizedMethodName.FullDescription(), virtualizedMethodName.MetadataToken.ToString());
            ConsoleLogger.Warn("\tMethodName: {0}", method.Name);
            ConsoleLogger.Warn("\tFullDescription: {0}", method.FullDescription());
            ConsoleLogger.Warn("\tMethodType: {0}", method.GetType());
            if (obj != null) ConsoleLogger.Warn("\nObj: {0}", obj.GetType());

            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i] ?? "null";
                ConsoleLogger.Warn("\tParameter ({1}) [{0}]: ({2})", i, parameter.GetType(), parameter);
            }

            var returnType = method is MethodInfo info ? info.ReturnType.FullName : "System.Object";
            ConsoleLogger.Warn("\tMDToken: 0x{0:X4}", method.MetadataToken);
            ConsoleLogger.Warn("\tReturn Type: {0}", returnType);
        }

        public static void Postfix(ref object __instance, ref object __result, ref object obj, ref object[] parameters, ref object[] arguments)
        {
            ConsoleLogger.Warn("\tReturns: {0}", __result);
        }
    }
}