using dnlib.DotNet;

namespace VMUnprotect.Core.Structure {
    /// <summary>
    ///     Structure of VMP Virtual Machine
    /// </summary>
    public class VmRuntimeStructure {
        /// <summary>
        ///     TypeDefinition of Virtual Machine
        /// </summary>
        public TypeDef? VmTypeDef
            {
                get;
                set;
            }

        /// <summary>
        ///     Call handler of VMP's virtual machine
        /// </summary>
        public MethodDef? FunctionHandler
            {
                get;
                set;
            }
    }
}