using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Helpers;

namespace VMUnprotect.Runtime.Hooks
{
    // NitroxPatcher
    public abstract class VmUnprotectPatch : Params, IVmupHook
    {
        private readonly List<MethodBase> _activePatches = new();
        
        protected VmUnprotectPatch(Context ctx, ILogger logger) : base(ctx, logger) { }

        public abstract void Patch(Harmony harmony);

        public void Restore(Harmony harmony)
        {
            foreach (var targetMethod in _activePatches)
                harmony.Unpatch(targetMethod, HarmonyPatchType.All, harmony.Id);
        }
        
        private HarmonyMethod GetHarmonyMethod(string methodName) {

            var method = AccessTools.DeclaredMethod(GetType(), methodName);

            if (method is null)
                throw new Exception($"Couldn't find {methodName}");

            return new HarmonyMethod(method);
        }

        protected void PatchTranspiler(Harmony harmony, MethodBase targetMethod, string transpilerMethod = "Transpiler") {
            PatchMultiple(harmony, targetMethod, null, null, transpilerMethod);
        }

        protected void PatchPrefix(Harmony harmony, MethodBase targetMethod, string prefixMethod = "Prefix") {
            PatchMultiple(harmony, targetMethod, prefixMethod);
        }

        protected void PatchPostfix(Harmony harmony, MethodBase targetMethod, string postfixMethod = "Postfix") {
            PatchMultiple(harmony, targetMethod, null, postfixMethod);
        }

        protected void PatchMultiple(
            Harmony harmony,
            MethodBase targetMethod,
            bool prefix = false,
            bool postfix = false,
            bool transpiler = false,
            bool finalizer = false,
            bool iLManipulator = false) {
            var prefixMethod = prefix ? "Prefix" : null;
            var postfixMethod = postfix ? "Postfix" : null;
            var transpilerMethod = transpiler ? "Transpiler" : null;
            var finalizerMethod = finalizer ? "Finalizer" : null;

            PatchMultiple(harmony, targetMethod, prefixMethod, postfixMethod, transpilerMethod, finalizerMethod);
        }

        private void PatchMultiple(
            Harmony harmony,
            MethodBase targetMethod,
            string prefixMethod = null,
            string postfixMethod = null,
            string transpilerMethod = null,
            string finalizerMethod = null) {
            if (targetMethod is null)
                throw new Exception("Target method cannot be null");

            var harmonyPrefixMethod = prefixMethod != null ? GetHarmonyMethod(prefixMethod) : null;
            var harmonyPostfixMethod = postfixMethod != null ? GetHarmonyMethod(postfixMethod) : null;
            var harmonyTranspilerMethod = transpilerMethod != null ? GetHarmonyMethod(transpilerMethod) : null;
            var harmonyFinalizerMethod = finalizerMethod != null ? GetHarmonyMethod(finalizerMethod) : null;

            harmony.Patch(targetMethod, harmonyPrefixMethod, harmonyPostfixMethod, harmonyTranspilerMethod,
                          harmonyFinalizerMethod);
            
            _activePatches.Add(targetMethod);
        }
    }
}