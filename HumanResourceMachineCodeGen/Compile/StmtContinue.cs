using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtContinue : Emitter
    {
        public StmtContinue(Parser parser)
        {
            string token = parser.GetNextToken();
            if (token != ";")
                throw new CompileException(ErrorHelper.Unexpected(";", token));
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if (ctv.ContinueLoopJump == null)
                throw new CompileException(ErrorHelper.InvailedBC);
            int jmpLab = ctv.JumpLabel++;
            ctv.ContinueLoopJump.Add(jmpLab);
            codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
        }
    }
}
