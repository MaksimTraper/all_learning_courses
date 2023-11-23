using System.Collections.Generic;

using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler.Nodes
{
    /// <summary>
    /// Последовательность узлов дерева разбора
    /// </summary>
    internal class Sequence : Node
    {
        public Sequence(NodeTag tag, Node node, MyPoint point) : this(tag, point)
        {
            InitNode(node);
        }

        public Sequence(NodeTag tag, Node first, Node second, MyPoint point) : this(tag, point)
        {
            InitNode(first);
            InitNode(second);
        }

        private Sequence(NodeTag tag, MyPoint point) : base(tag, point)
        {
            Query = new List<Node>();
        }

        /// <summary>
        /// Включение узла в последовательность без вложенности
        /// </summary> 
        /// <param name="node">Узел</param>
        private void InitNode(Node node)
        {
            if ((node as Sequence) != null)
                Query.AddRange((node as Sequence).GetActions());
            else
                Query.Add(node);
        }

        /// <summary>
        /// Вычисление дочерних узлов
        /// </summary>
        /// <param name="p"></param>
        public void EvalChildren(Parser p)
        {
            for (int i = 0; i < Query.Count; i++)
                Query[i].Eval(p);
        }

        /// <summary>
        /// Получение непосредственных дочерних элементов
        /// </summary>
        protected override Node[] GetChildren()
        {
            return Query.ToArray();
        }

        /// <summary>
        /// Получение всех дочерних элементов
        /// </summary>
        public List<Node> GetActions()
        {
            List<Node> actions = new List<Node>();
            foreach (var action in Query)
            {
                if ((action as Sequence) != null)
                    actions.AddRange((action as Sequence).GetActions());
                else
                    actions.Add(action);
            }

            return actions;
        }

        public List<Node> Query { get; private set; }
    }
}
