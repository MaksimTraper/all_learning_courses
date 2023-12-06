using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CukamotoWF
{
    internal class Sugeno
    {
        // получаем на вход массив x в задаваемом диапазоне, 
        // вычисляем y в заданных x
        // вычисляем a_i
        // вычисляем c_i
        // выч альфы

        public List<double> _x;
        public List<double> _y = new List<double>();
        public List<double> _a_i = new List<double>();
        public List<List<double>> _alpha_i = new List<List<double>>();
        public List<List<double>> _c_i = new List<List<double>>();

        public List<double> _diapasone = new List<double>();
        public int _num_sep;
        public double _diap_fun;

        public Sugeno(List<double> _x, int left_side, int right_side, int num_fun, double razmah)
        {
            this._x = _x;
            _diapasone.Add(left_side);
            _diapasone.Add(right_side);
            _num_sep = num_fun;
            _diap_fun = razmah;
        }

        /// <summary>
        /// Вычисление значений функции в точке
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        static public double calc_y_fun(double _x)
        {
            return Math.Log(_x)
                    * Math.Sin(_x / 5) + Math.Cos(_x / 10);
        }

        /// <summary>
        /// Вычисление значений функции в X(i)
        /// </summary>
        public void calc_y()
        {
            for (int i = 0; i < _x.Count; i++)
            {
                _y.Add(calc_y_fun(_x[i]));
            }
            return;
        }

        /// <summary>
        /// Вычислить a(i) для каждой треугольной функции
        /// </summary>
        public void calc_a_i()
        {
            for (int i = 0; i < _x.Count; i++)
            {
                _a_i.Add(_y[i] / _x[i]);
            }
            return;
        }

        /// <summary>
        /// Вычислить альфа(i) для каждого x в каждой функции
        /// </summary>
        public void calc_alpha_i()
        {
            double rast_center = (_diapasone[1] - _diapasone[0])/_num_sep;

            for (int i = 0; i < _num_sep; i++)
            {
                _alpha_i.Add(new List<double>());
                double center_fun = _diapasone[0] + rast_center * (i + 1);
                double left_boundary_func = center_fun - _diap_fun;
                double right_boundary_func = center_fun + _diap_fun;
                for (int j = 0; j < _x.Count; j++)
                {
                    if (_x[j] < left_boundary_func ||
                        _x[j] > right_boundary_func)
                        _alpha_i[i].Add(0);
                    else
                        _alpha_i[i].Add(1 -
                            ((Math.Abs(_x[j] - center_fun)) / _diap_fun));
                }
            }
            return;
        }

        /// <summary>
        /// Вычислить c(i) для каждого x по каждой из функций
        /// </summary>
        public void calc_c_i()
        {
            for (int i = 0; i < _num_sep; i++)
            {
                _c_i.Add(new List<double>());
                for (int j = 0; j < _x.Count; j++)
                {
                    _c_i[i].Add(_a_i[j] * _x[j]);
                }
            }
        }

        /// <summary>
        /// Вычислить C для каждого x
        /// </summary>
        /// <param name="num_x"></param>
        /// <returns></returns>
        public double calc_C(int num_x)
        {

            double chislit = 0;
            double znam = 0;

            for (int i = 0; i < _num_sep; i++)
            {
                chislit += _alpha_i[i][num_x] * _c_i[i][num_x];
                znam += _alpha_i[i][num_x];
            }
            if (znam == 0)
                return 0;
            
            double C = chislit/znam;

            return C;
        }
    }
}
