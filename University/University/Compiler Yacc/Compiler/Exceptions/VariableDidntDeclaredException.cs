using Compiler.Lex; 

namespace Compiler.Exceptions
{
    public class VariableWasntDeclaredException : MyException
    {
        public VariableWasntDeclaredException(MyPoint point, string message = "")
            : base("Переменная не была объявлена. " 
                   + message, new MyInvalidDataExceptionType(point))
        { }
    }
}
