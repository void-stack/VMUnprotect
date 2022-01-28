using Autofac;
using System.Reflection;
using VMUnprotect.Runtime.Hooks;
using Module = Autofac.Module;

namespace VMUnprotect.Runtime.Modules
{
    public class VmProtectPatchesModule : Module
    {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AssignableTo<IVmupHook>().AsImplementedInterfaces();
        }
    }
}