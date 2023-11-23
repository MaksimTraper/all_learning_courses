using Compiler.Lex;

namespace Compiler.Exceptions
{
    /// <summary>
    /// Исключение, вызванное ошибкой в программе
    /// </summary>
    public class MyErrorExceptionType : MyExceptionType
    {

    }
    
    /// <summary> 
    /// Исключение, вызванное некорректным вводом пользователя
    /// </summary>
    public class MyInvalidDataExceptionType : MyExceptionType
    {
        public MyInvalidDataExceptionType(MyPoint point)
        {
            Point = point;
        }

        public MyPoint Point { get; private set; }
    }

    public abstract class MyExceptionType
    {

    }
}