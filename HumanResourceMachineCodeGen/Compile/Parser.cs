using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool IsBlank(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private bool IsVarChar(char c)
        {
            return c == '*' || c == '_' || ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
        }

        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        private bool IsOperator(char c)
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
            while (be < code.Length && IsBlank(code[be]))
                be++;
            if (be == code.Length)
                return "";
            int en = be + 1;
            if (IsVarChar(code[be]))
            {
                while (en < code.Length && IsVarChar(code[en]))
                    en++;
            }
            else if (IsOperator(code[be]))
            {
                while (en < code.Length && IsOperator(code[en]))
                    en++;
            }
            else if (IsDigit(code[be]))
            {
                while (en < code.Length && IsDigit(code[en]))
                    en++;
            }
            string token = code.Substring(be, en - be);
            if (movePos)
                position = en;
            return token;
        }
    }
}
