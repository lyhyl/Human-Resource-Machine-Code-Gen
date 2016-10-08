using System.Collections.Generic;

namespace HumanResourceMachineCodeGen.Compile
{
    public class CompileTimeVars
    {
        public int JumpLabel { set; get; }
        public HashSet<int> BreakLoopJump { set; get; }
        public HashSet<int> ContinueLoopJump { set; get; }

        public CompileTimeVars(string code)
        {
            JumpLabel = 0;
        }
    }
}
