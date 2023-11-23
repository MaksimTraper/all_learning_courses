using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций, выполняемый тернарным узлом
    /// </summary>
    internal class TernaryActions : BaseNodeActions<Ternary>
    {
        public TernaryActions() : base(MyActions)
        { }

        /// <summary>
        /// Операции, выполняемые тернарным узлом
        /// </summary>
        public static readonly MyAction<Ternary>[] MyActions =
        {
            new MyAction<Ternary>(
                NodeTag.subString,
                new [] { MyType.String, MyType.Double, MyType.Double},
                MyType.String,
                (p, node) => new MyValue() { String = node.GetString(p) },
                (p, node) => node.Name + ".substr(" + node.Second.GenerateCode(p)
                                    + ", " + node.Third.GenerateCode(p) + ")"),
        };
    }
}
