using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtDec : Emitter
    {
        private string variable;

        public StmtDec(Parser parser, string var)
        {
            variable = var;
            string nt = parser.GetNextToken();
            if (nt != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", nt));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            codes.Add(new HRMCode(HRMInstr.Dec, Helper.GetAddress(vtab, variable)));
        }
    }
}
