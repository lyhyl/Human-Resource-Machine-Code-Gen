namespace HumanResourceMachineCodeGen.Compile
{
    public struct HRMCode
    {
        public HRMInstr Instruction;
        public int Param;
        public HRMCode(HRMInstr ins, int p = 0)
        {
            Instruction = ins;
            Param = p;
        }
        public override string ToString() => $"{{{Instruction},{Param}}}";
    }
}
