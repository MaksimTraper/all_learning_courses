namespace Compiler.Exceptions 
{
    public class UnexpectedEnumValueException : MyException
    {
        public UnexpectedEnumValueException(string message = "") : base (
            "Непредвиденное значение перечисления " 
            + message, new MyErrorExceptionType())
        { }
    }
}
