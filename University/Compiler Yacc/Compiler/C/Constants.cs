using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.C
{
    public class Constants
    {
        public const string Equal = " = ";
        public const string If = "if";
        public const string Div = "/";
        public const string Minus = "-";
        public const string Plus = "+";
        public const string Mul = "*";
        public const string Smaller = "<=";
        public const string Bigger = ">=";
        public const string Disjunction = "||";
        public const string Exit = "return 0";
        public const string Standart = "std";
        public const string StreamOut = Standart + "::cout";
        public const string Write = StreamOut + ".write";
        public const string Power = "pow";
        public const string Sqrt = "sqrt";
        public const string NotEqual = "!=";
    }
}
