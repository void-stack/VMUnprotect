using dnlib.DotNet;
using HarmonyLib;
using System;
using System.Reflection;
using VMUnprotect.Core.Structure;
using ILogger = VMUnprotect.Core.Abstraction.ILogger;

namespace VMUnprotect.Core {
    public class Context {
        public Context(ILogger logger, string vmpAssembly, CommandLineOptions options) {
            Harmony.DEBUG = options.EnableHarmonyLogs;
            
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            VmpAssembly = Assembly.LoadFile(vmpAssembly);
            VmpModuleDefMd = ModuleDefMD.Load(vmpAssembly);

            Harmony = new Harmony("com.hussaryyn.vmup");
        }

        public ILogger Logger { get; }
        public Assembly VmpAssembly { get; }
        public ModuleDefMD VmpModuleDefMd { get; }
        public VmRuntimeStructure? RuntimeStructure { get; set; } = new();
        public Harmony Harmony { get; }
        public CommandLineOptions Options { get; }
    }
}