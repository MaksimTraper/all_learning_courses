using System;

using Compiler.C;
using Compiler.Exceptions;
using Compiler.Nodes;

namespace Compiler.Helpers
{
    /// <summary>
    /// Набор методов для общего употребления
    /// </summary>
    public static class CommonMethods
    {
        /// <summary>
        /// Возвращает число в виде [+-][0-10)E[+-][0...]
        /// </summary>
        /// <param name="val">Число</param>
        /// <returns>Нормальный вид числа</returns>
        public static string DoubleToString(double val)
        {
            if (val == 0)
                return "0";

            int power = 0;

            while (Math.Abs(val) >= 10)
            {
                power++;
                val /= 10;
            }

            while (Math.Abs(val) < 1)
            {
                power--;
                val *= 10;
            }

            string str = val.ToString();
            if (power != 0)
            {
                str += "E";
                if (power > 0)
                    str += "+";
                str += power.ToString();
            }

            return str;
        }

        /// <summary>
        /// Сравнение массивов на порядок следования элементов и их равенство
        /// </summary>
        internal static bool AreArraysEqual(MyType[] first, MyType[] second)
        {
            if (first.Length != second.Length)
                return false;

            for (int i = 0; i < first.Length; i++)
                if (first[i] != second[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Определяет наследован ли класс <paramref name="type"/> от <paramref name="baseType"/>
        /// </summary>
        /// <param name="type">Проверяемый тип</param>
        /// <param name="baseType">Требуемый базовый тип</param>
        /// <returns></returns>
        /// <remarks>Если <paramref name="type"/> равен <paramref name="baseType"/> вернет false </remarks>
        internal static bool IsBasedOn(Type type, Type baseType)
        {
            if (type == baseType)
                return false;

            var currentType = type;

            while (true)
            {
                if (currentType.BaseType == null)
                    return false;

                if (currentType.BaseType == baseType)
                    return true;

                currentType = currentType.BaseType;
            }
        }

        /// <summary>
        /// Получение строкового представления <paramref name="type"/> для С
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetCType(MyType type)
        {
            switch (type)
            {
                case MyType.Bool:
                    return "bool";

                case MyType.Double:
                    return "double";

                case MyType.String:
                    return "std::string";

                default:
                    throw new UnexpectedEnumValueException();
            }
        }

        /// <summary>
        /// Генератор строковых представлений арифметических операций на С
        /// </summary>
        internal static string GenerateArithmeticOperation(Parser p, Binary node,
            string operation)
        {
            return "(" + node.lhs.GenerateCode(p) + ") " + operation +" (" +
                   node.rhs.GenerateCode(p) + ")";
        }

        /// <summary>
        /// Генератор строковых представлений операций библиотеки Math на С
        /// </summary>
        internal static string GenerateMathBibleOperation(Parser p, Binary node,
            string operation)
        {
            return operation + "(" + node.lhs.GenerateCode(p) + ", " + node.rhs.GenerateCode(p) + ")";
        } 

        /// <summary>
        /// Генератор строковых представлений команд вывода имени переменной и её значения на С
        /// </summary>
        internal static string PrintVariableCode(string name)
        {
            return Constants.Write + "(\"" + name + "\", " + name.Length + "); " +
                    Constants.StreamOut + " << \" is \" + std::to_string(" + name + ");";
        }

        /// <summary>
        /// Стандартные значения типов
        /// </summary>
        internal static string GetDefaultValueCode(MyType type)
        {
            switch (type)
            {
                case MyType.Bool: return "false";
                case MyType.Double: return "0";
                case MyType.String: return "\"\"";
                default:
                    throw new UnexpectedEnumValueException();
            }
        }
    }
}
