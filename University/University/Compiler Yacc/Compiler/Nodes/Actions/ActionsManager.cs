using System;
using System.Collections.Generic;
using System.Linq;

using Compiler.Exceptions;
using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Класс, отвечающий за хранение и выдачу наборов операций для узлов в соответствии с их типами
    /// </summary>
    internal static class ActionsManager
    {
        static ActionsManager()
        {
            Init();
        }

        /// <summary>
        /// Поиск и регистрация наборов операций 
        /// </summary>
        private static void Init()
        {
            var types = typeof(ActionsManager).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (CommonMethods.IsBasedOn(type, typeof(BaseNodeActions)))
                {
                    if (!type.IsAbstract)
                    {
                        var constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                            _actionsGroups.Add(type, (BaseNodeActions)constructor.Invoke(null));
                    }
                }
            }

        }

        /// <summary>
        /// Получение набора операций для узла типа <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Тип узла</typeparam>
        /// <returns>Набор выполняемых операций</returns>
        public static MyAction<T>[] GetActions<T>()
            where T : Node
        {
            return GetActions(typeof(T)) as MyAction<T>[];
        }

        /// <summary>
        /// Получение набора операций для узла типа <paramref name="type"/>
        /// </summary>
        /// <param name="type">Тип узла</param>
        /// <returns>Набор выполняемых операций</returns>
        public static MyAction[] GetActions(Type type)
        {
            if (!CommonMethods.IsBasedOn(type, typeof(Node)))
                throw new MyException("Параметр 'type' должен быть наследован от" + typeof(Compiler.Nodes.Node).FullName,
                    new MyErrorExceptionType());

            var groups = _actionsGroups.Where(group => group.Value.TargetType == type).ToList();

            if (groups.Count == 1)
                return (groups[0].Value as BaseNodeActions).Actions;

            throw new InvalidActionSignatureException();
        }

        private static Dictionary<Type, BaseNodeActions> _actionsGroups = new Dictionary<Type, BaseNodeActions>();
    }
}
