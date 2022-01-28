using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using VMUnprotect.Runtime.General;
using VMUnprotect.Runtime.Helpers;

// ReSharper disable InconsistentNaming

namespace VMUnprotect.Runtime.MiddleMan
{
    public class UnsafeInvokeMiddleMan : Params, IUnsafeInvokeMiddleMan
    {
        public UnsafeInvokeMiddleMan(Context ctx, ILogger logger) : base(ctx, logger) { }

        /// <summary>
        ///     A prefix is a method that is executed before the original method
        /// </summary>
        public bool Prefix(
            ref object __result,
            ref object __instance,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments) {
            var virtualizedMethodName = new StackTrace().GetFrame(7).GetMethod();
            var method = (MethodBase) __instance;

            Logger.Print("VMP MethodName: {0} (MDToken 0x{1:X4})", virtualizedMethodName.FullDescription(),
                         virtualizedMethodName.MetadataToken.ToString());
            Logger.Print("MethodName: {0}", method.Name);
            Logger.Print("FullDescription: {0}", method.FullDescription());
            Logger.Print("MethodType: {0}", method.GetType());

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (obj is not null)
                Logger.Print("Obj: {0}", Formatter.FormatObject(obj));

            // Loop through parameters and log them
            for (var i = 0; i < parameters.Length; i++) {
                var parameter = parameters[i];
                Logger.Print("Parameter ({1}) [{0}]: ({2})", i, parameter.GetType(), Formatter.FormatObject(parameter));
            }

            var returnType = method is MethodInfo info ? info.ReturnType.FullName : "System.Object";
            Logger.Print("MDToken: 0x{0:X4}", method.MetadataToken);
            Logger.Print("Return Type: {0}", returnType ?? "null");
            return true;
        }

        /// <summary>
        ///     A postfix is a method that is executed after the original method
        /// </summary>
        public void Postfix(
            ref object __instance,
            ref object __result,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments) {
            Logger.Print("Returns: {0}", __result);
        }
    }

    public interface IUnsafeInvokeMiddleMan
    {
        public void Postfix(
            ref object __result,
            ref object __instance,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments);

        public bool Prefix(
            ref object __result,
            ref object __instance,
            ref object obj,
            ref object[] parameters,
            ref object[] arguments);
    }
}