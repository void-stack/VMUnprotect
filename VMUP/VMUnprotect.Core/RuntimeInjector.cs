using dnlib.DotNet;
using dnlib.DotNet.Emit;
using VMUnprotect.Runtime;
using ILogger = Serilog.ILogger;

namespace VMUnprotect.Core;

public interface IRuntimeInjector
{
    public bool Inject();
}

public class RuntimeInjector : IRuntimeInjector
{
    private readonly IAnalyzerService _analyzer;
    private readonly Context _context;
    private readonly ILogger _logger;
    private readonly IRuntimeService _runtimeService;

    public RuntimeInjector(Context context, ILogger logger, IRuntimeService runtimeService, IAnalyzerService analyzer)
    {
        _context = context;
        _logger = logger;
        _runtimeService = runtimeService;
        _analyzer = analyzer;
    }

    public bool Inject()
    {
        _logger.Information("Getting Runtime Type Definition...");

        const string runtimeType = "VMUnprotect.Runtime.Unprotect";
        var targetModule = _context.Module;

        if (_runtimeService.GetRuntimeTypeDefinition(runtimeType, out var runtimeTypeDefinition))
        {
            var injected = InjectHelper.Inject(runtimeTypeDefinition, targetModule.GlobalType, targetModule).ToList();

            IMethod? init = injected.SingleOrDefault(m => m.Name == nameof(Unprotect.Initialize)) as MethodDef;
            IMethod? dispatcher = injected.SingleOrDefault(m => m.Name == nameof(Unprotect.VMDispatcher)) as MethodDef;

            if (init is null || dispatcher is null)
            {
                _logger.Error("Failed to get runtime type definition '{Type}'", runtimeType);
                return false;
            }

            _logger.Information("Injected runtime type definition '{Type}'", runtimeType);

            _logger.Debug("Injecting runtime initialization");
            InjectInit(targetModule, init);

            if (_analyzer.AnalyzeVmRuntimeStructure(out var structure))
                InjectDispatcherProxy(structure, dispatcher);
            else
                _logger.Error("Failed to analyze VM runtime structure");

            return true;
        }

        _logger.Debug("Failed to get runtime type definition '{Type}'", runtimeType);
        return false;
    }

    private void InjectInit(ModuleDef targetModule, IMethod? init)
    {
        var cctor = targetModule.GlobalType.FindOrCreateStaticConstructor();

        _logger.Debug("Injecting call to runtime initialization");
        cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
    }

    private void InjectDispatcherProxy(VmRuntimeStructure structure, IMethod? dispatcher)
    {
        var vmDispatcher = structure.VMDispatcher.ResolveMethodDefThrow();

        // make sure analyzer checks the parameters amount
        _logger.Debug("Injecting call to VMDispatcher proxy");
        vmDispatcher.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, dispatcher));
        vmDispatcher.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldarga_S, vmDispatcher.Parameters[2]));
        vmDispatcher.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldarga_S, vmDispatcher.Parameters[1]));
    }
}
