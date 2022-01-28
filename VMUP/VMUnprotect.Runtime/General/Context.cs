using Autofac;
using dnlib.DotNet;
using HarmonyLib;
using System.Reflection;
using VMUnprotect.Runtime.Structure;

namespace VMUnprotect.Runtime.General
{
    public class Context
    {
        public Context(Project project, ILogger logger, CommandLineOptions options) {
            Logger = logger;
            Project = project;
            Options = options;
        }

        public Project Project { get; }
        public Assembly Assembly { get; internal set; }
        public ModuleDef Module { get; internal set; }
        public Harmony Harmony { get; internal set; }
        public CommandLineOptions Options { get; }
        public VmRuntimeStructure VmRuntimeStructure { get; internal set; }

        public ILogger Logger { get; }
        public ILifetimeScope Scope { get; internal set; }
    }
}