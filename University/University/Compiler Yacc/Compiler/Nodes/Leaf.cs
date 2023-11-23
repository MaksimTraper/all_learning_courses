using System;
using System.Linq;

using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler.Nodes
{
    /// <summary>
    /// Лист дерева разбора 
    /// </summary>
    internal class Leaf : Node
    {
        #region Children

        protected override Node[] GetChildren()
        {
            return new Node[] { };
        }

        #endregion

        public MyValue Value { get; private set; }

        internal Leaf(NodeTag tag, MyPoint point) : base(tag, point)
        { }

        internal Leaf(string str, MyPoint point) : base(NodeTag.name, point)
        {
            this.Name = str;
        }

        internal Leaf(MyValue val, MyPoint point) : base(NodeTag.literal, point)
        {
            Value = val;
        }

        /// <summary>
        /// Получение возвращаемого типа 
        /// </summary>
        public MyType GetOutType(Parser p)
        {//tag = name
            var pair = GetPair(p);

            return pair != null
                ? pair.GetChildType(pair, p)
                : MyType.None;
        }

        /// <summary>
        /// Получение пары имя-значение по названию
        /// </summary>
        private Node GetPair(Parser p)
        {
            return p.regs.FirstOrDefault(reg => reg.Name == Name);
        }
    }
}
