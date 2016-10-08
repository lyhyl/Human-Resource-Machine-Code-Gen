using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtInc : Emitter
    {
        private string variable;

        public StmtInc(Parser parser, string var)
        {
            variable = var;
            string nt = parser.GetNextToken();
            if (nt != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", nt));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            codes.Add(new HRMCode(HRMInstr.Inc, Helper.GetAddress(vtab, variable)));
        }
    }
}
