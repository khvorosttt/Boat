using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Boat
{
    public partial class Form1 : Form
    {
        Chart chart;
        double v0 = 0;
        double mu = 0;
        double m = 0;
        double dt = 0;
        double t_end = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            In(ref v0, ref mu, ref m, ref dt, ref t_end);
            double koeff = mu / m;
            int N = (int)(t_end / dt);
            double[] exact_solution = new double[N + 1];
            double[] approximate_solution = new double[N + 1];
            double[] comparison = new double[N + 1];
            double[] t = new double[N + 1];
            Calc(ref exact_solution, ref approximate_solution, ref comparison, ref t, koeff);
            Out(t, exact_solution, approximate_solution, comparison);
            CreateChart();
            DataPoint[] dp=new DataPoint[exact_solution.Length];
            DataPoint[] dp1 = new DataPoint[approximate_solution.Length];
            for(int i=0; i < exact_solution.Length; i++)
            {
                dp[i] = new DataPoint(t[i], exact_solution[i]);
                dp[i].MarkerStyle = MarkerStyle.Circle;
                chart.Series[0].ToolTip = "X = #VALX, Y = #VALY";
                chart.Series[0].Points.Add(dp[i]);
                dp1[i] = new DataPoint(t[i], approximate_solution[i]);
                dp1[i].MarkerStyle = MarkerStyle.Circle;
                chart.Series[1].ToolTip = "X = #VALX, Y = #VALY";
                chart.Series[1].Points.Add(dp1[i]);
            }            
        }

        private void In(ref double v0, ref double mu, ref double m, ref double dt, ref double t_end)
        {
            StreamReader fileIn = new StreamReader("Inlet.in");
            string[] data = fileIn.ReadLine().Trim().Split(' ');
            v0 = double.Parse(data[0]);
            mu = double.Parse(data[1]);
            m = double.Parse(data[2]);
            dt = double.Parse(data[3]);
            t_end = double.Parse(data[4]);
            fileIn.Close();
        }

        private double AnalyticalSolution(double t,double koeff)
        {
            return v0 * Math.Exp(-koeff * t);
        }

        private double NumberSolution(double koeff, double v)
        {
            return v * (1 - koeff * dt);
        }

        private void Calc(ref double[] exact_solution, ref double[] approximate_solution, ref double[] comparison, ref double[] t, double koeff)
        {
            t[0]= 0;
            exact_solution[0] = v0;
            approximate_solution[0] = v0;
            comparison[0] = 0;
            for (int i=1; i < exact_solution.Length; i++)
            {
                t[i] = i * dt;
                exact_solution[i] = AnalyticalSolution(i * dt, koeff);
                approximate_solution[i] = NumberSolution(koeff, approximate_solution[i - 1]);
                comparison[i] = exact_solution[i] - approximate_solution[i];
            }
        }

        private void Out(double[] t, double[] exact_solution, double[] approximate_solution, double[] comparison)
        {
            StreamWriter fileOut = new StreamWriter("Outlet.out");
            for (int i = 0; i < exact_solution.Length; i++)
            {
                fileOut.WriteLine(t[i]+" "+exact_solution[i] + " " + approximate_solution[i] + " " + comparison[i]);
            }
            fileOut.Close();
        }

        private void CreateChart()
        {
            chart = new Chart();
            chart.Parent = this;
            chart.SetBounds(10, 10, ClientSize.Width - 20,
                ClientSize.Height - 20);
            ChartArea area = new ChartArea();
            area.Name = "Speet";
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = t_end;
            area.AxisX.MajorGrid.Interval = 0.5;
            chart.ChartAreas.Add(area);
            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Spline;
            series1.BorderWidth = 1;
            series1.LegendText = "v";
            chart.Series.Add(series1);
            Series series2 = new Series();
            series2.ChartType = SeriesChartType.Spline;
            series2.BorderWidth = 1;
            series2.LegendText = "v_ap";
            chart.Series.Add(series2);
            Legend legend = new Legend();
            chart.Legends.Add(legend);
        }
    }
}
