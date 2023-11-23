namespace Compiler.Exceptions 
{
    public class CircularEvalException : MyException
    {
        public CircularEvalException(string message = "") : base(
            "Обнаружен узел, имеющий рекурсивное дерево разбора. " 
            + message, new MyErrorExceptionType())
        { }
    }
}
