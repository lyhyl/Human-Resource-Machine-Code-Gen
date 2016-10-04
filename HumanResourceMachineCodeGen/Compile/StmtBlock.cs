using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtBlock : Emitter
    {
        List<Emitter> stmts = new List<Emitter>();

        public StmtBlock(Parser parser)
        {
            string token = parser.GetNextToken();
            if (token != "{")
                throw new CompileException(ErrorHelper.Unexpected("{", token));
            token = parser.GetNextToken();
            while (token != "}")
            {
                Emitter stmt = null;
                switch (token)
                {
                    case "if":
                        stmt = new StmtIfElse(parser);
                        break;
                    case "while":
                        stmt = new StmtWhile(parser);
                        break;
                    case "break":
                        stmt = new StmtBreak(parser);
                        break;
                    case "continue":
                        stmt = new StmtContinue(parser);
                        break;
                    case "read":
                    case "write":
                        stmt = new StmtBuildInFunc(parser, token);
                        break;
                    default:
                        stmt = HandleAssignment(parser, token);
                        break;
                }
                stmts.Add(stmt);
                token = parser.GetNextToken();
            }
        }

        private static Emitter HandleAssignment(Parser parser, string token)
        {
            Emitter stmt;
            // hand = exp
            if (token == "hand")
            {
                token = parser.GetNextToken();
                if (token == "++" || token == "--")
                    throw new CompileException(ErrorHelper.IncDecHand);
                if (token != "=")
                    throw new CompileException(ErrorHelper.Unexpected("=", token));
                stmt = new StmtAssignment(parser, "hand");
            }
            else
            {
                string var = token;
                token = parser.GetNextToken();
                switch (token)
                {
                    case "=":
                        // var = exp
                        stmt = new StmtAssignment(parser, var);
                        break;
                    case "++":
                        // var++
                        stmt = new StmtInc(parser, var);
                        break;
                    case "--":
                        // var--
                        stmt = new StmtDec(parser, var);
                        break;
                    default:
                        throw new CompileException(ErrorHelper.Unknown);
                }
            }
            return stmt;
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            foreach (var stmt in stmts)
                stmt.Emit(codes, vtab, ctv);
        }
    }
}
