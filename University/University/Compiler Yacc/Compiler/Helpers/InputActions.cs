using System;
using System.Linq;

using Compiler.Exceptions;
using Compiler.Lex;
using Compiler.Nodes;

namespace Compiler.Helpers
{
    /// <summary>
    /// Выполнение действий, подаваемых на вход 
    /// </summary>
    internal static class InputActions
    {
        /// <summary>
        /// Присвоение переменной <paramref name="lhs"/> значения <paramref name="rhs"/>
        /// </summary>
        public static void Assign(Node lhs, Node rhs, Parser p)
        {
            if (!(rhs is Leaf))
                rhs = new Leaf(rhs.Eval(p), new MyPoint(rhs.Point.GetTokenEnd()));
            
            var reg = p.regs.FirstOrDefault(myReg => myReg.Name == lhs.Name && myReg.Tag != NodeTag.name);
            if (reg != null) //если уже есть элемент с таким именем
            {
                var leftType = lhs.GetChildType(lhs, p);
                var rightType = rhs.GetChildType(rhs, p);
                
                if (leftType != MyType.None)
                    if (leftType != rightType)
                        throw new MyException("Несовпадение типов. ",
                            new MyInvalidDataExceptionType(rhs.Point.GetTokenEnd()));

                var regToRemove = p.regs.FirstOrDefault(myReg => myReg.Name == lhs.Name);
                if (regToRemove == null)
                    throw new VariableWasntDeclaredException(rhs.Point.GetTokenEnd());

                p.regs.Remove(regToRemove);

                rhs = new Leaf(rhs.Eval(p), rhs.Point);
            }

            rhs.Name = lhs.Name;
            p.regs.Add(rhs);
        }

        public static string GetHelp(int maxNameLength)
        {
            return
                @"
________________________________________________________________________________
Имена регистронезависимы, являются строками распознаются первые " + maxNameLength.ToString() + @" символа(-ов),
Выражения могут использовать числа или переменные. 
Переменные могут быть пустыми или содержать деревья выражений. 
Выполнение команды/команд осуществляется по клавише Enter.
СПИСОК КОМАНД:
    > help                       Текущая команда.
    > exit                       Завершает программу, ровно как и ^C.
    > print                      Вывод значений всех имеющихся в памяти переменных на экран.
    > reset                      Удаляет все переменные
    > eval x                     Вычисляет выражение
    > mode val                   Меняет режим работы: val = 0: вычисление, val = 1: генерация кода
    > type expr                  Выводит тип переменной
    > equ ( x expression )       Записывает выражение в переменную x
    > (x add y)                  =  x + y  Сложение чисел или конкатенация строк
    > (x min y)                  =  x - y
    > (x mult y)                 =  x * y
    > (x div y)                  =  x / y
    > neg x                      Отрицание вещественной или логической переменной
    > substr (x y z)             = выделение подстроки из X, начиная с y - того элемента длиной z.
    > (x disj y)                 = дизъюнкция
    > (x noteq y)                = не равно
    > (cond) if (expr expr... expr ) условие If: пока cond = true
    > 

    Логические значения: true, false
    Строки выделяются кавычками " + '\"' + "..." + '\"' +@"
________________________________________________________________________________
";
        }

        /// <summary>
        /// Очистка значений переменных
        /// </summary>
        public static void Clear(Parser p)
        {
            p.regs.Clear();
        }

        /// <summary>
        /// Генерация строкового представления объявленных переменных и их значений
        /// </summary>
        public static string GetRegisters(Parser p)
        {
            string str = "";

            for (int i = 0; i < p.regs.Count; i++)
            {
                if (p.regs[i] != null)
                {
                    str += "\n";
                    str += p.regs[i].Name + " = " + p.regs[i].Eval(p);
                }
            }

            return str;
        }

        /// <summary>
        /// Завершение выполнения
        /// </summary>
        public static void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Вычисление <paramref name="node"/> и выдача результата
        /// </summary>
        /// <param name="node">Узел</param>
        public static string Display(Node node, Parser p)
        {
            MyValue result = node.Eval(p);
            string line = "result: ";
            switch (result.Type)
            {
                case MyType.Double:
                    line += result.Double + " или " + CommonMethods.DoubleToString(result.Double);
                    break;

                case MyType.String:
                    line += result.String;
                    break;

                case MyType.Bool:
                    line += result.Bool ? "true" : "false";
                    break;
            }

            return line;
        }
    }
}
