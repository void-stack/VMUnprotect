using HarmonyLib;
using System;
using System.Reflection;
using VMUnprotect.Core.Abstraction;
using VMUnprotect.Core.Hooks.Methods;

namespace VMUnprotect.Core.Hooks {
    /// <summary>
    ///     Manages patches of Harmony
    /// </summary>
    public static class HooksManager {
        private static readonly ILogger CtxLogger = Engine.Logger;
        /// <summary>
        ///     Applies Harmony patches to VMP functions.
        /// </summary>
        /// <param name="ctx">Context</param>
        internal static void HooksApply(Context ctx) {
            // Check if functionHandler was found.
            if (ctx.RuntimeStructure?.FunctionHandler is null)
                throw new ArgumentException("Could not locate Function Handler.");

            // Patch all
            ctx.Harmony.PatchAll();

            // resolve method.
            CtxLogger.Debug("Found VMPFunctionHandler, MDToken 0x{0:X4}", ctx.RuntimeStructure.FunctionHandler.MDToken.ToInt32());

            // Should we use VmProtectDumperTranspiler or VmProtectDumperUnsafeInvoke
            switch (ctx.Options.UseTranspiler) {
                case true: {
                    // Resolve VMP Method Handler
                    var resolvedVmpHandler = ctx.VmpAssembly.ManifestModule.ResolveMethod(ctx.RuntimeStructure.FunctionHandler.MDToken.ToInt32());

                    // Prepare Transpiler
                    var transpiler = typeof(VmProtectDumperTranspiler).GetMethod(nameof(VmProtectDumperTranspiler.Transpiler));
                    CtxLogger.Debug("Applying Transpiler and replacing Invokes.");

                    // Apply transpiler to the function handler (This is old method not recommended)
                    ctx.Harmony.Patch(resolvedVmpHandler, transpiler: new HarmonyMethod(transpiler));
                }
                    break;
                case false: {
                    // Get method for patch
                    var invokeMethod = typeof(object).Assembly.GetType("System.Reflection.RuntimeMethodInfo")
                        .GetMethod("UnsafeInvokeInternal", BindingFlags.NonPublic | BindingFlags.Instance);

                    // Prepare prefix and postfix
                    var prefix = typeof(VmProtectDumperUnsafeInvoke).GetMethod(nameof(VmProtectDumperUnsafeInvoke.InvokePrefix));
                    var postfix = typeof(VmProtectDumperUnsafeInvoke).GetMethod(nameof(VmProtectDumperUnsafeInvoke.InvokePostfix));

                    // Patch
                    ctx.Harmony.Patch(invokeMethod, new HarmonyMethod(prefix));
                    ctx.Harmony.Patch(invokeMethod, postfix: new HarmonyMethod(postfix));
                }
                    break;
            }
        }
    }
}