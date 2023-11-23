using System;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций для узла типа <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Тип узла</typeparam>
    internal abstract class BaseNodeActions<T> : BaseNodeActions
        where T : Node
    {
        protected BaseNodeActions(MyAction<T>[] actions) : base(typeof(T), actions as MyAction[])
        { }

        public MyAction<T>[] ActionsTyped()
        {
            return Actions as MyAction<T>[];
        }
    }

    /// <summary>
    /// Набор нетипизированных операций для узла
    /// </summary>
    internal abstract class BaseNodeActions 
    {
        public BaseNodeActions(Type targetType, MyAction[] actions)
        {
            TargetType = targetType;
            Actions = actions;
        }

        public Type TargetType { get; private set; }

        public MyAction[] Actions { get; private set; }
    }
}
