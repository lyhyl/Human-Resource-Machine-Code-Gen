using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtBuildInFunc : Emitter
    {
        private string cmd;
        private string var;
        private StmtExpression exp;

        public StmtBuildInFunc(Parser parser, string token)
        {
            cmd = token;
            string nt = parser.GetNextToken();
            if (nt != "(")
                throw new CompileException(ErrorHelper.Unexpected("(", nt));
            var = parser.GetNextToken();
            //exp = new StmtExpression(parser);
            nt = parser.GetNextToken();
            if (nt != ")")
                throw new CompileException(ErrorHelper.Unexpected(")", nt));
            nt = parser.GetNextToken();
            if (nt != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", nt));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if (cmd == "read")
            {
                codes.Add(new HRMCode(HRMInstr.In));
                if (var != "hand")
                    codes.Add(new HRMCode(HRMInstr.Cpyt, Helper.GetAddress(vtab, var)));
            }
            else if (cmd == "write")
            {
                if (var != "hand")
                    codes.Add(new HRMCode(HRMInstr.Cpyf, Helper.GetAddress(vtab, var)));
                codes.Add(new HRMCode(HRMInstr.Out));
            }
            else
                throw new CompileException(ErrorHelper.Unknown);
        }
    }
}
