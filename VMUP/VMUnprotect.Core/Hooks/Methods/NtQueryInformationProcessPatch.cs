using System;
using System.Linq;

namespace VMUnprotect.Core.Hooks.Methods {
    public static class NtQueryInformationProcessPatch {
        private static IntPtr _ntQueryInformationProcessPtr = new(0x0);
        private static Delegate? NtQueryInformationProcessDelegate { get; set; }

        public static void OverwriteProcessInformation(object obj, ref object[] arguments) {
            if (!Engine.Ctx.Options.BypassAntiDebug)
                return;

            /*__kernel_entry NTSTATUS NtQueryInformationProcess(
                [in]            HANDLE           ProcessHandle,
                [in]            PROCESSINFOCLASS ProcessInformationClass,
                [out]           PVOID            ProcessInformation,
                [in]            ULONG            ProcessInformationLength,
                [out, optional] PULONG           ReturnLength
            );*/

            if (obj is not null && obj.Equals(NtQueryInformationProcessDelegate))
                arguments[2] = (IntPtr) 0x0; // [out] PVOID ProcessInformation = 0x0
        }

        public static void GetDelegateForFunctionPointer(object __result, object[] parameters) {
            if (!Engine.Ctx.Options.BypassAntiDebug)
                return;

            // Steal IntPtr 
            if (parameters.Any(x => x.Equals("NtQueryInformationProcess")))
                _ntQueryInformationProcessPtr = (IntPtr) __result;

            // Steal Delegate
            if (parameters.Any(x => x.Equals(_ntQueryInformationProcessPtr)))
                NtQueryInformationProcessDelegate = (Delegate) __result;
        }
    }
}