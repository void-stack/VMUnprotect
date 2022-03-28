using System;
using System.Linq;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Helpers;

namespace VMUnprotect.Runtime.Hooks.Methods.AntiDebug
{
    public class NtQueryInformationProcessPatch : Params, INtQueryInformationProcessPatch
    {
        private IntPtr _ntQueryInformationProcessPtr = new(0x0);
        public NtQueryInformationProcessPatch(Context ctx, ILogger logger) : base(ctx, logger) { }
        private Delegate NtQueryInformationProcessDelegate { get; set; }

        //__kernel_entry NTSTATUS NtQueryInformationProcess(
        //    [in]            HANDLE           ProcessHandle,
        //    [in]            PROCESSINFOCLASS ProcessInformationClass,
        //    [out]           PVOID            ProcessInformation,
        //    [in]            ULONG            ProcessInformationLength,
        //    [out, optional] PULONG           ReturnLength
        //);
        public void OverwriteProcessInformation(object obj, ref object[] arguments) {
            if (obj is not null && obj.Equals(NtQueryInformationProcessDelegate))
                arguments[2] = (IntPtr) 0x0; // [out] PVOID ProcessInformation = 0x0 // We don't care about this structure
        }

        public void GetDelegateForFunctionPointer(object __result, object[] parameters) {
            if (parameters == null)
                return;

            if (parameters.Any(x => x.Equals("NtQueryInformationProcess")))
                _ntQueryInformationProcessPtr = (IntPtr) __result;

            if (parameters.Any(x => x.Equals(_ntQueryInformationProcessPtr) &&
                                    _ntQueryInformationProcessPtr != new IntPtr(0x0)))
                NtQueryInformationProcessDelegate = (Delegate) __result;
        }
    }

    public interface INtQueryInformationProcessPatch
    {
        public void OverwriteProcessInformation(object obj, ref object[] arguments);
        public void GetDelegateForFunctionPointer(object __result, object[] parameters);
    }
}