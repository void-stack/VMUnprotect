using System.Diagnostics;
using VMUnprotect.Methods;
using VMUnprotect.Utils;

namespace VMUnprotect.Hooks.Methods
{
    /// <summary>
    ///     This class contains Harmony Patches (Thanks https://github.com/Washi1337)
    ///     Use Methods/MiddleMan.cs
    /// </summary>
    public static class VmProtectDumperUnsafeInvoke
    {

        /// <summary>
        ///     Prefix of UnsafeInvoke, this runs before.
        /// </summary>
        public static void InvokePrefix(ref object __instance, ref object obj, ref object[] parameters, ref object[] arguments)
        {
            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = new StackTrace().GetFrame(3).GetMethod().MetadataToken == HooksManager.FunctionHandlerToken.ToInt32();

            if (!isVmpFunction)
                return;

            ConsoleLogger.Info("VmProtectDumperUnsafeInvoke Prefix:");
            ConsoleLogger.Warn("{");
            MiddleMan.Prefix(ref __instance, ref obj, ref parameters, ref arguments);
        }

        /// <summary>
        ///     Postfix of UnsafeInvoke, this runs after.
        /// </summary>
        public static void InvokePostfix(ref object __instance, ref object __result, ref object obj, ref object[] parameters, ref object[] arguments)
        {
            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = new StackTrace().GetFrame(3).GetMethod().MetadataToken == HooksManager.FunctionHandlerToken.ToInt32();

            if (!isVmpFunction)
                return;

            // Log it
            ConsoleLogger.Info("VmProtectDumperUnsafeInvoke Result:");
            MiddleMan.Postfix(ref __instance, ref __result, ref obj, ref parameters, ref arguments);
            ConsoleLogger.Warn("}\n");
        }
    }
}