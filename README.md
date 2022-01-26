<p align="center">
  <img width="256" heigth="256" src="docs/vmup.png">
<h1 align="center">VMUnprotect.NET</h1>
<p align="center">
  <strong>VMUnprotect</strong> is a project engaged in hunting virtualized <a href="https://vmpsoft.com">VMProtect</a> methods. It makes use of <a href="https://github.com/pardeike/Harmony">Harmony</a> to dynamically read <strong>VMP</strong> behavior. Currently only supports method administration. Works on <a href="https://vmpsoft.com/20210919/vmprotect-3-5-1/">VMProtect 3.5.1</a> (Latest) and few versions back.
</p>
</p>
<p align="center">
  <img src="https://forthebadge.com/images/badges/built-with-love.svg" alt="appveyor-ci" />
  <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg" alt="appveyor-ci" />
</p>
</p>

## Showcase
<img src="docs/show.gif">

# Usage
```sh
VMUnprotect.exe 
  -f, --file             Required. Path to file.
  --usetranspiler        (Default: false) Use an older method that makes use of Transpiler (not recommended).
  --enableharmonylogs    (Default: false) Disable or Enable logs from Harmony.
  --bypassantidebug      (Default: false) Bypass VMProtect Anti Debug.
  --help                 Display this help screen.
  --version              Display version information.
```

# Supported Protections
Note: ***All Supported Protections are working combined***

Protection Name         | Is supported 
------------------------|-------------- 
Memory Protection       | ✓  
Import Protection       | ✓  
Resource Protection     | ✓  
Debugger Detection      | ✓  
Virtualization Tools    | ✓ 
Strip Debug Information | ✓  
Pack the Output File    | ✓ 

# Usage can be found in ```MiddleMan```
```csharp
using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using VMUnprotect.Core.Abstraction;
using VMUnprotect.Core.Helpers;

namespace VMUnprotect.Core.MiddleMan {
    /// <summary>
    ///     Works as Middle Man to make life easier
    /// </summary>
    public static class UnsafeInvokeMiddleMan {
        private static readonly ILogger ConsoleLogger = Engine.Logger;

        /// <summary>
        ///     A prefix is a method that is executed before the original method
        /// </summary>
        public static void Prefix(ref object __instance, ref object obj, ref object[] parameters, ref object[] arguments) {
            var virtualizedMethodName = new StackTrace().GetFrame(7).GetMethod();
            var method = (MethodBase) __instance;

            ConsoleLogger.Print("VMP MethodName: {0} (MDToken {1:X4})", virtualizedMethodName.FullDescription(), virtualizedMethodName.MetadataToken.ToString());
            ConsoleLogger.Print("MethodName: {0}", method.Name);
            ConsoleLogger.Print("FullDescription: {0}", method.FullDescription());
            ConsoleLogger.Print("MethodType: {0}", method.GetType());
            
            if (obj is not null)
                ConsoleLogger.Print("Obj: {0}", obj.GetType());

            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                ConsoleLogger.Print("Parameter ({1}) [{0}]: ({2})", i, parameter.GetType(), Formatter.FormatObject(parameter));
            }

            var returnType = method is MethodInfo info ? info.ReturnType.FullName : "System.Object";
            ConsoleLogger.Print("MDToken: 0x{0:X4}", method.MetadataToken);
            ConsoleLogger.Print("Return Type: {0}", returnType ?? "null");
        }

        /// <summary>
        ///     A postfix is a method that is executed after the original method
        /// </summary>
        public static void Postfix(ref object __instance, ref object __result, ref object obj, ref object[] parameters, ref object[] arguments) {
            ConsoleLogger.Print("Returns: {0}", __result);
        }
    }
}
```

## Current Features
- Tracing invokes in virtualized methods.
- Manipulating parameters and return values.
- Bypass NtQueryInformationProcess, IsLogging, get_IsAttached


## Todo
- Change this to support more VM's
- VMP Stack tracing
- Bypass VMP Debugger Detection ✓
- Bypass VMP CRC Check
- Nice WPF GUI

# FAQ
### What is code virtualization? 
As VMProtect describes it on their's website. Code virtualization is the next step in software protection. Most protection systems encrypt the code and then decrypt it at the application’s startup. VMProtect doesn’t decrypt the code at all! Instead, the encrypted code runs on a virtual CPU that is markedly different from generic x86 and x64 CPUs as the command set is different for each protected file.

### Can it devirtualize VMP?
No, isn't even meant for devirtualization.

# Credits
* [Washi](https://github.com/Washi1337) Overall credits for the project and inspiration with UnsafeInvokeInternal, thanks <3

This tool uses the following (open source) software:
* [dnlib](https://github.com/0xd4d/dnlib) by [0xd4d](https://github.com/0xd4d), licensed under the MIT license, for reading/writing assemblies.
* [Harmony](https://github.com/pardeike/Harmony) by [Andreas Pardeike](https://github.com/pardeike), licensed under the MIT license
* [Serilog](https://github.com/serilog/serilog) provides diagnostic logging to files, the console, and elsewhere. It is easy to set up, has a clean API.


## Want to support this project?
BTC: bc1q048wrqztka5x2syt9mtj68uuf73vqry60s38vf
