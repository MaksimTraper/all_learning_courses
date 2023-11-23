using Compiler.Lex; 

namespace Compiler.Exceptions
{
    public class UnexpectedValueTypeException : MyException
    {
        public UnexpectedValueTypeException(MyPoint point, string message = "") : base(
            "Непредвиденный тип выражения. " + message, new MyInvalidDataExceptionType(point))
        { }
    }
}
