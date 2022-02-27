using Autofac;
using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Hooks.Methods.AntiDebug;
using VMUnprotect.Runtime.MiddleMan;

// ReSharper disable InconsistentNaming

namespace VMUnprotect.Runtime.Hooks.Methods
{
    public class VmProtectDumperUnsafeInvoke : VmUnprotectPatch
    {
        public VmProtectDumperUnsafeInvoke(Context ctx, ILogger logger) : base(ctx, logger) { }

        private static MethodInfo TargetMethod() {
            var runtimeMethodInfoType = AccessTools.TypeByName("System.Reflection.RuntimeMethodInfo");
            var invokeMethod = AccessTools.DeclaredMethod(runtimeMethodInfoType, "UnsafeInvokeInternal");
            return invokeMethod;
        }

        public static bool Prefix(
            ref object __result,
            ref object __instance,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments) {
            var structure = Ctx.VmRuntimeStructure;

            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = structure is { } && new StackTrace().GetFrame(3).GetMethod().MetadataToken ==
                structure.FunctionHandler.MDToken.ToInt32();

            if (!isVmpFunction)
                return true;

            if (Ctx.Options.BypassAntiDebug)
                Ctx.Scope.Resolve<INtQueryInformationProcessPatch>().OverwriteProcessInformation(obj, ref arguments);


            Logger.Info("VmProtectDumperUnsafeInvoke Prefix:");
            Logger.Warn("{");
            return Ctx.Scope.Resolve<IUnsafeInvokeMiddleMan>()
                      .Prefix(ref __result, ref __instance, ref obj, ref parameters, ref arguments);
        }


        public static void Postfix(
            ref object __instance,
            ref object __result,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments) {
            var structure = Ctx.VmRuntimeStructure;

            // Check if this invoke is coming from VMP Handler
            var isVmpFunction = structure is { } && new StackTrace().GetFrame(3).GetMethod().MetadataToken ==
                structure.FunctionHandler.MDToken.ToInt32();

            if (!isVmpFunction)
                return;

            if (Ctx.Options.BypassAntiDebug)
                Ctx.Scope.Resolve<INtQueryInformationProcessPatch>().GetDelegateForFunctionPointer(__result, arguments);

            Logger.Info("VmProtectDumperUnsafeInvoke Result:");
            Ctx.Scope.Resolve<IUnsafeInvokeMiddleMan>()
               .Postfix(ref __instance, ref __result, ref obj, ref parameters, ref arguments);
            Logger.Warn("}\n");
        }

        public override void Patch(Harmony harmony) {
            PatchPrefix(harmony, TargetMethod());
            PatchPostfix(harmony, TargetMethod());
        }
    }
}