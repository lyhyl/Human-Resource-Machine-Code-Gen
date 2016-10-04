using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtWhile : Emitter
    {
        private StmtExpression cond;
        private StmtBlock loop;

        public StmtWhile(Parser parser)
        {
            string token = parser.GetNextToken();
            if (token != "(")
                throw new CompileException(ErrorHelper.Unexpected("(", token));
            cond = new StmtExpression(parser);
            token = parser.GetNextToken();
            if (token != ")")
                throw new CompileException(ErrorHelper.Unexpected(")", token));
            loop = new StmtBlock(parser);
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if(cond.IsConstantExpression)
            {
                if(cond.ConstantResult)
                {
                    int jmpLab = ctv.JumpLabel++;
                    codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
                    loop.Emit(codes, vtab, ctv);
                    codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
                }
            }
            else
            {
                if (cond.Comparer == "==" || cond.Comparer == "!=")
                    EmitEqu(codes, vtab, ctv);
                else
                    EmitRel(codes, vtab, ctv);
            }
        }

        private void EmitEqu(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if (cond.Comparer == "!=")
            {
                int jmpLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
                cond.Emit(codes, vtab, ctv);
                int jzLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Jz, jzLab));
                loop.Emit(codes, vtab, ctv);
                codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
                codes.Add(new HRMCode(HRMInstr.Lab, jzLab));
            }
            else
            {
                int jmpLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
                cond.Emit(codes, vtab, ctv);
                int jzLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Jz, jzLab));
                int joutLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Jmp, joutLab));
                codes.Add(new HRMCode(HRMInstr.Lab, jzLab));
                loop.Emit(codes, vtab, ctv);
                codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
                codes.Add(new HRMCode(HRMInstr.Lab, joutLab));
            }
        }

        private void EmitRel(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            int jmpLab = ctv.JumpLabel++;
            codes.Add(new HRMCode(HRMInstr.Lab, jmpLab));
            cond.Emit(codes, vtab, ctv);
            int jnLab = ctv.JumpLabel++;
            codes.Add(new HRMCode(HRMInstr.Jn, jnLab));
            int jzLab = int.MaxValue;
            if (!cond.Comparer.Contains("="))
            {
                jzLab = ctv.JumpLabel++;
                codes.Add(new HRMCode(HRMInstr.Jz, jzLab));
            }
            loop.Emit(codes, vtab, ctv);
            codes.Add(new HRMCode(HRMInstr.Jmp, jmpLab));
            if (!cond.Comparer.Contains("="))
                codes.Add(new HRMCode(HRMInstr.Lab, jzLab));
            codes.Add(new HRMCode(HRMInstr.Lab, jnLab));
        }
    }
}
