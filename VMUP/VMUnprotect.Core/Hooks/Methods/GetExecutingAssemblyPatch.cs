using HarmonyLib;
using System.Reflection;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Harmony Patch for GetExecutingAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly), nameof(Assembly.GetExecutingAssembly))]
    public class GetExecutingAssemblyPatch {
        private static ILogger CtxLogger => Engine.Ctx.Logger;

        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(string).Assembly)
                return;

            //MessageBox.Show($"Swapped {__result.FullName} | GetExecutingAssembly", "GetExecutingAssembly");
            __result = Engine.Ctx.VmpAssembly;
        }
    }
}