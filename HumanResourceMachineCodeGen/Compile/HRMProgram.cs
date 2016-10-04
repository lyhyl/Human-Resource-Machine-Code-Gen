using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class HRMProgram
    {
        private Parser parser;
        private StmtBlock block;
        private Dictionary<string, int> varTab = new Dictionary<string, int>();
        private CompileTimeVars ctv;

        public HRMProgram(string code)
        {
            parser = new Parser(code);
            ctv = new CompileTimeVars(code);
            string token = parser.GetNextToken();
            if (token == "var")
                CreateVarTab();
            block = new StmtBlock(parser);
            token = parser.GetNextToken();
            if (!string.IsNullOrWhiteSpace(token))
                throw new CompileException(ErrorHelper.UnexpectedEndOfProgram);
        }

        private void CreateVarTab()
        {
            HashSet<int> sa = new HashSet<int>();
            Dictionary<string, int> tmpVarTab = new Dictionary<string, int>();
            while (true)
            {
                string token = parser.GetNextToken();
                string name = token;
                int s = -1;
                token = parser.GetNextToken();
                if (token == "(")
                {
                    token = parser.GetNextToken();
                    if (!int.TryParse(token, out s))
                        throw new CompileException(ErrorHelper.BadAddress);
                    if (s < 0)
                        throw new CompileException(ErrorHelper.BadAddress);
                    token = parser.GetNextToken();
                    if (token != ")")
                        throw new CompileException(ErrorHelper.Unexpected("(", token));
                    token = parser.GetNextToken();
                }
                if (token != "," && token != ";")
                    throw new CompileException(ErrorHelper.Unexpected(",' or ';", token));
                tmpVarTab[name] = s;
                sa.Add(s);
                if (token == ";")
                    break;
            }
            int addr = 1;
            foreach (var tvt in tmpVarTab)
            {
                if (tvt.Value != -1)
                    varTab[tvt.Key] = tvt.Value;
                else
                {
                    while (sa.Contains(addr))
                        addr++;
                    varTab[tvt.Key] = addr++;
                }
            }
        }

        public List<HRMCode> Emit()
        {
            List<HRMCode> codes = new List<HRMCode>();
            block.Emit(codes, varTab, ctv);
            return codes;
        }
    }
}
