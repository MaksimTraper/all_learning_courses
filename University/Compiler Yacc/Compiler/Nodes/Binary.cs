using System.Linq;
using System;
using Compiler.C;
using Compiler.Exceptions;
using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler.Nodes
{
    /// <summary>
    /// Бинарный узел дерева разбора
    /// </summary>
    internal class Binary : Node
    {
        #region Children

        public Node lhs { get; set; }
        public Node rhs { get; set; }

        protected override Node[] GetChildren()
        {
            return new[] {lhs, rhs};
        }

        #endregion

        internal Binary(NodeTag t, Node l, Node r, MyPoint point) : base(t, point)
        {
            this.lhs = l; this.rhs = r;
        }


        /// <summary>
        /// Выполнение цикла пока выполняется <paramref name="cond"/>
        /// </summary>
        /// <param name="cond">Условие цикла</param> 
        /// <param name="expr">Тело цикла</param>
        public void Condition(Node cond, Node expr, Parser p)
        {
            var type = this.GetChildType(cond, p);

            if (type != MyType.Bool)
                throw new UnexpectedValueTypeException(cond.Point, "Условие должно возвращать Boolean");

            if (cond.Eval(p).Bool)
            {
                //var value = cond.Eval(p);

                //if (!value.Bool)
                //    break;
                expr.Eval(p);
            }
        }

        /// <summary>
        /// Генерация кода на С для операции присвоения
        /// </summary>
        public string AssignCode(Parser p)
        {
            if (rhs.Tag == NodeTag.name)
                if (p.regs.All(reg => reg.Name != rhs.Name))
                    throw new VariableWasntDeclaredException(rhs.Point);

            string name = lhs.Name;

            string typeBlock = "";
            if (p.regs.All(reg => reg.Name != lhs.Name))
            {
                typeBlock = CommonMethods.GetCType(GetChildType(rhs, p)) + " ";
                var type = GetChildType(rhs, p);

                AddToRegs(p, type, name);
            }
            else
            {
                var variable = p.regs.First(reg => reg.Name == name);
                if (variable.GetChildType(variable, p) != GetChildType(rhs, p))
                    throw new TypesMismatchException();
            }
            return typeBlock + name + Constants.Equal + rhs.GenerateCode(p);
        }

        /// <summary>
        /// Добавление переменной в список объявленных переменных
        /// </summary>
        public void AddToRegs(Parser p, MyType type, string name)
        {
            MyValue val = null;
            switch (type)
            {
                case MyType.Bool:
                    val = new MyValue() { Bool = false };
                    break;

                case MyType.Double:
                    val = new MyValue() { Double = 0 };
                    break;

                case MyType.String:
                    val = new MyValue() { String = "" };
                    break;

                default:
                    throw new UnexpectedEnumValueException();
            }

            var leaf = new Leaf(val, new MyPoint(0, 0));
            leaf.Name = name;

            p.regs.Add(leaf);
        }


        
    }
}
