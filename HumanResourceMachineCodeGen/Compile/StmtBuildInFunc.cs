using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtBuildInFunc : Emitter
    {
        private string cmd;

        public StmtBuildInFunc(Parser parser, string token)
        {
            cmd = token;
            string nt = parser.GetNextToken();
            if (nt != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", nt));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if (cmd == "read")
                codes.Add(new HRMCode(HRMInstr.In));
            else if (cmd == "write")
                codes.Add(new HRMCode(HRMInstr.Out));
            else
                throw new CompileException(ErrorHelper.Unknown);
        }
    }
}
