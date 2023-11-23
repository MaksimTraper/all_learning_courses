using System;

using Compiler.C;
using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler.Nodes
{
    /// <summary>
    /// Унарный узел дерева разбора 
    /// </summary>
    internal class Unary : Node
    {
        #region Children

        public Node child { get; set; }

        protected override Node[] GetChildren()
        {
            return new[] { child };
        }

        #endregion

        internal Unary(NodeTag t, Node c, MyPoint point) : base(t, point)
        {
            this.child = c;
        }
    }
}
