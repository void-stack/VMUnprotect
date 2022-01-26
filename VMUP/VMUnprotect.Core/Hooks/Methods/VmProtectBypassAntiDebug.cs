using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace VMUnprotect.Core.Hooks.Methods {
    /// <summary>
    ///     Honestly, I Don't know about this, need better Idea
    /// </summary>
    public static class VmProtectBypassAntiDebug {
        public static bool Filter(out object __result, object obj, IList<object> arguments, MethodInfo? methodInfo) {
            if (!Engine.Ctx.Options.BypassAntiDebug) {
                __result = true; // dont skip original function
                return true;
            }

            // GetExecutingAssembly.Get_Location() check 
            if (methodInfo == GetExecutingAssembly) {
                __result = Engine.Ctx.VmpAssembly;
                return false;
            }

            // Bypass IsLogging
            if (methodInfo == IsLogging) {
                __result = false;
                return false; // skip function and set __result to false
            }

            // Bypass GetIsAttached
            if (methodInfo == GetIsAttached) {
                __result = false;
                return false; // skip function and set __result to false
            }

            // Bypass NtQueryInformationProcess
            /*__kernel_entry NTSTATUS NtQueryInformationProcess(
                [in]            HANDLE           ProcessHandle,
                [in]            PROCESSINFOCLASS ProcessInformationClass,
                [out]           PVOID            ProcessInformation,
                [in]            ULONG            ProcessInformationLength,
                [out, optional] PULONG           ReturnLength
            );*/

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (obj is not null && obj.Equals(NtQueryInformationProcessDelegate))
                arguments[2] = (IntPtr) 0x0; // [out] PVOID ProcessInformation = 0x0

            __result = true; // dont skip original function
            return true;
        }

        /// <summary>
        ///     There's a function that takes "NtQueryInformationProcess" and converts it into IntPtr for
        ///     GetDelegateForFunctionPointer.
        /// </summary>
        public static void FindNtQueryInformationProcessDelegate(object __result, object[] parameters) {
            if (!Engine.Ctx.Options.BypassAntiDebug)
                return;

            // Steal IntPtr
            if (parameters.Any(x => x.Equals("NtQueryInformationProcess")))
                _ntQueryInformationProcessPtr = (IntPtr) __result;

            // Steal Delegate
            if (parameters.Any(x => x.Equals(_ntQueryInformationProcessPtr)))
                NtQueryInformationProcessDelegate = (Delegate) __result;
        }

        #region VMP_ANTIDEBUG_FUNCTIONS
        private static IntPtr _ntQueryInformationProcessPtr = new(0x0);
        private static Delegate? NtQueryInformationProcessDelegate { get; set; }
        private static MethodInfo? GetExecutingAssembly => AccessTools.Method(typeof(Assembly), nameof(Assembly.GetExecutingAssembly));
        private static MethodInfo? GetIsAttached => AccessTools.Method(typeof(Debugger), "get_IsAttached");
        private static MethodInfo? IsLogging => AccessTools.Method(typeof(Debugger), "IsLogging");
        #endregion
    }
}