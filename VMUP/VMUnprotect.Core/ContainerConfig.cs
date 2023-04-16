using Autofac;
using Serilog;

namespace VMUnprotect.Core;

internal static class ContainerConfig
{
    private static readonly ContainerBuilder Builder = new();

    public static IContainer Configure(Context context)
    {
        Builder.RegisterInstance(context.Options.Logger).As<ILogger>().SingleInstance();
        Builder.RegisterInstance(context).As<Context>().SingleInstance();

        Builder.RegisterType<AnalyzerService>().As<IAnalyzerService>().SingleInstance();
        Builder.RegisterType<RuntimeInjector>().As<IRuntimeInjector>().SingleInstance();
        Builder.RegisterType<RuntimeService>().As<IRuntimeService>().SingleInstance();

        return Builder.Build();
    }
}
