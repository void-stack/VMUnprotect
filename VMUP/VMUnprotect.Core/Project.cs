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

        if (runtime.Inject())
        {
            _context.Logger.Information("Successfully injected runtime");
            _context.WriteToDisk();
            _container.Dispose();
        }
        else
        {
            _context.Logger.Error("Failed to inject runtime");
        }

        Console.ReadKey();
    }
}
