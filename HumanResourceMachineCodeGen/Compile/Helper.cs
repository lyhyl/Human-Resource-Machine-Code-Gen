using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class Helper
    {
        public static int GetAddress(Dictionary<string, int> vtab, string variable)
        {
            bool addr = variable[0] == '*';
            string v = addr ? variable.Substring(1) : variable;
            if (!vtab.ContainsKey(v))
                throw new CompileException(ErrorHelper.UndefinedVariable(v));
            return addr ? -vtab[v] : vtab[v];
        }
    }
}
