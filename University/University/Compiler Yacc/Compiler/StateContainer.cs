using System.IO;

using Compiler.Helpers;
using Compiler.Lex;

namespace Compiler
{
    /// <summary>
    /// Хранилище общих объектов для синтаксического и лексического анализаторов
    /// </summary>
    public class StateContainer
    {
        internal StateContainer(
            int maxNameLength, 
            TextReader reader, 
            TextWriter writer, 
            MyMode workMode,
            FinishMode exitMode)
        {
            MaxNameLength = maxNameLength;
            Reader = reader;
            Writer = writer;
            WorkMode = workMode;
            ExitMode = exitMode;
        }

        public int MaxNameLength { get; set; }

        public TextReader Reader { get; set; }

        public TextWriter Writer { get; set; }

        public MyMode WorkMode { get;set; }

        public FinishMode ExitMode { get; set; }

        internal Parser Parser { get; set; }

        public MyPoint Point { get; set; } 
    }
}
