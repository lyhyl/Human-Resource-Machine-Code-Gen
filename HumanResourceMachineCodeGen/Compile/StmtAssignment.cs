using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtAssignment : Emitter
    {
        private string dest;
        private StmtExpression exp;

        public StmtAssignment(Parser parser, string d)
        {
            dest = d;
            exp = new StmtExpression(parser);
            string token = parser.GetNextToken();
            if (token != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", token));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            exp.Emit(codes, vtab, ctv);
            if (dest != "hand")
                codes.Add(new HRMCode(HRMInstr.Cpyt, Helper.GetAddress(vtab, dest)));
        }
    }
}
