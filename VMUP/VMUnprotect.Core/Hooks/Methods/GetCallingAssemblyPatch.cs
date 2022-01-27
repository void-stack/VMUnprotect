using HarmonyLib;
using System.Reflection;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Harmony Patch for GetCallingAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly), nameof(Assembly.GetCallingAssembly))]
    public class GetCallingAssemblyPatch {
        private static ILogger CtxLogger => Engine.Ctx.Logger;
        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(string).Assembly)
                return;

            //MessageBox.Show($"Swapped {__result.FullName} | GetCallingAssembly", "GetCallingAssembly");
            __result = Engine.Ctx.VmpAssembly;
        }
    }
}