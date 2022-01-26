using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using VMUnprotect.Core.Abstraction;
using VMUnprotect.Core.Hooks;
using VMUnprotect.Core.Structure;

namespace VMUnprotect.Core {
    public class Engine {
        public Engine(Context context) {
            Ctx = context;
            Logger = context.Logger;
        }
        internal static Context Ctx { get; private set; } = null!;
        public static ILogger Logger { get; private set; } = new EmptyLogger();

        public void Start() {
            var fileEntryPoint = Ctx.VmpAssembly.EntryPoint;
            var moduleHandle = Ctx.VmpAssembly.ManifestModule.ModuleHandle;
            var parameters = fileEntryPoint.GetParameters();

            Logger.Debug("Entrypoint method: {0}", fileEntryPoint.FullDescription());
            Logger.Debug("ModuleHandle: {0:X4}", moduleHandle);

            Logger.Debug("--- Analyzing VMP Structure...");
            AnalyzeStructure();

            Logger.Info("--- Applying hooks.");
            ApplyHooks();

            Logger.Info("--- Invoking Constructor.");
            RuntimeHelpers.RunModuleConstructor(moduleHandle);

            Logger.Info("--- Invoking assembly.\n");
            fileEntryPoint.Invoke(null, parameters.Length == 0 ? null : new object[] {new[] {string.Empty}}); // parse arguments from commandlineoptions
        }
        private static void ApplyHooks() {
            try {
                HooksManager.HooksApply(Ctx);
            }
            catch (Exception ex) {
                Logger.Error("Failed to apply Harmony patches: {0}", ex.StackTrace);
            }
        }

        private void AnalyzeStructure() {
            try {
                VmAnalyzer.Run(Ctx);
            }
            catch (Exception ex) {
                Logger.Error("Failed to analyze VMProtect Structure: {0}", ex.StackTrace);
            }
        }
    }
}