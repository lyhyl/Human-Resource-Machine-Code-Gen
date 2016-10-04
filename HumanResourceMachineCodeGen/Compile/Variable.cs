using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    public enum VariableType { RefVar, StoVar, ZeroVar, HandVar }
    public class Variable
    {
        VariableType Type;
        public int Storage;
        public Variable(VariableType type, int storage)
        {
            Type = type;
            Storage = storage;
        }
    }
}
