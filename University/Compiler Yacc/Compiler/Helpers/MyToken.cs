using System;

using Compiler.Lex;

namespace Compiler.Helpers
{
    /// <summary>
    /// Сигнатура лексемы для лексического анализатора 
    /// </summary>
    internal class MyToken
    {
        public MyToken(
            string input,
            Tokens token,
            Action<Lexer> addAction = null) : this(input, (int)token, addAction)
        { }

        public MyToken(
            string input,
            int token,
            Action<Lexer> addAction = null)
        {
            Input = input;
            Token = token;
            AddAction = addAction ?? ((lexer) => { });
        }

        public string Input { get; private set; }
        public int Token { get; private set; }
        public Action<Lexer> AddAction { get; private set; }
    }
}
