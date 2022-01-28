/*using Autofac;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.MiddleMan;

namespace VMUnprotect.Runtime.Hooks.Methods
{
    public class VmProtectDumperTranspiler : VmUnprotectPatch
    {
        public VmProtectDumperTranspiler(Context ctx, ILogger logger) : base(ctx, logger) { }

        public static object HookedInvoke(
            object obj,
            BindingFlags bindingFlags,
            Binder binder,
            object[] parameters,
            CultureInfo culture,
            MethodBase methodBase) {
            try {
                // Indicate this method was called by newer version of VMP.
                Logger.Warn(
                    "============================================= HookedInvoke =============================================\n");

                // Route the arguments and return value to our middleman function where they can be manipulated or logged.
                return Ctx.Scope.Resolve<ITranspilerMiddleMan>().Log(obj, null, null, ref parameters, null, methodBase);
            } catch (Exception ex) {
                // Log the exception.
                Logger.Error(ex.ToString());
                return null;
            }
        }

        public static object HookedInvokeOld(object obj, object[] parameters, MethodBase methodBase) {
            try {
                // Indicate this method was called by older version of VMP.
                Logger.Warn(
                    "============================================= HookedInvokeOld =============================================\n");

                // Route the arguments and return value to our middleman function where they can be manipulated or logged.
                return Ctx.Scope.Resolve<ITranspilerMiddleMan>().Log(obj, null, null, ref parameters, null, methodBase);
            } catch (Exception ex) {
                // Log the exception.
                Logger.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>A transpiler that replaces all occurrences of a given method with another with additional Ldarg_1 instruction</summary>
        /// <param name="instructions">The enumeration of <see cref="T:HarmonyLib.CodeInstruction" /> to act on</param>
        /// <param name="from">Method to search for</param>
        /// <param name="to">Method to replace with</param>
        /// <returns>Modified enumeration of <see cref="T:HarmonyLib.CodeInstruction" /></returns>
        private static void ReplaceVmpInvoke(ref IEnumerable<CodeInstruction> instructions, MethodBase @from, MethodBase to) {
            if ((object) from == null) throw new ArgumentException("Unexpected null argument", nameof(from));
            if ((object) to == null) throw new ArgumentException("Unexpected null argument", nameof(to));

            var code = new List<CodeInstruction>(instructions);

            for (var x = 0; x < code.Count; x++) {
                var ins = code[x];
                if (ins.operand as MethodBase != from) continue;

                // replace callvirt Invoke with our debug invoke.
                ins.opcode = OpCodes.Callvirt;
                ins.operand = to;

                // insert additional Ldarg_1 which corresponds to MethodBase of invoked function.
                // TODO: Improve this, can be easily broken by obfuscation or future VMP updates
                code.Insert(x, new CodeInstruction(OpCodes.Ldarg_1));
                Logger.Info("Replaced with custom Invoke and injected MethodBase argument at {0}.", x);
            }

        }

        /// <summary>A transpiler that alters instructions that calls specific method</summary>
        /// <param name="instructions">The enumeration of <see cref="T:HarmonyLib.CodeInstruction" /> to act on</param>
        /// <returns>Modified enumeration of <see cref="T:HarmonyLib.CodeInstruction" /></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            Logger.Debug("VMP Function Handler Transpiler");

            // Newer version
            ReplaceVmpInvoke(ref instructions, AccessTools.Method(typeof(MethodBase), "Invoke", new[] {
                typeof(object), typeof(BindingFlags), typeof(Binder), typeof(object[]),
                typeof(CultureInfo)
            }), AccessTools.Method(typeof(VmProtectDumperTranspiler), nameof(HookedInvoke)));

            // Older version
            /*ReplaceVmpInvoke(ref instructions,
                             AccessTools.Method(typeof(MethodBase), "Invoke", new[] {typeof(object), typeof(object[])}),
                             AccessTools.Method(typeof(VmProtectDumperTranspiler), nameof(HookedInvokeOld)));#1#

            // Replace all occurrences of MethodBase.Invoke with our debug version.
            return instructions;
        }

        public override void Patch(Harmony harmony) {
            if (!Ctx.Options.UseTranspiler)
                return;

            var resolvedMethod =
                Ctx.Assembly.ManifestModule.ResolveMethod(Ctx.VmRuntimeStructure.FunctionHandler.MDToken.ToInt32());

            if (resolvedMethod is not null)
                PatchTranspiler(harmony, resolvedMethod);
            else
                Logger.Error("Failed to resolve FunctionHandler!");
        }

        public override void Restore(Harmony harmony) {
            throw new NotImplementedException();
        }
    }
}*/
