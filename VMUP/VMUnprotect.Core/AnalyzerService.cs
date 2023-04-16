using dnlib.DotNet;
using ILogger = Serilog.ILogger;

namespace VMUnprotect.Core;

public class AnalyzerService : IAnalyzerService
{
    private readonly Context _context;
    private readonly ILogger _logger;

    public AnalyzerService(Context context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool AnalyzeVmRuntimeStructure(out VmRuntimeStructure structure)
    {
        _logger.Information("Analyzing VM runtime structure...");

        var module = _context.Module;
        var methods = module
            .GetTypes()
            .SelectMany(t => t.Methods)
            .Where(m => m.Body is not null);

        // skip searching for VMDispatcher and VMCallHandler for now
        var vmDispatcher = module.ResolveToken(0x0600022A) as IMethodDefOrRef;
        var vmCallHandler = module.ResolveToken(0x0600020D) as IMethodDefOrRef;

        _logger.Debug("VMDispatcher: {VMDispatcher}", vmDispatcher);
        _logger.Debug("VMCallHandler: {VMCallHandler}", vmCallHandler);

        structure = new VmRuntimeStructure
        {
            VMDispatcher = vmDispatcher,
            VMCallHandler = vmCallHandler
        };

        return true;
    }
}

public interface IAnalyzerService
{
    bool AnalyzeVmRuntimeStructure(out VmRuntimeStructure structure);
}
