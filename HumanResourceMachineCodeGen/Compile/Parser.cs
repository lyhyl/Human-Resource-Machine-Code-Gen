namespace HumanResourceMachineCodeGen.Compile
{
    public class Parser
    {
        private int line;
        private int col;
        private int position;
        private string code;

        public Parser(string co)
        {
            line = 0;
            col = 0;
            position = 0;
            code = co;
        }

        private bool IsBlankChar(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private bool IsVarChar(char c)
        {
            return c == '*' || c == '_' || ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
        }

        private bool IsDigitChar(char c)
        {
            return '0' <= c && c <= '9';
        }

        private bool IsOperatorChar(char c)
        {
            return c == '>' || c == '<' || c == '=' || c == '!' || c == '+' || c == '-';
        }

        public string PeekNextToken()
        {
            return GetNextToken(false);
        }

        public string GetNextToken(bool movePos = true)
        {
            int be = position;
            while (be < code.Length && IsBlankChar(code[be]))
                be++;
            if (be == code.Length)
                return "";
            int en = be + 1;
            if (IsVarChar(code[be]))
            {
                while (en < code.Length && IsVarChar(code[en]))
                    en++;
            }
            else if (IsOperatorChar(code[be]))
            {
                while (en < code.Length && IsOperatorChar(code[en]))
                    en++;
            }
            else if (IsDigitChar(code[be]))
            {
                while (en < code.Length && IsDigitChar(code[en]))
                    en++;
            }
            string token = code.Substring(be, en - be);
            if (movePos)
                position = en;
            return token;
        }
    }
}
