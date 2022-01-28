using Autofac;
using HarmonyLib;
using System.Collections.Generic;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Helpers;

namespace VMUnprotect.Runtime.Hooks
{
    public class HooksManager : Params, IHooksManager
    {
        private readonly Harmony _harmony = new("com.hussaryyn.vmup");
        private bool _isApplied;

        public HooksManager(Context ctx, ILogger logger) : base(ctx, logger) { }

        public void Initialize() {
            if (_isApplied)
                return;

            Logger.Info("Starting");
            Harmony.DEBUG = Ctx.Options.EnableHarmonyLogs;

            _isApplied = true;
        }

        public void ApplyHooks() {
            Logger.Info("Patching...");

            foreach (var patch in Ctx.Scope.Resolve<IEnumerable<IVmupHook>>()) {
                Logger.Debug($"Applying hook {patch.GetType().Name}");
                patch.Patch(_harmony);
            }

            Logger.Info("Completed!");
        }
    }

    public interface IHooksManager
    {
        void Initialize();
        void ApplyHooks();
    }
}