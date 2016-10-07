namespace HumanResourceMachineCodeGen.Compile
{
    public class ErrorHelper
    {
        public static string Unexpected(string ex, string got)
            => $"Expect '{ex}', but get '{got}'";

        public static string UndefinedVariable(string v)
            => $"Undefined variable `{v}`";

        public static readonly string InvailedBC = "break/continue must inside while block";

        public static readonly string EmptyExpression = "Empty expression is not allowed";
        public static readonly string InvailedExpression = "Invailed expression";

        public static readonly string MultiHand = "`hand` can only appear once in one expression";
        public static readonly string MinusHand = "`-hand` is not allowed";
        public static readonly string IncDecHand = "`hand` cannot '++'/'--'";

        public static readonly string UnexpectedEndOfProgram = "Get unexpected token at the end of program";
        public static readonly string BadAddress = "Bad address";
        public static readonly string Unknown = "Unknown syntax";
    }
}
