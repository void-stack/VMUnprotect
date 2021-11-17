using dnlib.DotNet;
using System;
using System.Linq;
using VMUnprotect.Utils;

namespace VMUnprotect
{
    /// <summary>
    ///     Contains various patterns of VM Structure
    /// </summary>
    public static class VmAnalyzer
    {
        /// <summary>
        ///     Fills VM runtime structure
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Run(Loader context)
        {
            // TODO: Improve this, add Stack inspection, etc.
            var (functionHandler, vmTypeDef) = LocateVmHandlerAndTypDef(Loader.VmpModuleDefMd);

            if (functionHandler is null || vmTypeDef is null)
                throw new ArgumentException("Could not locate VmProtectFunctionHandler.");

            context.RuntimeStructure = new VmRuntimeStructure
            {
                FunctionHandler = functionHandler,
                VmTypeDef = vmTypeDef
            };
        }

    #region VMP_FUNCTION_HANDLER

        /// <summary>
        ///     These locals can be found in VMP Function Handler
        /// </summary>
        private static readonly string[] VmpFunctionHandlerLocals =
        {
            "System.Object", "System.Int32", "System.Reflection.MethodInfo", "System.Reflection.ParameterInfo[]",
            "System.Type[]", "System.Reflection.Emit.DynamicMethod", "System.Reflection.Emit.ILGenerator"
        };

        /// <summary>
        ///     Tries to search and match VMProtect MethodHandler
        /// </summary>
        /// <param name="module">Target Module</param>
        /// <returns>MethodDef and TypeDef of Handler, if not returns NULL</returns>
        private static (MethodDef, TypeDef) LocateVmHandlerAndTypDef(ModuleDef module)
        {
            MethodDef vmpHandler = null;
            TypeDef vmTypeDef = null;

            foreach (var type in module.GetTypes())
            {
                vmpHandler = type.Methods.Where(IsVmpFunctionHandler).FirstOrDefault(method => new LocalTypes(method).All(VmpFunctionHandlerLocals));

                if (vmpHandler == null)
                    continue;

                vmTypeDef = type;
                break;
            }

            return (vmpHandler, vmTypeDef);
        }

        /// <summary>
        ///     Checks RetType and Params, etc of MethodDef
        /// </summary>
        /// <param name="method"></param>
        /// <returns>Does method match the requirements</returns>
        private static bool IsVmpFunctionHandler(MethodDef method)
        {
            return method is {IsStatic: false}
                   && method.MethodSig.GetParamCount() == 2
                   && method.MethodSig.RetType.GetElementType() == ElementType.Class
                   && method.MethodSig.Params[0].GetElementType() == ElementType.Class
                   && method.MethodSig.Params[1].GetElementType() == ElementType.Boolean;
        }

    #endregion

    }
}