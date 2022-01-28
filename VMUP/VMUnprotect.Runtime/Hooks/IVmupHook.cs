using HarmonyLib;

namespace VMUnprotect.Runtime.Hooks
{
    public interface IVmupHook
    {
        void Patch(Harmony instance);
        void Restore(Harmony instance);
    }
}