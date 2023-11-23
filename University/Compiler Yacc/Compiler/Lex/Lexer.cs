using System;
using System.Linq;
using System.Text;

using QUT.Gppg;

using Compiler.Exceptions;
using Compiler.Helpers;
using Compiler.Nodes;

namespace Compiler.Lex
{
    /// <summary>
    /// Лексический анализатор 
    /// </summary>
    internal class Lexer : QUT.Gppg.AbstractScanner<Node, LexLocation>
    {
        public Lexer(StateContainer container)
        {
            _container = container;

            var point = new MyPoint(0, -1);
            container.Point = point;
            _point = point;
        }

        private readonly StateContainer _container;
        private readonly StringBuilder _text = new StringBuilder();

        /// <summary>
        /// Список лексем
        /// </summary>
        public static MyToken[] MyTokens =
        {
                new MyToken("true", Tokens.LITERAL, 
                    (lexer) => lexer.yylval = lexer._container.Parser.MakeLeaf(true)),
                new MyToken("false", Tokens.LITERAL, 
                    (lexer) => lexer.yylval = lexer._container.Parser.MakeLeaf(false)),

                new MyToken("minus", Tokens.MINUS),
                new MyToken("add", Tokens.ADD),
                new MyToken("con", (int)'&'),
                new MyToken("div", Tokens.DIV),
                new MyToken("equ", Tokens.ASSIGN),
                new MyToken("eval", Tokens.EVAL),
                new MyToken("exit", Tokens.EXIT),
                new MyToken("help", Tokens.HELP),
                new MyToken("<=", Tokens.SMALLER),
                new MyToken(">=", Tokens.BIGGER),
                new MyToken("mode", Tokens.MODECHANGE),
                new MyToken("mult", Tokens.MULT),//
                new MyToken("print", Tokens.PRINT),
                new MyToken("reset", Tokens.RESET),
                new MyToken("type", Tokens.GETTYPE),
                new MyToken("if", Tokens.CONDITION),
                new MyToken("^", (int)'^'),
                new MyToken("~", (int)'~'),
                new MyToken("substr", Tokens.SUBSTRING),
                new MyToken("neg", Tokens.NEG),
                new MyToken("disj", Tokens.DISJUNCTION),
                new MyToken("noteq", Tokens.NOTEQUAL)
            };

        private MyPoint _point;

        private LexLocation _singleton = new LexLocation();
        public override LexLocation yylloc
        {
            get { return _singleton; }
        }

        #region StringParse

        /// <summary>
        /// Лексический анализ
        /// </summary>
        /// <returns>Код лексемы</returns>
        public override int yylex()
        {
            char ch = (char)_container.Reader.Read();
            _text.Length = 0;
            if ((int)ch == 65535)
                return OnFinish();

            if (ch == '\n')
                return ReturnEOL();

            if (char.IsWhiteSpace(ch))
                return SkipSpace();

            if (char.IsDigit(ch))
                return ReadLiteral(ch);

            if (ch == '\"')
                return ReadString();

            if (IsFirstCharValid(ch))
                return ReadToken(ch);

            _point.Column++;
            switch (ch)
            {
                case ',': case '(': case ')': case '-': case '+': case '*': case '/': case '^':
                case '&': case '~':
                    return ch;
                case ';':
                    return (int)Tokens.EOL;

                case '<': case '>'://case '<': case '=': case '>':
                    return ReadToken(ch);

                default:
                    yyerror("Illergal character " + ch + " at " + _point.Row.ToString() + " row, " +
                _point.Column.ToString() + " column");
                    return yylex();
            }
        }


        private int SkipSpace()
        {
            _point.Column++;
            while (char.IsWhiteSpace((char)_container.Reader.Peek()))
            {
                int ord = _container.Reader.Read();
                _point.Column++;

                if (ord == (int)'\n')
                    return ReturnEOL();
            } 
            
            return yylex();
        }

        private int ReadLiteral(char ch)
        {
            _text.Append(ch);
            _point.Column++;
            while (char.IsDigit((char)_container.Reader.Peek()))
            {
                _point.Column++;
                _text.Append((char)_container.Reader.Read());
            }

            if ((char)_container.Reader.Peek() == ',')
            {
                _point.Column++;
                _text.Append((char)_container.Reader.Read());

                while (char.IsDigit((char)_container.Reader.Peek()))
                {
                    _point.Column++;
                    _text.Append((char)_container.Reader.Read());
                }
            }

            try
            {
                yylval = _container.Parser.MakeLeaf(double.Parse(_text.ToString()));
                return (int)Tokens.LITERAL;
            }
            catch (FormatException)
            {
                yyerror("Invalid literal at " + _point.Row.ToString() + " row, " + 
                        _point.Column.ToString() + " column");
                return (int)Tokens.error;
            }
        }

        private int ReadString()
        {
            while (true)
            {
                char symbol = (char)_container.Reader.Read();
                _point.Column++;
                if (symbol == '\"')
                {
                    _point.Column++; //кавычка
                    yylval = _container.Parser.MakeLeaf(_text.ToString());
                    return (int)Tokens.LITERAL;
                }

                _text.Append(char.ToLower(symbol));
            }
        }

        private int ReadToken(char ch)
        {
            _text.Append(char.ToLower(ch));
            while (IsCharValid((char)_container.Reader.Peek()))
            {
                _point.Column++;
                _text.Append(char.ToLower((char)_container.Reader.Read()));
            }

            _point.Column++;

            string str = _text.ToString();

            var myToken = Lexer.MyTokens.FirstOrDefault(token => token.Input == str);
            if (myToken != null)
            {
                myToken.AddAction(this);
                return myToken.Token;
            }

            if (_text.Length > _container.MaxNameLength)
                _text.Remove(_container.MaxNameLength, _text.Length - _container.MaxNameLength);
            yylval = _container.Parser.MakeIdLeaf(_text.ToString());
            return (int)Tokens.LETTER;
        }

        private bool IsFirstCharValid(char ch)
        {
            return char.IsLetter(ch) || ch == '<' || ch == '>' || ch == '=';
        }

        private bool IsCharValid(char ch)
        {
            return IsFirstCharValid(ch) || char.IsDigit(ch);
        }

        private int ReturnEOL()
        {
            _point.Row++;
            return (int)Tokens.EOL;
        }

        #endregion

        private int state = -1;
        /// <summary>
        /// Завершение работы
        /// </summary>
        private int OnFinish()
        {
            state++;
            switch (_container.ExitMode)
            {
                case FinishMode.Console:
                    if (state == 1)
                    {
                        _container.Reader = Console.In;
                        _container.Writer = Console.Out;
                        return (int)Tokens.FOO;
                    }

                    return (int)Tokens.EOL;

                case FinishMode.Close:
                    if (state == 1)
                        return (int)Tokens.BREAK;

                    return (int)Tokens.EOL;

                default:
                    throw new UnexpectedEnumValueException();
            }
        }

        /// <summary>
        /// Вывод сообщений об ошибках
        /// </summary>
        public override void yyerror(string format, params object[] args)
        {
            var point = _container.Point.GetTokenEnd();
            _container.Writer.WriteLine(format + " В " + (point.Row + 1).ToString() +
                " строке, " + (point.Column + 1).ToString() + " столбце", args);
        }
    }
}
