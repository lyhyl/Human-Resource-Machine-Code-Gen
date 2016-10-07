using System.Collections.Generic;
using System.Linq;

namespace HumanResourceMachineCodeGen.Compile
{
    public class StmtExpression : Emitter
    {
        public class Element
        {
            public bool Positive;
            public string Variable;
            public Element(bool pos, string var)
            {
                Positive = pos;
                Variable = var;
            }
        }

        private List<Element> elements = new List<Element>();
        public string Comparer { private set; get; }
        public bool IsConstantExpression { private set; get; }
        public bool ConstantResult { private set; get; }
        public bool RequireSwitchLogic { private set; get; }

        public StmtExpression(Parser parser)
        {
            RequireSwitchLogic = false;
            IsConstantExpression = false;
            string token = parser.PeekNextToken();
            List<string> raw = new List<string>();
            while (IsVariable(token) || IsOperator(token) || IsComparer(token))
            {
                token = parser.GetNextToken();
                raw.Add(token);
                token = parser.PeekNextToken();
            }
            if (raw.Count == 0)
                throw new CompileException(ErrorHelper.EmptyExpression);

            int handc = 0;
            foreach (var tk in raw)
                if (tk == "hand")
                    handc++;
            if (handc > 1)
                throw new CompileException(ErrorHelper.MultiHand);

            ConvertToStandardFormat(raw);
            ReductElement();
            if (elements.Count > 0 && elements[0].Variable == "hand" && !elements[0].Positive)
            {
                RequireSwitchLogic = true;
                foreach (var elem in elements)
                    elem.Positive = !elem.Positive;
                Comparer = OppComparer(Comparer);
            }
        }

        private string OppComparer(string comparer)
        {
            if (comparer == "==")
                return "!=";
            if (comparer == "!=")
                return "==";
            if (comparer == ">")
                return "<=";
            if (comparer == ">=")
                return "<";
            if (comparer == "<")
                return ">=";
            if (comparer == "<=")
                return ">";
            throw new CompileException(ErrorHelper.Unknown);
        }

        private void ReductElement()
        {
            // TODO
        }

        /// <summary>
        /// Standard format: `exp >(=) 0` or  `exp (!= 0)` or `exp == 0`
        /// </summary>
        /// <param name="raw">raw elements</param>
        private void ConvertToStandardFormat(List<string> raw)
        {
            int condi = GetComparer(raw);
            if (condi == -1)
            {
                Comparer = "!=";
                elements = GetElements(raw);
            }
            else
            {
                Comparer = raw[condi];
                List<string> left = new List<string>();
                List<string> right = new List<string>();
                for (int i = 0; i < condi; i++)
                    left.Add(raw[i]);
                for (int i = condi + 1; i < raw.Count; i++)
                    right.Add(raw[i]);
                List<Element> lelem = GetElements(left);
                List<Element> relem = GetElements(right);
                if (lelem.Count == 0 || relem.Count == 0)
                    throw new CompileException(ErrorHelper.EmptyExpression);
                if (Comparer == ">" || Comparer == ">=")
                {
                    // left - right
                    if (!IsZeroVariable(lelem))
                        foreach (var elem in lelem)
                            elements.Add(elem);
                    if (!IsZeroVariable(relem))
                        foreach (var elem in relem)
                            elements.Add(new Element(!elem.Positive, elem.Variable));
                }
                else
                {
                    // if (Comparer == "<" || Comparer == "<=")
                    // if (Comparer == "==" || Comparer == "!=")
                    // right - left
                    if (!IsZeroVariable(relem))
                        foreach (var elem in relem)
                            elements.Add(elem);
                    if (!IsZeroVariable(lelem))
                        foreach (var elem in lelem)
                            elements.Add(new Element(!elem.Positive, elem.Variable));
                    if (Comparer == "<")
                        Comparer = ">";
                    if (Comparer == "<=")
                        Comparer = ">=";
                }
            }
            elements.Sort((Element a, Element b) =>
            {
                if (a.Variable == "hand")
                    return 1;
                if (b.Variable == "hand")
                    return -1;
                return (a.Positive ? -1 : 0) + (b.Positive ? 1 : 0);
            });
            if (elements.Count == 0)
            {
                IsConstantExpression = true;
                if (Comparer == "==")
                    ConstantResult = true;
                else if (Comparer == "!=")
                    ConstantResult = false;
                else
                    ConstantResult = Comparer.Contains("=");
            }
        }

        private bool IsZeroVariable(List<Element> elems)
        {
            return elems.Count == 1 && elems[0].Variable == "0";
        }

        private List<Element> GetElements(List<string> raw)
        {
            List<Element> elem = new List<Element>();
            if (!IsOperator(raw.First()))
                raw.Insert(0, "+");
            if ((raw.Count & 1) != 0)
                throw new CompileException(ErrorHelper.InvailedExpression);
            for (int i = 0; i < raw.Count; i += 2)
            {
                if (!IsOperator(raw[i]))
                    throw new CompileException(ErrorHelper.Unexpected("+'/'-", raw[i]));
                if (!IsVariable(raw[i + 1]))
                    throw new CompileException(ErrorHelper.Unexpected("variable", raw[i + 1]));
                bool pos = raw[i] == "+";
                elem.Add(new Element(pos, raw[i + 1]));
            }
            return elem;
        }

        private int GetComparer(List<string> raw)
        {
            for (int i = 0; i < raw.Count; i++)
                if (IsComparer(raw[i]))
                    return i;
            return -1;
        }

        private bool IsInSet(string token, string[] oper)
        {
            foreach (var op in oper)
                if (token == op)
                    return true;
            return false;
        }

        private bool IsOperator(string token)
        {
            string[] oper = { "+", "-" };
            return IsInSet(token, oper);
        }

        private bool IsComparer(string token)
        {
            string[] oper = { "<", ">", "<=", ">=", "==", "!=" };
            return IsInSet(token, oper);
        }

        private bool IsVariable(string token)
        {
            if (token == "0")
                return true;
            foreach (var c in token)
                if (!(('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '*'))
                    return false;
            return true;
        }

        public void Emit(List<HRMCode> codes, Dictionary<string, int> vtab, CompileTimeVars ctv)
        {
            if (IsConstantExpression)
                return;
            if (elements[0].Variable == "hand")
            {
                if (!elements[0].Positive)
                    throw new CompileException(ErrorHelper.MinusHand);
            }
            else
            {
                int addr = Helper.GetAddress(vtab, elements[0].Variable);
                codes.Add(new HRMCode(HRMInstr.Cpyf, addr));
                if (!elements[0].Positive)
                {
                    // get zero
                    codes.Add(new HRMCode(HRMInstr.Sub, addr));
                    // get -var
                    codes.Add(new HRMCode(HRMInstr.Sub, addr));
                }
            }
            for (int i = 1; i < elements.Count; i++)
                codes.Add(
                    new HRMCode(
                        elements[i].Positive ? HRMInstr.Add : HRMInstr.Sub,
                        Helper.GetAddress(vtab, elements[i].Variable)));
        }
    }
}
