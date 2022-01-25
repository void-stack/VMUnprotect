using HarmonyLib;
using System.Reflection;
using VMUnprotect.Init;
using VMUnprotect.Utils;

namespace VMUnprotect.Hooks.Methods
{
    /// <summary>
    ///     Harmony Patch for GetEntryAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly))]
    [HarmonyPatch(nameof(Assembly.GetEntryAssembly))]
    public class GetEntryAssemblyPatch
    {
        public static void Postfix(ref Assembly __result)
        {
            if (__result != typeof(GetEntryAssemblyPatch).Assembly) return;

            ConsoleLogger.Debug("Swapped [{0}] '{1}'", "GetEntryAssembly", __result.FullName);
            __result = Loader.VmpAssembly;
        }
    }
}