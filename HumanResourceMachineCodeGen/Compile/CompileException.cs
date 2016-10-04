using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourceMachineCodeGen.Compile
{
    public class CompileException : Exception
    {
        private CompileException()
        {
        }

        public CompileException(string message) : base(message)
        {
        }

        public CompileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
