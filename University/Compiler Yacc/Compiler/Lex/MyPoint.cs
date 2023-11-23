using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lex
{
    /// <summary>
    /// Положение лексического анализатора 
    /// </summary>
    public class MyPoint
    {
        public MyPoint(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public MyPoint(MyPoint source)
        {
            Row = source.Row;
            Column = source.Column;
        }

        public MyPoint GetTokenEnd()
        {
            if (Column == -1)
                return new MyPoint(_lastRow, LastRowLastColumn);
            
            return new MyPoint(Row, Column);
        }

        public int Column { get; set; }

        private int _row = -1;

        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                LastRowLastColumn = Column - 1;
                _lastRow = _row;
                _row = value;
                Column = -1;
            }
        }

        private int _lastRow;
        public int LastRowLastColumn { get; private set; }
    }
}
