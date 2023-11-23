using System;

using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Сигнатура операции, выполняемой узлом
    /// </summary>
    /// <typeparam name="T">Тип узла</typeparam>
    internal class MyAction<T> : MyAction
        where T : Node
    {
        /// <param name="argsCount">-1 = any</param>
        internal MyAction(
            NodeTag tag, 
            int argsCount, 
            Func<MyType[], bool> areArgumentsCorrect, 
            Func<Parser, T, MyType> outTypeFunc, 
            Func<Parser, T, MyValue> action,
            Func<Parser, T, string> codeGenerateFunc)  
            : this(tag,
            (args) =>
            {
                if (argsCount == -1)
                    return true;

                return args.Length == argsCount ? areArgumentsCorrect(args) : false;
            }, 
            outTypeFunc, 
            action,
            codeGenerateFunc)
        { }

        internal MyAction(
            NodeTag tag, 
            MyType[] inTypes, 
            MyType outType, 
            Func<Parser, T, MyValue> action, 
            Func<Parser, T, string> codeGenerateFunc) 
            : this(tag, 
            (args) => CommonMethods.AreArraysEqual(args, inTypes), 
            (p, node) => outType, 
            action,
            codeGenerateFunc)
        { }

        private MyAction(
            NodeTag tag,
            Func<MyType[], bool> areArgumentsCorrect,
            Func<Parser, T, MyType> outTypeFunc,
            Func<Parser, T, MyValue> action,
            Func<Parser, T, string> codeGenerateFunc)
            : base(
                tag,
                areArgumentsCorrect,
                (p, node) => outTypeFunc(p, node as T),
                (p, node) => action(p, node as T),
                (p, node) => codeGenerateFunc(p, node as T))
        { }
    }

    /// <summary>
    /// Сигнатура операции, выполняемой узлом
    /// </summary>
    internal abstract class MyAction
    {
        protected MyAction(
            NodeTag tag, 
            Func<MyType[], bool> areArgumentsCorrect,
            Func<Parser, Node, MyType> outTypeFunc, 
            Func<Parser, Node, MyValue> action,
            Func<Parser, Node, string> codeGenerateFunc)
        {
            Tag = tag;
            AreArgumentsCorrect = areArgumentsCorrect;
            OutTypeFunc = outTypeFunc;
            Action = action;
            CodeGenerateFunc = codeGenerateFunc;
        }

        public NodeTag Tag { get; private set; }
        public Func<MyType[], bool> AreArgumentsCorrect { get; private set; }
        public Func<Parser, Node, MyType> OutTypeFunc { get; private set; }
        public Func<Parser, Node, MyValue> Action { get; private set; }
        public Func<Parser, Node, string> CodeGenerateFunc { get; private set; }
    }
}
