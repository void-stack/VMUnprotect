using dnlib.DotNet;
using System;
using System.Reflection;
using VMUnprotect.Runtime.Helpers;

namespace VMUnprotect.Runtime.General
{
    internal class AssemblyLoader : Params, IAssemblyLoader
    {
        public AssemblyLoader(Context ctx, ILogger logger) : base(ctx, logger) { }

        public void LoadAssembly() {
            var assembly = Ctx.Project.TargetFilePath;

            try {
                Ctx.Module = ModuleDefMD.Load(assembly);
                Ctx.Assembly = Assembly.LoadFile(assembly);
                Logger.Info("Loaded assembly '{0}'", Ctx.Module.Name);
            } catch (Exception exception) {
                Logger.Info("Failed to load assembly '{0}'", assembly);
                Logger.Error($"Stacktrace: {exception}");
                Environment.Exit(0);
            }
        }
    }

    public interface IAssemblyLoader
    {
        void LoadAssembly();
    }
}