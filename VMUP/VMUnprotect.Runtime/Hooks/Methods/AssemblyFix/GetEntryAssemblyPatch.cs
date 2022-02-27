using HarmonyLib;
using System.Reflection;
using VMUnprotect.Runtime.General;

// ReSharper disable InconsistentNaming

namespace VMUnprotect.Runtime.Hooks.Methods.AssemblyFix
{
    public class GetEntryAssemblyPatch : VmUnprotectPatch
    {
        private static readonly MethodInfo TargetMethod =
            AccessTools.Method(typeof(Assembly), nameof(Assembly.GetEntryAssembly));

        public GetEntryAssemblyPatch(Context ctx, ILogger logger) : base(ctx, logger) { }

        public static void Postfix(ref Assembly __result) {
            if (__result != typeof(string).Assembly)
                return;

            //MessageBox.Show($"Swapped {__result.FullName} | GetCallingAssembly", "GetCallingAssembly");
            __result = Ctx.Assembly;
        }

        public override void Patch(Harmony instance) {
            PatchPostfix(instance, TargetMethod);
        }

    }
}