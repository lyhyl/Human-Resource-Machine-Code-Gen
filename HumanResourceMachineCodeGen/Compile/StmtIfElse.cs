using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtIfElse : Emitter
    {
        private StmtExpression cond;
        private StmtBlock ifBlock = null, elseBlock = null;

        public StmtIfElse(Parser parser)
        {
            string token = parser.GetNextToken();
            if (token != "(")
                throw new CompileException(ErrorHelper.Unexpected("(", token));
            cond = new StmtExpression(parser);
            token = parser.GetNextToken();
            if (token != ")")
                throw new CompileException(ErrorHelper.Unexpected(")", token));
            ifBlock = new StmtBlock(parser);
            token = parser.PeekNextToken();
            if (token == "else")
            {
                token = parser.GetNextToken();
                elseBlock = new StmtBlock(parser);
            }
            if(cond.RequireSwitchLogic)
            {
                var tmp = ifBlock;
                ifBlock = elseBlock;
                elseBlock = tmp;
            }
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if(cond.IsConstantExpression)
            {
                if (cond.ConstantResult)
                    ifBlock?.Emit(codes, vtab, ctv);
                else
                    elseBlock?.Emit(codes, vtab, ctv);
            }
            else if (cond.Comparer == "==" || cond.Comparer == "!=")
                EmitEqu(codes, vtab, ctv);
            else
                EmitRel(codes, vtab, ctv);
        }

        private void EmitRel(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            cond.Emit(codes, vtab, ctv);
            int jnLab = ctv.JumpLabel++, jzLab = int.MaxValue;
            codes.Add(new HRMCode(HRMInstr.Jn, jnLab));
            if (!cond.Comparer.Contains("="))
            {
                jzLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Jz, jzLab));
            }
            ifBlock?.Emit(codes, vtab, ctv);
            int jmpLab = ctv.JumpLabel++;
            codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
            if (!cond.Comparer.Contains("="))
                codes.Add(new HRMCode(HRMInstr.Lab, jzLab));
            codes.Add(new HRMCode(HRMInstr.Lab, jnLab));
            elseBlock?.Emit(codes, vtab, ctv);
            codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
        }

        private void EmitEqu(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            cond.Emit(codes, vtab, ctv);
            int jzLab = ctv.JumpLabel++;
            codes.Add(new HRMCode(HRMInstr.Jz, jzLab));
            if (cond.Comparer == "!=")
                ifBlock?.Emit(codes, vtab, ctv);
            else
                elseBlock?.Emit(codes, vtab, ctv);
            int jmpLab = ctv.JumpLabel++;
            codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
            codes.Add(new HRMCode(HRMInstr.Lab, jzLab));
            if (cond.Comparer == "!=")
                elseBlock?.Emit(codes, vtab, ctv);
            else
                ifBlock?.Emit(codes, vtab, ctv);
            codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
        }
    }
}
