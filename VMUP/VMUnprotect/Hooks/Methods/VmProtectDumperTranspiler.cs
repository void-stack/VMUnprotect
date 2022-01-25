using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VMUnprotect.Methods;
using VMUnprotect.Utils;

namespace VMUnprotect.Hooks.Methods
{
    /// <summary>
    ///     This class holds target functions.
    /// </summary>
    internal class Targets
    {
        /// <summary>
        ///     This function is used by current VMProtect version.
        /// </summary>
        /// <param name="obj">
        ///     The object on which to invoke the method or constructor. If a method is static, this argument is
        ///     ignored. If a constructor is static, this argument must be <see langword="null" /> or an instance of the class that
        ///     defines the constructor.
        /// </param>
        /// <param name="invokeAttr">
        ///     A bitmask that is a combination of 0 or more bit flags from
        ///     <see cref="T:System.Reflection.BindingFlags" />. If <paramref name="binder" /> is <see langword="null" />, this
        ///     parameter is assigned the value <see cref="F:System.Reflection.BindingFlags.Default" />; thus, whatever you pass in
        ///     is ignored.
        /// </param>
        /// <param name="binder">
        ///     An object that enables the binding, coercion of argument types, invocation of members, and
        ///     retrieval of <see langword="MemberInfo" /> objects via reflection. If <paramref name="binder" /> is
        ///     <see langword="null" />, the default binder is used.
        /// </param>
        /// <param name="parameters">
        ///     An argument list for the invoked method or constructor. This is an array of objects with the same number, order,
        ///     and type as the parameters of the method or constructor to be invoked. If there are no parameters, this should be
        ///     <see langword="null" />.
        ///     If the method or constructor represented by this instance takes a ByRef parameter, there is no special attribute
        ///     required for that parameter in order to invoke the method or constructor using this function. Any object in this
        ///     array that is not explicitly initialized with a value will contain the default value for that object type. For
        ///     reference-type elements, this value is <see langword="null" />. For value-type elements, this value is 0, 0.0, or
        ///     <see langword="false" />, depending on the specific element type.
        /// </param>
        /// <param name="culture">
        ///     An instance of <see langword="CultureInfo" /> used to govern the coercion of types. If this is
        ///     <see langword="null" />, the <see langword="CultureInfo" /> for the current thread is used. (This is necessary to
        ///     convert a <see langword="String" /> that represents 1000 to a <see langword="Double" /> value, for example, since
        ///     1000 is represented differently by different cultures.)
        /// </param>
        /// <returns>
        ///     An <see langword="Object" /> containing the return value of the invoked method, or <see langword="null" /> in
        ///     the case of a constructor, or <see langword="null" /> if the method's return type is <see langword="void" />.
        ///     Before calling the method or constructor, <see langword="Invoke" /> checks to see if the user has access permission
        ///     and verifies that the parameters are valid.
        /// </returns>
        public object HookedInvoke(object obj, BindingFlags bindingFlags, Binder binder, object[] parameters, CultureInfo culture, MethodBase methodBase)
        {
            try
            {
                // Indicate this method was called by newer version of VMP.
                ConsoleLogger.Warn("============================================= HookedInvoke =============================================\n");

                // Route the arguments and return value to our middleman function where they can be manipulated or logged.
                return TranspilerMiddleMan.VmpMethodLogger(obj, null, null, ref parameters, null, methodBase);
            }
            catch (Exception ex)
            {
                // Log the exception.
                ConsoleLogger.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        ///     This function is used by older VMProtect version.
        /// </summary>
        /// <param name="obj">
        ///     The object on which to invoke the method or constructor. If a method is static, this argument is
        ///     ignored. If a constructor is static, this argument must be <see langword="null" /> or an instance of the class that
        ///     defines the constructor.
        /// </param>
        /// <param name="parameters">
        ///     An argument list for the invoked method or constructor. This is an array of objects with the same number, order,
        ///     and type as the parameters of the method or constructor to be invoked. If there are no parameters,
        ///     <paramref name="parameters" /> should be <see langword="null" />.
        ///     If the method or constructor represented by this instance takes a <see langword="ref" /> parameter (
        ///     <see langword="ByRef" /> in Visual Basic), no special attribute is required for that parameter in order to invoke
        ///     the method or constructor using this function. Any object in this array that is not explicitly initialized with a
        ///     value will contain the default value for that object type. For reference-type elements, this value is
        ///     <see langword="null" />. For value-type elements, this value is 0, 0.0, or <see langword="false" />, depending on
        ///     the specific element type.
        /// </param>
        /// <returns>
        ///     An object containing the return value of the invoked method, or <see langword="null" /> in the case of a
        ///     constructor.
        /// </returns>
        public object HookedInvokeOld(object obj, object[] parameters, MethodBase methodBase)
        {
            try
            {
                // Indicate this method was called by older version of VMP.
                ConsoleLogger.Warn("============================================= HookedInvokeOld =============================================\n");

                // Route the arguments and return value to our middleman function where they can be manipulated or logged.
                return TranspilerMiddleMan.VmpMethodLogger(obj, null, null, ref parameters, null, methodBase);
            }
            catch (Exception ex)
            {
                // Log the exception.
                ConsoleLogger.Error(ex.StackTrace);
                return null;
            }
        }
    }

    /// <summary>
    ///     This class contains Harmony Patches
    /// </summary>
    public static class VmProtectDumperTranspiler
    {
        /// <summary>A transpiler that replaces all occurrences of a given method with another with additional Ldarg_1 instruction</summary>
        /// <param name="instructions">The enumeration of <see cref="T:HarmonyLib.CodeInstruction" /> to act on</param>
        /// <param name="from">Method to search for</param>
        /// <param name="to">Method to replace with</param>
        /// <returns>Modified enumeration of <see cref="T:HarmonyLib.CodeInstruction" /></returns>
        private static IEnumerable<CodeInstruction> ReplaceVmpInvoke(this IEnumerable<CodeInstruction> instructions, MethodBase from, MethodBase to)
        {
            if ((object) from == null) throw new ArgumentException("Unexpected null argument", nameof(from));
            if ((object) to == null) throw new ArgumentException("Unexpected null argument", nameof(to));

            var code = new List<CodeInstruction>(instructions);

            for (var x = 0; x < code.Count; x++)
            {
                var ins = code[x];
                if (ins.operand as MethodBase != from) continue;

                // replace callvirt Invoke with our debug invoke.
                ins.opcode = OpCodes.Callvirt;
                ins.operand = to;

                // insert additional Ldarg_1 which corresponds to MethodBase of invoked function.
                // TODO: Improve this, can be easily broken by obfuscation or future VMP updates
                code.Insert(x, new CodeInstruction(OpCodes.Ldarg_1));
                ConsoleLogger.Info("Replaced with custom Invoke and injected MethodBase argument at {0}.", x);
            }

            return code.AsEnumerable();
        }

        /// <summary>A transpiler that alters instructions that calls specific method</summary>
        /// <param name="instructions">The enumeration of <see cref="T:HarmonyLib.CodeInstruction" /> to act on</param>
        /// <returns>Modified enumeration of <see cref="T:HarmonyLib.CodeInstruction" /></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            ConsoleLogger.Debug("VMP Function Handler Transpiler");

            var codeInstructions = instructions.ToList();

            // Replace all occurrences of MethodBase.Invoke with our debug version.
            return codeInstructions.ReplaceVmpInvoke(AccessTools.Method(typeof(MethodBase),
                        "Invoke",
                        new[]
                        {
                            typeof(object), typeof(BindingFlags), typeof(Binder), typeof(object[]),
                            typeof(CultureInfo)
                        }),
                    AccessTools.Method(typeof(Targets), nameof(Targets.HookedInvoke)))
                // Older version of VMP
                .ReplaceVmpInvoke(AccessTools.Method(typeof(MethodBase), "Invoke", new[] {typeof(object), typeof(object[])}),
                    AccessTools.Method(typeof(Targets), nameof(Targets.HookedInvokeOld)));
        }
    }
}