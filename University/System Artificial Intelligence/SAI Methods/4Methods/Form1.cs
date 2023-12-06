using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace CukamotoWF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        Chart tChart = new Chart();
        Series series = new Series();
        Series series2 = new Series();

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                tChart.Legends.Add("Legend");
                tChart.Parent = groupBox1;
                tChart.Dock = DockStyle.Fill;
                tChart.ChartAreas.Add(new ChartArea("Tsugeno"));
                series.ChartType = SeriesChartType.Line;
                series.ChartArea = "Tsugeno";
            }
            catch { }

            series.Points.Clear();
            tChart.Series.Clear();

            int count_x = Convert.ToInt32(textBox3.Text);
            int left_side = Convert.ToInt32(textBox1.Text);
            int right_side = Convert.ToInt32(textBox2.Text);

            for (int i = left_side; i < right_side; i++)
            {
                double y = Sugeno.calc_y_fun(i);

                series.Points.AddXY(i, y);
            }

            tChart.Series.Add(series);
            tChart.Series[0].LegendText = "Исходная функция";
            tChart.Series[0].ChartType = SeriesChartType.Line;









            List<double> x = new List<double>();
            Random rnd = new Random();

            double x_v = 0;
            for (int i = 0; i < count_x; i++)
            {
                do
                {
                    x_v = rnd.Next(left_side * 100, right_side * 100) * 0.01;
                }
                while (x_v == 0);
                x.Add(x_v);
            }

            x.Sort();

            int num_fun = Convert.ToInt32(textBox5.Text);
            double razmah = Convert.ToDouble(textBox4.Text);

            Sugeno s = new Sugeno(x, left_side, right_side, num_fun, razmah);

            s.calc_y();
            s.calc_a_i();

            s.calc_alpha_i();
            s.calc_c_i();

            series2.ChartArea = "Tsugeno";
            series2.ChartType = SeriesChartType.Line;
            series2.Points.Clear();





            List<double> C = new List<double>();

            for (int i = 0; i < x.Count; i++)
            {
                C.Add(s.calc_C(i));
                series2.Points.AddXY(x[i], C[i]);

                string text = "";
                if (Math.Round(s._y[i], 10) == Math.Round(C[i], 10))
                {
                    text = "Y = C";
                }
                else
                {
                    text = "Y не совпадает с C";
                }

                listBox1.Items.Add(string.Format("X = " + x[i] + "; Y = " + s._y[i] + "; C = " + C[i] + ";               " + text));
            }
           




            tChart.Series.Add(series2);
            tChart.Series[1].LegendText = "Аппроксимация";

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            tChart.Series.Clear();
            series.Points.Clear();
            series2.Points.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.HorizontalScrollbar = true; // Горизонтальный скролл
            listBox1.ScrollAlwaysVisible = true; // Вертикальный скролл
        }
    }
}
