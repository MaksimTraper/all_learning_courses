using System;
using System.Collections.Generic;
using System.IO;

using Compiler.Exceptions;
using Compiler.Helpers;
using Compiler.Lex;
using Compiler.Nodes;

namespace Compiler {
    /// <summary>
    /// �������������� ����������
    /// </summary>
    internal partial class Parser {
        Parser(Lexer s, StateContainer container) : base(s)
        {
            container.Parser = this;
            _container = container;
        }

        private StateContainer _container;

        public List<Node> regs = new List<Node>();
    
        static void Main(string[] args)
        {
            var container = new StateContainer(
                8,
                Console.In,
                Console.Out,
                MyMode.Calculate,
                FinishMode.Close);

            if (args.Length > 0)
                SetUp(args, container);

            Parser parser = new Parser(new Lexer(container), container);

            if (container.Writer == Console.Out)
                container.Writer.WriteLine(@"����������: ������ ������ ���������������� � ������ ����������.
������: 20390.
�����: ������ ������ ��������������

��� ������ �� ��������� ������� �������: ^C
��� ������������ ����������� ��������� ������� �������: help" );

            parser.Parse();
        }

        //��������� ���������� ��������� ������
        #region SetUp

        const string MySource = "-source-";
        const string MyDestination = "-dest-";
        const string MyFinishMode = "-onfin-";
        const string MyWorkMode = "-mode-";
        const string MyConsole = "Console";

        private static void SetUp(string[] args, StateContainer container)
        {
            if (args.Length == 1 && args[0].ToLower() == "help")
            {
                container.Writer.Write(
@"MyParser.
-source-[key] - �������� (���� � ����� ��� console),
-dest-[key] - �������� (���� � ����� ��� console),
-onfin-[key] - �������� ��� �����������: 0 - �������, 1 - ���������� � �������,
-mode-[key] - �����: 0 - ���������� ���������, 1 - ��� �� C");
                InputActions.Exit();
            }

            foreach (string arg in args)
            {
                if (arg.ToLower().StartsWith(MySource))
                {
                    string source = arg.Remove(0, MySource.Length);
                    if (source.ToLower() == MyConsole)
                        container.Reader = Console.In;
                    else
                        TryOpenSourceFile(source, container);
                }

                if (arg.ToLower().StartsWith(MyDestination))
                {
                    string destination = arg.Remove(0, MyDestination.Length);
                    if (destination.ToLower() == MyConsole)
                        container.Writer = Console.Out;
                    else
                    {
                        var stream = new StreamWriter(new FileStream(
                            CreateOutputFilePath(destination), FileMode.Create));
                        stream.AutoFlush = true;
                        container.Writer = stream;
                    }
                }

                if (arg.ToLower().StartsWith(MyFinishMode))
                {
                    string mode = arg.Remove(0, MyFinishMode.Length);
                    if (mode == "0")
                        container.ExitMode = FinishMode.Close;
                    else
                    {
                        if (mode == "1")
                            container.ExitMode = FinishMode.Console;
                        else
                            throw new UnexpectedEnumValueException();
                    }
                }

                if (arg.ToLower().StartsWith(MyWorkMode))
                {
                    string mode = arg.Remove(0, MyWorkMode.Length);
                    if (mode == "0")
                        container.WorkMode = MyMode.Calculate;
                    else
                    {
                        if (mode == "1")
                            container.WorkMode = MyMode.GenerateCode;
                        else
                            throw new UnexpectedEnumValueException();
                    }
                }
            }
        }

        private static void TryOpenSourceFile(string path, StateContainer container)
        {
            if (!File.Exists(path))
                throw new Exception("�������� �� ������");

            container.Reader = new StreamReader(new FileStream(path, FileMode.Open));
        }

        private static string CreateOutputFilePath(string path)
        {
            string[] args = path.Split('.');
            string name = args[0];
            string extension = path.Remove(0, name.Length + 1);

            string str = name + "." + extension;
            int i = 1;
            while (File.Exists(str))
            {
                str = name + "(" + i.ToString() + ")." + extension;
                i++;
            }

            return str;
        }

        #endregion

        //�������� ����� �������� �������
        #region NodesMakers

        public Node MakeSequence(NodeTag tag, Node first) 
        {
            return new Sequence(tag, first, _container.Point.GetTokenEnd());
        }

        public Node MakeSequence(NodeTag tag, Node first, Node second) 
        {
            return new Sequence(tag, first, second, _container.Point.GetTokenEnd());
        }

        public Node MakeTernary(NodeTag tag, Node first, Node second, Node third)
        {
            return new Ternary(tag, first, second, third, _container.Point.GetTokenEnd());
        }

        public Node MakeBinary( NodeTag tag, Node lhs, Node rhs ) 
        {
            return new Binary( tag, lhs, rhs, _container.Point.GetTokenEnd());
        }

        public Node MakeUnary(NodeTag tag, Node child) 
        {
            return new Unary( tag, child, _container.Point.GetTokenEnd());
        }

        public Node MakeIdLeaf(string str) 
        {
            return new Leaf(str, _container.Point.GetTokenEnd());
        }

        public Node MakeLeaf(NodeTag tag) 
        {
            return new Leaf(tag, _container.Point.GetTokenEnd());
        }

        public Node MakeLeaf(bool b)
        {
            return new Leaf(new MyValue() { Bool = b }, _container.Point.GetTokenEnd());
        }

        public Node MakeLeaf(double v) 
        {
            return new Leaf( new MyValue() {Double = v }, _container.Point.GetTokenEnd());
        }

        public Node MakeLeaf(string str) 
        {
            return new Leaf( new MyValue() { String = str }, _container.Point.GetTokenEnd());
        }

        #endregion

        /// <summary>
        /// ���������� ��������� ��� ��������� ���� ��� <paramref name="node"/>
        /// </summary>
        /// <param name="node">���� ������ �������</param>
        private void DoStat(Node node)
        {
            try
            {
                switch (_container.WorkMode)
                {
                    case MyMode.Calculate:
                        node.Eval(this);
                        break;

                    case MyMode.GenerateCode:
                        string code = node.GenerateCode(this);
                        _container.Writer.WriteLine(code);
                        break;

                    default:
                        throw new UnexpectedEnumValueException();
                }
            }
            catch (MyException e)
            {
                //���� ���������� ������� ������������ ������ - ����� ���������,
                //����� - ��������� ������ ���������
                if ((e.Type as MyInvalidDataExceptionType) != null)
                {
                    var exception = e.Type as MyInvalidDataExceptionType;
                    _container.Writer.WriteLine(e.Message + "� " + (exception.Point.Row + 1).ToString() +
                                                " ������, " + exception.Point.Column.ToString() + " �������");
                }
                else
                    throw;
            }
        }

        /// <summary>
        /// ��������� ������ ���������� � �������
        /// </summary>
        public void OnBreak()
        {
            switch (_container.ExitMode)
            {
                case FinishMode.Close:
                    InputActions.Exit();
                    break;

                case FinishMode.Console:
                    _container.Reader = Console.In;
                    _container.Writer = Console.Out;
                    break;

                default:
                    throw new UnexpectedEnumValueException();
            }
        }

        /// <summary>
        /// �������� ����������
        /// </summary>
        /// <param name="lhs">����������</param>
        /// <param name="rhs">�������� ��� ���������</param>
        public void Assign(Node lhs, Node rhs)
        {
            InputActions.Assign(lhs, rhs, this);
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        public void PrintHelp()
        {
            _container.Writer.WriteLine(InputActions.GetHelp(_container.MaxNameLength));
        }

        /// <summary>
        /// ������� ����������
        /// </summary>
        public void ClearRegisters()
        {
            InputActions.Clear(this);
        }

        /// <summary>
        /// ����� ����������
        /// </summary>
        public void PrintRegisters() 
        {
            _container.Writer.WriteLine(InputActions.GetRegisters(this));
            _container.Writer.Flush();
        }

        /// <summary>
        /// ��������� ���� ����
        /// </summary>
        public void GetNodeType(Unary node)
        { 
            var type = node.GetChildType(node.child, this);

            string str = "";

            switch (type)
            {
                case MyType.Bool: str = "Boolean"; break;
                case MyType.Double: str = "Numeric"; break;
                case MyType.String: str = "String"; break;

                default:
                    throw new UnexpectedEnumValueException();
            }

            _container.Writer.WriteLine(str);
        }

        /// <summary>
        /// ���������� ������
        /// </summary>
        public void CallExit() {
            InputActions.Exit();
        }

        /// <summary>
        /// ����� ������ ������
        /// </summary>
        /// <param name="child">��������, ���������� ����� �����</param>
        public void ChangeMode(Node child)
        {
            var value = child.Eval(this);
            if (value.Type != MyType.Double)
                throw new UnexpectedValueTypeException(child.Point);

            if (value.Double == 0)
            {
                _container.WorkMode = MyMode.Calculate;
                ClearRegisters();
            }
            else
            {
                if (value.Double == 1)
                {
                    _container.WorkMode = MyMode.GenerateCode;
                    ClearRegisters();
                }
                else
                    throw new UnexpectedValueException(child.Point, new string[] { "0", "1" });
            }
        }

        /// <summary>
        /// ����� �������� ���� ������ �������
        /// </summary>
        public void Display( Node node )
        {
            try
            {
                _container.Writer.WriteLine(InputActions.Display(node, this));
            }
            catch (CircularEvalException)
            {
                Scanner.yyerror("Eval has circular dependencies");
            }
            catch
            {
                Scanner.yyerror("Invalid expression evaluation");
            }
        }
    }
}

    