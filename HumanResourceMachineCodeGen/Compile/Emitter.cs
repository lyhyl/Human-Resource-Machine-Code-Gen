using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    interface Emitter
    {
        void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv);
    }
}
