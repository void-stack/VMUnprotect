using dnlib.DotNet;

namespace VMUnprotect.Core;

public record VmRuntimeStructure
{
    public required IMethodDefOrRef VMDispatcher
    {
        get;
        init;
    }

    public required IMethodDefOrRef VMCallHandler
    {
        get;
        init;
    }
}
