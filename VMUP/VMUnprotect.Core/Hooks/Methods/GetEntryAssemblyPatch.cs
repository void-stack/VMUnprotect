using HarmonyLib;
using System.Reflection;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Harmony Patch for GetEntryAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly), nameof(Assembly.GetEntryAssembly))]
    public class GetEntryAssemblyPatch {
        private static ILogger CtxLogger => Engine.Ctx.Logger;

        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(string).Assembly)
                return;

            //MessageBox.Show($"Swapped {__result.FullName} | GetEntryAssembly", "GetEntryAssembly");
            __result = Engine.Ctx.VmpAssembly;
        }
    }
}