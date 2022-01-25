using dnlib.DotNet;
using HarmonyLib;
using System;
using System.Reflection;
using VMUnprotect.Hooks.Methods;
using VMUnprotect.Init;
using VMUnprotect.Utils;

namespace VMUnprotect.Hooks
{
    /// <summary>
    ///     Manages patches of Harmony
    /// </summary>
    public static class HooksManager
    {
        // Harmony instance
        private static Harmony Harmony { get; set; }

        public static MDToken FunctionHandlerToken
        {
            get;
            private set;
        }

        /// <summary>
        ///     Applies Harmony patches to VMP functions.
        /// </summary>
        /// <param name="runtimeStructure">Structure of VMP Runtime</param>
        /// <param name="options">Options</param>
        internal static void HooksApply(VmRuntimeStructure runtimeStructure, CommandLineOptions options)
        {
            // Enable Harmony Debug file.
            Harmony.DEBUG = true;

            Harmony = new Harmony("com.hussaryyn.vmup");
            Harmony.PatchAll();

            // Check if functionHandler was found.
            if (runtimeStructure.FunctionHandler is null)
                throw new ArgumentException("Could not locate Function Handler.");

            // Save VMP Function Handler MDToken for check
            FunctionHandlerToken = runtimeStructure.FunctionHandler.MDToken;

            // resolve method.
            ConsoleLogger.Debug("Found VMPFunctionHandler, MDToken 0x{0:X4}", FunctionHandlerToken.ToInt32());

            if (options.UseTranspiler)
            {
                // Resolve VMP Method Handler
                var resolvedVmpHandler = Loader.VmpAssembly.ManifestModule.ResolveMethod(FunctionHandlerToken.ToInt32());

                // Prepare Transpiler
                var transpiler = typeof(VmProtectDumperTranspiler).GetMethod(nameof(VmProtectDumperTranspiler.Transpiler));
                ConsoleLogger.Debug("Applying Transpiler and replacing Invokes.");

                // Apply transpiler to the function handler (This is old method not recommended)
                Harmony.Patch(resolvedVmpHandler, transpiler: new HarmonyMethod(transpiler));
            }
            else
            {
                // Get method for patch
                var invokeMethod = typeof(object).Assembly.GetType("System.Reflection.RuntimeMethodInfo")
                    .GetMethod("UnsafeInvokeInternal", BindingFlags.NonPublic | BindingFlags.Instance);

                // Prepare prefix and postfix
                var prefix = typeof(VmProtectDumperUnsafeInvoke).GetMethod(nameof(VmProtectDumperUnsafeInvoke.InvokePrefix));
                var postfix = typeof(VmProtectDumperUnsafeInvoke).GetMethod(nameof(VmProtectDumperUnsafeInvoke.InvokePostfix));

                // Patch
                Harmony.Patch(invokeMethod, new HarmonyMethod(prefix));
                Harmony.Patch(invokeMethod, postfix: new HarmonyMethod(postfix));
            }
        }
    }
}