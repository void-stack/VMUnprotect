using System.Diagnostics;
using VMUnprotect.Core.Abstraction;
using VMUnprotect.Core.MiddleMan;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     This class contains Harmony Patches (Thanks https://github.com/Washi1337)
    ///     Use Methods/MiddleMan.cs
    /// </summary>
    public static class VmProtectDumperUnsafeInvoke {
        private static readonly ILogger CtxLogger = Engine.Logger;

        /// <summary>
        ///     Prefix of UnsafeInvoke, this runs before.
        /// </summary>
        public static void InvokePrefix(ref object __instance, ref object obj, ref object[] parameters, ref object[] arguments) {
            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = Engine.Ctx.RuntimeStructure?.FunctionHandler != null
                                && Engine.Ctx.RuntimeStructure != null
                                && new StackTrace().GetFrame(3).GetMethod().MetadataToken == Engine.Ctx.RuntimeStructure.FunctionHandler.MDToken.ToInt32();

            if (!isVmpFunction)
                return;

            CtxLogger.Info("VmProtectDumperUnsafeInvoke Prefix:");
            CtxLogger.Warn("{");
            UnsafeInvokeMiddleMan.Prefix(ref __instance, ref obj, ref parameters, ref arguments);
        }

        /// <summary>
        ///     Postfix of UnsafeInvoke, this runs after.
        /// </summary>
        public static void InvokePostfix(ref object __instance, ref object __result, ref object obj, ref object[] parameters, ref object[] arguments) {
            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = Engine.Ctx.RuntimeStructure?.FunctionHandler != null
                                && Engine.Ctx.RuntimeStructure != null
                                && new StackTrace().GetFrame(3).GetMethod().MetadataToken == Engine.Ctx.RuntimeStructure.FunctionHandler.MDToken.ToInt32();

            if (!isVmpFunction)
                return;

            // Log it
            CtxLogger.Info("VmProtectDumperUnsafeInvoke Result:");
            UnsafeInvokeMiddleMan.Postfix(ref __instance, ref __result, ref obj, ref parameters, ref arguments);
            CtxLogger.Warn("}\n");
        }
    }
}