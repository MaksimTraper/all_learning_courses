namespace Compiler.Exceptions 
{
    public class TypesMismatchException : MyException
    {
        public TypesMismatchException(string message = "") : base(
            "Несоответствие типов. " + message, new MyErrorExceptionType())
        { }
    }
}
