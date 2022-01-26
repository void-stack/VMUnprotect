using HarmonyLib;
using System.Reflection;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Harmony Patch for GetCallingAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly))]
    [HarmonyPatch(nameof(Assembly.GetCallingAssembly))]
    public class GetCallingAssemblyPatch {
        private static readonly ILogger? CtxLogger = Engine.Logger;

        /// <summary>
        ///     Used for VMP Spoofing
        /// </summary>
        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(GetCallingAssemblyPatch).Assembly) return;

            CtxLogger?.Debug("Swapped [{0}] '{1}'", "GetCallingAssembly", __result.FullName);
            __result = Engine.Ctx.VmpAssembly;
        }
    }
}