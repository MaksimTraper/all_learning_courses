using Compiler.Lex; 

namespace Compiler.Exceptions
{
    public class UnexpectedValueException : MyException
    {
        public UnexpectedValueException(MyPoint point, string[] validValues, string message = "") 
            : base ("Непредвиденное значение. Допустимые значения: " 
                    + ValidValuesTemplation(validValues), new MyInvalidDataExceptionType(point))
        { }

        private static string ValidValuesTemplation(string[] validValues)
        {
            string str = "";
            for (int i = 0; i < validValues.Length; i++)
            {
                str += validValues[i];
                if (i != validValues.Length - 1)
                    str += ", ";
            }

            str += ". ";
            return str;
        }
    }
}
