using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using VMUnprotect.Core.Abstraction;

namespace VMUnprotect.Core.Hooks.Methods {
    [HarmonyPatch(typeof(Debugger), nameof(Debugger.IsLogging))]
    public class DebugIsLoggingPatch {
        private static ILogger CtxLogger => Engine.Ctx.Logger;

        /// <summary>
        ///     Transpiler overwrites body so we can't just return
        /// </summary>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions) {
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret)
            };

            //MessageBox.Show("IsAttached returned false", "IsAttached");
            return codes.AsEnumerable();
        }
    }
}