using Autofac;
using HarmonyLib;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VMUnprotect.Runtime.Hooks;
using VMUnprotect.Runtime.Structure;

namespace VMUnprotect.Runtime.General
{
    internal static class Engine
    {
        private static IContainer Container { get; set; }

        internal static void Initialize(Project project, ILogger logger, CommandLineOptions options) {
            logger.Info("initializing Engine...");
            Container = ContainerConfig.Configure(logger, project, options);
        }

        internal static void Run(Project project) {
            var logger = Container.Resolve<ILogger>();
            var ctx = Container.Resolve<Context>();
            var hooks = Container.Resolve<IHooksManager>();

            logger.Info("Starting...");
            var stopwatch = Stopwatch.StartNew();

            logger.Debug("Loading sample...");
            Container.Resolve<IAssemblyLoader>().LoadAssembly();

            logger.Debug("Discovering VMProtect Runtime...");
            Container.Resolve<IVmRuntimeAnalyzer>().Discover();

            using var scope = Container.BeginLifetimeScope();
            ctx.Scope = scope;

            logger.Debug("Applying VMProtect hooks...");
            hooks.Initialize();
            hooks.ApplyHooks();


            logger.Debug("Invoking Target...");
            InvokeTarget(ctx, logger);

            stopwatch.Stop();
            logger.Info("Finished all tasks in {0}", stopwatch.Elapsed);
            logger.Info("Restoring Hooks...");
            hooks.RestoreAll();
        }

        private static void InvokeTarget(Context ctx, ILogger logger) {

            var fileEntryPoint = ctx.Assembly.EntryPoint;
            var moduleHandle = ctx.Assembly.ManifestModule.ModuleHandle;
            var parameters = fileEntryPoint.GetParameters();

            logger.Debug("Entrypoint method: {0}", fileEntryPoint.FullDescription());
            logger.Debug("ModuleHandle: {0:X4}", moduleHandle);

            logger.Info("--- Invoking Constructor.");
            RuntimeHelpers.RunModuleConstructor(moduleHandle);

            logger.Info("--- Invoking assembly.\n");
            fileEntryPoint.Invoke(
                null,
                parameters.Length == 0 ? null : new object[] {new[] {string.Empty}}); // parse arguments from commandlineoptions
        }
    }
}