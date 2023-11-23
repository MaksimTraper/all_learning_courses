using System.Linq;

using Compiler.C;
using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций, выполняемых листом
    /// </summary>
    internal class LeafActions : BaseNodeActions<Leaf>
    {
        public LeafActions() : base(MyActions)
        { }

        /// <summary>
        /// Операции, выполняемые листом
        /// </summary>
        public static readonly MyAction<Leaf>[] MyActions =
        {
            new MyAction<Leaf>(
                NodeTag.myBreak,
                -1,
                (args) => true,
                (p, node) => MyType.None, 
                (p, node) =>
                {
                    p.OnBreak();
                    return new MyValue();
                },
                (p, node) =>
                {
                    p.OnBreak();
                    return "";
                }), 

            new MyAction<Leaf>(
                NodeTag.foo,
                -1,
                (args) => true,
                (p, node) => MyType.None,
                (p, node) => new MyValue(), 
                (p, node) => ""), 

            new MyAction<Leaf>(
                NodeTag.name, 
                0, 
                (args) => true,
                (p, node) => node.GetOutType(p), 
                (p, node) => p.regs.First(reg => reg.Name == node.Name).Eval(p),
                (p, node) => node.Name),

            new MyAction<Leaf>(
                NodeTag.literal, 
                0, 
                (args) => true,
                (p, node) => node.Value.Type, 
                (p, node) => node.Value,
                (p, node) => node.Value.ToString()),

            //////////////////////////////////////////////////

            new MyAction<Leaf>(
                NodeTag.exit, 
                new MyType[] { }, 
                MyType.None,
                (p, node) =>
                {
                    p.CallExit();
                    return new MyValue();
                },
                (p, node) => Constants.Exit),

            new MyAction<Leaf>(
                NodeTag.help, 
                new MyType[] { }, 
                MyType.None,
                (p, node) =>
                {
                    p.PrintHelp();
                    return new MyValue();
                },
                (p, node) => ""),

            new MyAction<Leaf>(
                NodeTag.print, 
                new MyType[] { },
                MyType.None,
                (p, node) =>
                {
                    p.PrintRegisters();
                    return new MyValue();
                },
                (p, node) =>
                {
                    string code = "";
                    foreach (var reg in p.regs)
                        code += CommonMethods.PrintVariableCode(reg.Name) + '\n';

                    return code;
                }),
            new MyAction<Leaf>(
                NodeTag.clear, 
                new MyType[] { },
                MyType.None,
                (p, node) =>
                {
                    p.ClearRegisters();
                    return new MyValue();
                },
                (p, node) =>
                {
                    string code = "";
                    foreach (var variable in p.regs)
                    {
                        code += variable.Name + " = " + CommonMethods.GetDefaultValueCode(variable.GetChildType(variable, p)) + ";\n";
                    }

                    return code;
                }),
        };
    }
}
