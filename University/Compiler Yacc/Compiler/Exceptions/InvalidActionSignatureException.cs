namespace Compiler.Exceptions 
{
    public class InvalidActionSignatureException : MyException
    {
        public InvalidActionSignatureException(string message = "") : base(
            "Действия с такой сигнатурой не зарегистрировано или несколько одинаковых сигнатур. " 
            + message, new MyErrorExceptionType())
        { }
    }
}
