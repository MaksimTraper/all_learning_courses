using Compiler.Exceptions;
using Compiler.Helpers;
using System;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций, выполняемых унарным узлом
    /// </summary>
    internal class UnaryActions : BaseNodeActions<Unary>
    {
        public UnaryActions() : base(MyActions)
        { }

        /// <summary>
        /// Операции, выполняемые унарным узлом
        /// </summary>
        public static readonly MyAction<Unary>[] MyActions =
        {
            new MyAction<Unary>(
                NodeTag.display, 
                1, 
                (args) => true,
                (p, node) => MyType.None, 
                (p, node) =>
                {
                    p.Display(node.child);
                    return new MyValue(); 
                },
                (p, node) =>
                {
                    if (string.IsNullOrWhiteSpace(node.child.Name))
                        throw new MyException("Некорректное значение. Должно быть имя переменной.",
                            new MyInvalidDataExceptionType(node.child.Point.GetTokenEnd()));

                    return CommonMethods.PrintVariableCode(node.child.Name);
                }),

            new MyAction<Unary>(
                NodeTag.getType,
                1,
                (args) => true,
                (p, node) => MyType.None,
                (p, node) =>
                {
                    p.GetNodeType(node);
                    return new MyValue();
                },
                (p, node) => ""),

            //////////////////////////////////////////////////

            new MyAction<Unary>(
                NodeTag.changeMode,
                new [] { MyType.Double },
                MyType.None,
                (p, node) =>
                {
                    p.ChangeMode(node.child);
                    return new MyValue();
                },
                (p, node) =>
                {
                    p.ChangeMode(node.child);
                    return "";
                }), 

            new MyAction<Unary>(
                NodeTag.negate, 
                new [] { MyType.Double }, 
                MyType.Double,
                (p, node) => new MyValue() { Double = -(node.child.Eval(p).Double) },
                (p, node) => "-(" + node.child.GenerateCode(p) + ")"),

            new MyAction<Unary>(
                NodeTag.negate, 
                new [] { MyType.Bool },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = !(node.child.Eval(p).Bool) },
                (p, node) => "!(" + node.child.GenerateCode(p) + ")"),

            new MyAction<Unary>(
                NodeTag.sqrt,
                new [] { MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = Math.Sqrt(node.child.Eval(p).Double) },
                (p, node) => "~(" + node.child.GenerateCode(p) + ")"),

        };
    }
}
