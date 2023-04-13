using dnlib.DotNet;
using VMUnprotect.Runtime;

namespace VMUnprotect.Core;

public interface IRuntimeService
{
    public bool GetRuntimeTypeDefinition(string? fullName, out TypeDef runtimeTypeDefinition,
        bool isReflectionName = false);
}

public class RuntimeService : IRuntimeService
{
    private readonly Context _context;
    private readonly ModuleDef? _runtimeModule;

    public RuntimeService(Context context)
    {
        _runtimeModule = ModuleDefMD.Load(typeof(Unprotect).Assembly.Location);
        _context = context;
    }

    public bool GetRuntimeTypeDefinition(string? fullName, out TypeDef runtimeTypeDefinition,
        bool isReflectionName = false)
    {
        if (_runtimeModule is null)
            throw new Exception("Runtime module is null!");

        if (fullName is null)
            throw new ArgumentNullException(nameof(fullName));

        runtimeTypeDefinition = _runtimeModule.Find(fullName, isReflectionName);
        return runtimeTypeDefinition is not null;
    }
}
