using HarmonyLib;
using System;
using System.Reflection;
using VMUnprotect.Hooks.Methods;
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

        /// <summary>
        ///     Applies Harmony patches to VMP functions.
        /// </summary>
        /// <param name="runtimeStructure">Structure of VMP Runtime</param>
        internal static void HooksApply(VmRuntimeStructure runtimeStructure)
        {
            // Enable Harmony Debug file.
            Harmony.DEBUG = true;

            Harmony = new Harmony("com.hussaryyn.vmup");
            Harmony.PatchAll(typeof(Loader).Assembly);

            // Check if functionHandler was found.
            if (runtimeStructure.FunctionHandler is null)
                throw new ArgumentException("Could not locate Function Handler.");

            // resolve method.
            var mdToken = runtimeStructure.FunctionHandler.MDToken.ToInt32();
            var resolvedVmpHandler = Loader.VmpAssembly.ManifestModule.ResolveMethod(mdToken);

            ConsoleLogger.Debug("Found VMPFunctionHandler, MDToken {0}", mdToken);
            ConsoleLogger.Debug("Applying Transpiler and replacing Invokes.");

            // Apply transpiler to the function handler.
            Harmony.Patch(resolvedVmpHandler, transpiler: new HarmonyMethod(typeof(VmProtectDumper).GetMethod(nameof(VmProtectDumper.Transpiler), (BindingFlags) (-1))));
        }
    }
}