using System.Collections.Generic;
using System.Linq;

using Compiler.Exceptions;
using Compiler.Helpers;
using Compiler.Lex;
using Compiler.Nodes.Actions;

namespace Compiler.Nodes
{
    /// <summary>
    /// Узел дерева разбора
    /// </summary>
    internal abstract class Node
    {
        protected Node(NodeTag tag, MyPoint point)
        {
            Tag = tag;
            Point = point;
        }

        /// <summary>  
        /// Получение дочерних узлов
        /// </summary>
        protected abstract Node[] GetChildren();

        /// <summary>
        /// Тег выполняемой операции
        /// </summary>
        public NodeTag Tag { get; private set; }

        /// <summary>
        /// Имя узла (имя переменной или значения, соответствующего некоторой переменной)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Флаг опеределяющий разбирается ли в текущий момент данный узел
        /// </summary>
        protected bool active = false;
        
        /// <summary>
        /// Вычисление значения узла
        /// </summary>
        public MyValue Eval(Parser p)
        {
            try
            {
                this.Prolog();

                var childrenTypes = GetChildrenTypes(this, p);
                var actions = ActionsManager.GetActions(this.GetType()).Where(func => func.Tag == Tag && 
                                                                              func.AreArgumentsCorrect(childrenTypes)).ToList();

                if (actions.Count == 1)
                    return actions[0].Action(p, this);

                throw new TypesMismatchException();
            }
            finally
            {
                this.Epilog();
            }
        }

        /// <summary>
        /// Получение типов, возвращаемых дочерними узлами
        /// </summary>
        /// <param name="node">Текущий узел</param>
        private MyType[] GetChildrenTypes(Node node, Parser p)
        {
            var children = node.GetChildren();
            if (children.Length != 0)
            {
                var types = new List<MyType>();
                foreach (var child in children)
                {
                    types.Add(GetChildType(child, p));
                }

                return types.ToArray();
            }

            return new MyType[] { };
        }

        /// <summary>
        /// Получение типа, возвращаемого узлом
        /// </summary>
        /// <param name="node">Узел</param>
        public MyType GetChildType(Node node, Parser p)
        {
            var types = GetChildrenTypes(node, p);

            var actions = ActionsManager.GetActions(node.GetType()).Where(func => func.Tag == node.Tag && 
                                                                          func.AreArgumentsCorrect(types)).ToList();

            if (actions.Count == 1)
                return actions[0].OutTypeFunc.Invoke(p, node);

            throw new InvalidActionSignatureException();
        }

        /// <summary>
        /// Генерация кода на С
        /// </summary>
        public string GenerateCode(Parser p)
        {
            try
            {
                this.Prolog();

                var childrenTypes = GetChildrenTypes(this, p);
                var actions = ActionsManager.GetActions(this.GetType()).Where(func => func.Tag == Tag &&
                                                                                      func.AreArgumentsCorrect(childrenTypes)).ToList();

                if (actions.Count == 1)
                    return actions[0].CodeGenerateFunc(p, this);

                throw new TypesMismatchException();
            }
            finally
            {
                this.Epilog();
            }
        }

        /// <summary>
        /// Проверка на зацикливание разбора, установка флага
        /// </summary>
        public void Prolog()
        {
            if (this.active)
                throw new CircularEvalException();
            this.active = true;
        }

        /// <summary>
        /// Окончание разбора
        /// </summary>
        public void Epilog()
        {
            this.active = false;
        }

        /// <summary>
        /// Положение узла в тексте программы
        /// </summary>
        public MyPoint Point { get; private set; }
    }
}
