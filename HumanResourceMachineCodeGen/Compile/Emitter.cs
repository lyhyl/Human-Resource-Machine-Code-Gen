using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    interface Emitter
    {
        void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv);
    }
}
