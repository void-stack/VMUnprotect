using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using VMUnprotect.Core.Abstraction;
using VMUnprotect.Core.Helpers;

namespace VMUnprotect.Core.MiddleMan {
    /// <summary>
    ///     Works as Middle Man to make life easier
    /// </summary>
    public static class UnsafeInvokeMiddleMan {
        private static readonly ILogger ConsoleLogger = Engine.Logger;

        /// <summary>
        ///     A prefix is a method that is executed before the original method
        /// </summary>
        public static bool Prefix(ref object __result, ref object __instance, ref object obj, ref object[] parameters, ref object[] arguments) {
            var virtualizedMethodName = new StackTrace().GetFrame(7).GetMethod();
            var method = (MethodBase) __instance;

            ConsoleLogger.Print("VMP MethodName: {0} (MDToken 0x{1:X4})", virtualizedMethodName.FullDescription(), virtualizedMethodName.MetadataToken.ToString());
            ConsoleLogger.Print("MethodName: {0}", method.Name);
            ConsoleLogger.Print("FullDescription: {0}", method.FullDescription());
            ConsoleLogger.Print("MethodType: {0}", method.GetType());

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (obj is not null)
                ConsoleLogger.Print("Obj: {0}", Formatter.FormatObject(obj));

            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                ConsoleLogger.Print("Parameter ({1}) [{0}]: ({2})", i, parameter.GetType(), Formatter.FormatObject(parameter));
            }

            var returnType = method is MethodInfo info ? info.ReturnType.FullName : "System.Object";
            ConsoleLogger.Print("MDToken: 0x{0:X4}", method.MetadataToken);
            ConsoleLogger.Print("Return Type: {0}", returnType ?? "null");
            return true;
        }

        /// <summary>
        ///     A postfix is a method that is executed after the original method
        /// </summary>
        public static void Postfix(ref object __instance, ref object __result, ref object obj, ref object[] parameters, ref object[] arguments) {
            ConsoleLogger.Print("Returns: {0}", __result);
        }
    }
}