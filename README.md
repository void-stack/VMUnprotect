# In this version VMUnprotect.Runtime gets injected into target assembly`

This for now is very wip, in `AnalyzerService.cs` for now two lines are hardcoded.

```csharp
var vmDispatcher = module.ResolveToken(0x0600022A) as IMethodDefOrRef;
var vmCallHandler = module.ResolveToken(0x0600020D) as IMethodDefOrRef;
```

also in `Unprotect.cs`, where `method_61` is name of `vmDispatcher`.
`EDBAECB7.B5AE7F33` is VMProtect opcode to call other virtualized method.

```csharp
string callingMethod = Utils.GetCallingMethod("method_61");

if (callingMethod == "EDBAECB7.B5AE7F33")
    callingMethod = "VMCall";
```

# Example output
<img src="docs\example.png"/>