using System.Linq;
using System;
using Compiler.C;
using Compiler.Exceptions;
using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций, выполняемых бинарным узлом
    /// </summary>
    internal class BinaryActions : BaseNodeActions<Binary>
    {
        public BinaryActions() : base(MyActions)
        { }

        /// <summary>
        /// Операции, выполняемые бинарным узлом
        /// </summary>
        public static readonly MyAction<Binary>[] MyActions =
        {
            new MyAction<Binary>(
                NodeTag.assign,
                2,
                (args) => true,
                (p, node) => MyType.None,
                (p, node) =>
                {
                    if (node.rhs.Tag == NodeTag.name)
                        if (p.regs.All(reg => reg.Name != node.rhs.Name))
                            throw new VariableWasntDeclaredException(node.rhs.Point.GetTokenEnd());

                    p.Assign(node.lhs, node.rhs);
                    return new MyValue();
                },
                (p, node) => node.AssignCode(p)),

            new MyAction<Binary>(
                NodeTag.condition,
                2,
                (args) => true,
                (p, node) => MyType.None,
                (p, node) =>
                {
                    node.Condition(node.lhs, node.rhs, p);
                    return new MyValue();
                },
                (p, node) =>
                {
                    string code = Constants.If + " (" + node.lhs.GenerateCode(p) + ")";
                    code += '\n' + "{" + '\n' + node.rhs.GenerateCode(p) + '\n' + "}";
                    return code;
                }),

            //////////////////////////////////////////////////

            new MyAction<Binary>(
                NodeTag.div,
                new [] { MyType.Double, MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = node.lhs.Eval(p).Double / node.rhs.Eval(p).Double },
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node,
                    Constants.Div)),

            new MyAction<Binary>(
                NodeTag.minus,
                new [] { MyType.Double, MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = node.lhs.Eval(p).Double - node.rhs.Eval(p).Double },
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node,
                    Constants.Minus)),

            new MyAction<Binary>(
                NodeTag.plus,
                new [] { MyType.Double, MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = node.lhs.Eval(p).Double + node.rhs.Eval(p).Double},
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node,
                    Constants.Plus)),

            new MyAction<Binary>(
                NodeTag.plus,
                new [] { MyType.String, MyType.String },
                MyType.String,
                (p, node) => new MyValue() { String = node.lhs.Eval(p).String + node.rhs.Eval(p).String },
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node,
                    Constants.Plus)),

            new MyAction<Binary>(
                NodeTag.mul,
                new [] { MyType.Double, MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = node.lhs.Eval(p).Double * node.rhs.Eval(p).Double },
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node,
                    Constants.Mul)),

            new MyAction<Binary>(
                NodeTag.smaller,
                new [] { MyType.Double, MyType.Double },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).Double <= node.rhs.Eval(p).Double },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.Smaller)),

            new MyAction<Binary>(
                NodeTag.smaller,
                new [] { MyType.String, MyType.String },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).String.Length <= node.rhs.Eval(p).String.Length },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.Smaller)),


            new MyAction<Binary>(
                NodeTag.bigger,
                new [] { MyType.Double, MyType.Double },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).Double >= node.rhs.Eval(p).Double },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.Bigger)),

            new MyAction<Binary>(
                NodeTag.bigger,
                new [] { MyType.String, MyType.String },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).String.Length >= node.rhs.Eval(p).String.Length },
                (p, node) => CommonMethods.GenerateArithmeticOperation(p, node, Constants.Bigger)),

            new MyAction<Binary>(
                NodeTag.disjunction,
                new [] { MyType.Bool, MyType.Bool},
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).Bool || node.rhs.Eval(p).Bool },
                (p, node) => 
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.Disjunction)),


            new MyAction<Binary>(
                NodeTag.notequal,
                new [] { MyType.Double, MyType.Double },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).Double != node.rhs.Eval(p).Double },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.NotEqual)),

            new MyAction<Binary>(
                NodeTag.notequal,
                new [] { MyType.String, MyType.String },
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).String != node.rhs.Eval(p).String },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.NotEqual)),

            new MyAction<Binary>(
                NodeTag.notequal,
                new [] { MyType.Bool, MyType.Bool},
                MyType.Bool,
                (p, node) => new MyValue() { Bool = node.lhs.Eval(p).Bool != node.rhs.Eval(p).Bool },
                (p, node) =>
                    CommonMethods.GenerateArithmeticOperation(p, node, Constants.NotEqual)),

            new MyAction<Binary>(
                NodeTag.power,
                new [] { MyType.Double, MyType.Double },
                MyType.Double,
                (p, node) => new MyValue() { Double = Math.Pow(node.lhs.Eval(p).Double, node.rhs.Eval(p).Double) },
                (p, node) => CommonMethods.GenerateMathBibleOperation(p, node, Constants.Power)),
            


        };
    }
}
