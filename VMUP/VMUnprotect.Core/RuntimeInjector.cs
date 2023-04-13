using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILogger = Serilog.ILogger;

namespace VMUnprotect.Core;

public interface IRuntimeInjector
{
    bool InjectToStaticConstructor(ModuleDefMD target, string typeDefinition, string methodName);
    bool InjectToMethod(ModuleDefMD target, string typeDefinition, string methodName);
}

public class RuntimeInjector : IRuntimeInjector
{
    private readonly ILogger _logger;
    private readonly IRuntimeService _runtimeService;

    public RuntimeInjector(ILogger logger, IRuntimeService runtimeService)
    {
        _logger = logger;
        _runtimeService = runtimeService;
    }

    public bool InjectToStaticConstructor(ModuleDefMD target, string typeDefinition, string methodName)
    {
        if (!InjectTypeDefinition(target, typeDefinition, out var members))
            return false;

        if (members.SingleOrDefault(method => method.Name == methodName) is not MethodDef targetMethod)
        {
            _logger.Debug("Failed to get target method '{Method}'", methodName);
            return false;
        }

        var cctor = target.GlobalType.FindOrCreateStaticConstructor();
        cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, targetMethod));
        _logger.Debug("Placed call to '{Method}' in static constructor!", methodName);
        return true;
    }

    public bool InjectToMethod(ModuleDefMD target, string typeDefinition, string methodName)
    {
        _logger.Information("Injecting to static constructor '{Type}'", typeDefinition);

        if (!_runtimeService.GetRuntimeTypeDefinition(typeDefinition, out var runtimeTypeDefinition))
        {
            _logger.Debug("Failed to get runtime type definition '{Type}'", typeDefinition);
            return false;
        }

        var members = InjectHelper.Inject(runtimeTypeDefinition, target.GlobalType, target);
        var cctor = target.GlobalType.FindOrCreateStaticConstructor();

        if (members.SingleOrDefault(method => method.Name == methodName) is not MethodDef targetMethod)
        {
            _logger.Debug("Failed to get target method '{Method}'", methodName);
            return false;
        }

        cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, targetMethod));
        _logger.Debug("Placed call to '{Method}' in static constructor!", methodName);
        return true;
    }

    private bool InjectTypeDefinition(ModuleDef target, string typeDefinition, out List<IDnlibDef> members)
    {
        _logger.Information("Injecting type '{Type}'", typeDefinition);

        if (!_runtimeService.GetRuntimeTypeDefinition(typeDefinition, out var runtimeTypeDefinition))
        {
            _logger.Debug("Failed to get runtime type definition '{Type}'", typeDefinition);
            members = null!;
            return false;
        }

        members = InjectHelper.Inject(runtimeTypeDefinition, target.GlobalType, target).ToList();
        return true;
    }
}
