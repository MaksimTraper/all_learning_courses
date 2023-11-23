using System; 

namespace Compiler.Exceptions
{
    public class MyException : Exception
    {
        public MyException(string message, MyExceptionType type) : base(message)
        {
            Type = type;
        }

        public MyExceptionType Type { get; private set; }
    }
}
