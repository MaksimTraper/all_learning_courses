using System;
using Compiler.Exceptions;
using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler.Nodes
{
    /// <summary>
    /// Тернарный узел дерева разбора
    /// </summary>
    internal class Ternary : Node
    {
        #region

        public Node First { get; set; }
        public Node Second { get; set; }
        public Node Third { get; set; }

        protected override Node[] GetChildren()
        {
            return new[] { First, Second, Third };
        }

        #endregion

        internal Ternary(NodeTag t, Node first, Node second, Node third, MyPoint point) : base(t, point)
        {
            First = first;
            Second = second;
            Third = third;
        }

        /// <summary>
        /// Выполнение операции выделения подстроки
        /// </summary>
        public string GetString(Parser p)
        {
            int second = 0;
            bool secondCheck = int.TryParse(Second.Eval(p).Double.ToString(), out second);
            int third = 0;
            bool thirdCheck = int.TryParse(Third.Eval(p).Double.ToString(), out third);
            if (!(secondCheck || thirdCheck))
                throw new TypesMismatchException();

            string first = First.Eval(p).String;

            if (second + third > first.Length)
                throw new MyException("Выход за границы строки", new MyInvalidDataExceptionType(Point));

            return first.Substring(second, third);
        }
    }
}
