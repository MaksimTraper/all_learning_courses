using Compiler.Helpers;

namespace Compiler.Nodes.Actions
{
    /// <summary>
    /// Набор операций, выполняемых последовательным узлом
    /// </summary>
    internal class SequenceActions : BaseNodeActions<Sequence>
    {
        public SequenceActions() : base(MyActions)
        { }

        /// <summary>
        /// Операции, выполняемые последовательным узлом
        /// </summary>
        public static readonly MyAction<Sequence>[] MyActions =
        {
            new MyAction<Sequence>(
                NodeTag.sequence, 
                -1, 
                (args) => true,  
                (p, node) => MyType.None,
                (p, node) =>
                {
                    node.EvalChildren(p);
                    return new MyValue();
                },
                (p, node) =>
                {
                    string code = "";
                    for (int i = 0; i < node.Query.Count; i++)
                    {
                        string currCode = node.Query[i].GenerateCode(p);
                        if (!string.IsNullOrWhiteSpace(currCode))
                        {
                            code += currCode;
                            if (node.Query[i].Tag != NodeTag.condition && node.Query[i].Tag != NodeTag.print)
                                if (code[code.Length - 1] != ';')
                                    code += ";";

                            if (i != node.Query.Count - 1)
                                if (code[code.Length - 1] != '\n')
                                    code += '\n';
                        }
                    }

                    return code;
                }),
        };
    }
}
