using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtContinue : Emitter
    {
        private Parser parser;

        public StmtContinue(Parser parser)
        {
            this.parser = parser;
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
        }
    }
}
