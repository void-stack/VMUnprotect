using HarmonyLib;
using System.Reflection;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Harmony Patch for GetEntryAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly))]
    [HarmonyPatch(nameof(Assembly.GetEntryAssembly))]
    public class GetEntryAssemblyPatch {
        private static readonly ILogger? CtxLogger = Engine.Logger;

        /// <summary>
        ///     Used for VMP Spoofing
        /// </summary>
        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(GetEntryAssemblyPatch).Assembly) return;

            CtxLogger?.Debug("Swapped [{0}] '{1}'", "GetEntryAssembly", __result.FullName);
            __result = Engine.Ctx.VmpAssembly;
        }
    }
}