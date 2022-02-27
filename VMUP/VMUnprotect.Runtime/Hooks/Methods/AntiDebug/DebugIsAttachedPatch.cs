using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VMUnprotect.Runtime.General;

namespace VMUnprotect.Runtime.Hooks.Methods.AntiDebug
{
    public class DebugIsAttachedPatch : VmUnprotectPatch
    {
        private static readonly MethodInfo TargetMethod = AccessTools.Method(typeof(Debugger), "get_IsAttached");

        public DebugIsAttachedPatch(Context ctx, ILogger logger) : base(ctx, logger) { }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions) {
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret)
            };

            Logger.Debug($"Recompiled {nameof(TargetMethod)} body to IL and forced to return: false");
            return codes.AsEnumerable();
        }

        public override void Patch(Harmony instance) {
            if (Ctx.Options.BypassAntiDebug)
                PatchTranspiler(instance, TargetMethod);
        }
    }
}