using HarmonyLib;
using System.Reflection;
using VMUnprotect.Utils;

namespace VMUnprotect.Hooks.Methods
{
    /// <summary>
    ///     Harmony Patch for GetCallingAssembly
    /// </summary>
    [HarmonyPatch(typeof(Assembly))]
    [HarmonyPatch("GetCallingAssembly")]
    public class GetCallingAssemblyPatch
    {
        public static void Postfix(ref Assembly __result)
        {
            if (__result != typeof(GetCallingAssemblyPatch).Assembly) return;

            ConsoleLogger.Debug("Swapped [{0}] '{1}'", "GetCallingAssembly", __result.FullName);
            __result = Loader.VmpAssembly;
        }
    }
}