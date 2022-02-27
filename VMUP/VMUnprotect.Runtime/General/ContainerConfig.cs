using Autofac;
using VMUnprotect.Runtime.Hooks;
using VMUnprotect.Runtime.Hooks.Methods.AntiDebug;
using VMUnprotect.Runtime.MiddleMan;
using VMUnprotect.Runtime.Modules;
using VMUnprotect.Runtime.Structure;

namespace VMUnprotect.Runtime.General
{
    internal static class ContainerConfig
    {
        private static ContainerBuilder Builder { get; set; }

        public static IContainer Configure(ILogger logger, Project project, CommandLineOptions options) {
            Builder = new ContainerBuilder();

            Builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
            Builder.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().SingleInstance();
            Builder.RegisterType<VmRuntimeAnalyzer>().As<IVmRuntimeAnalyzer>().SingleInstance();
            Builder.RegisterType<HooksManager>().As<IHooksManager>().SingleInstance();

            Builder.RegisterType<UnsafeInvokeMiddleMan>().As<IUnsafeInvokeMiddleMan>().SingleInstance();
            Builder.RegisterType<NtQueryInformationProcessPatch>().As<INtQueryInformationProcessPatch>().SingleInstance();

            Builder.RegisterModule(new VmProtectPatchesModule());

            logger.Debug("Creating context...");
            Builder.RegisterInstance(new Context(project, logger, options)).As<Context>().SingleInstance();

            return Builder.Build();
        }
    }
}