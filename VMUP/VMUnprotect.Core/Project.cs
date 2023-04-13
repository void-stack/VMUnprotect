using Autofac;

namespace VMUnprotect.Core;

public class Project
{
    private readonly IContainer _container;
    private readonly Context _context;

    public Project(Options options)
    {
        _context = new Context(options);
        _container = ContainerConfig.Configure(_context);
    }

    public void Run()
    {
        var runtime = _container.Resolve<IRuntimeInjector>();
        _context.Logger.Information("Running project...");

        if (runtime.InjectToStaticConstructor(_context.Module, "VMUnprotect.Runtime.Unprotect", "Initialize"))
            _context.Logger.Information("Successfully injected runtime!");
        else
            _context.Logger.Error("Failed to inject runtime!");

        _context.WriteToDisk();
        _container.Dispose();

        Console.ReadKey();
    }
}
