using System;

using Compiler.Exceptions;
using Compiler.Helpers;

namespace Compiler.Nodes
{
    /// <summary>
    /// Значение, поддерживающее внутреннюю типизцию
    /// </summary> 
    /// <remarks>Типизация статическая, тип устанавливается при первом использовании</remarks>
    internal class MyValue
    {
        internal MyValue()
        {
            Type = MyType.None;
        }

        public MyType Type { get; private set; }
        private double _double;
        private string _string;
        private bool _bool;

        public double Double
        {
            get
            {
                InternalGet(MyType.Double);
                return _double;
            }
            set
            {
                InternalSet(MyType.Double);
                _double = value;
            }
        }

        public string String
        {
            get
            {
                InternalGet(MyType.String);
                return _string;
            }
            set
            {
                InternalSet(MyType.String);
                _string = value;
            }
        }

        public bool Bool
        {
            get
            {
                InternalGet(MyType.Bool);
                return _bool;
            }
            set
            {
                InternalSet(MyType.Bool);
                _bool = value;
            }
        }

        private void InternalGet(MyType type)
        {
            if (Type == MyType.None)
                throw new MyException("Значение не было присвоено", new MyErrorExceptionType());

            if (Type != type)
                throw new TypesMismatchException();
        }

        private void InternalSet(MyType type)
        {
            if (Type == MyType.None)
            {
                Type = type;
                return;
            }

            if (Type != type)
                throw new TypesMismatchException();
        }

        public override string ToString()
        {
            switch (Type)
            {
                case MyType.Bool: return _bool ? "true" : "false";
                case MyType.Double: return CommonMethods.DoubleToString(_double);
                case MyType.String: return '\"' + _string + '\"';

                default:
                    throw new UnexpectedEnumValueException();
            }
        }
    }
}
