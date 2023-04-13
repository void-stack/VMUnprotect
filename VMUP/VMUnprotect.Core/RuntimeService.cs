using dnlib.DotNet;
using VMUnprotect.Runtime;
using ILogger = Serilog.ILogger;

namespace VMUnprotect.Core;

public interface IRuntimeService
{
    public bool GetRuntimeTypeDefinition(string? fullName, out TypeDef runtimeTypeDefinition,
        bool isReflectionName = false);
}

public class RuntimeService : IRuntimeService
{
    private readonly ILogger _logger;
    private readonly ModuleDef? _runtimeModule;

    public RuntimeService(ILogger logger)
    {
        _logger = logger;
        _runtimeModule = ModuleDefMD.Load(typeof(Unprotect).Assembly.Location);
    }

    public bool GetRuntimeTypeDefinition(string? fullName, out TypeDef runtimeTypeDefinition,
        bool isReflectionName = false)
    {
        if (_runtimeModule is null)
            throw new Exception("Runtime module is null!");

        if (fullName is null)
            throw new ArgumentNullException(nameof(fullName));

        runtimeTypeDefinition = _runtimeModule.Find(fullName, isReflectionName);

        bool found = runtimeTypeDefinition is not null;
        _logger.Debug("Trying to find runtime type definition '{Type}': result {Found}", fullName, found);

        return found;
    }
}
