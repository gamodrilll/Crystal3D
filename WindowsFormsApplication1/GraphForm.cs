using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
        }

        public GraphForm(string[] keys, float[] values, string legend):this()
        {
            chart1.Series[0].Name = legend; 
            chart1.Series[0].Points.DataBindXY(keys, values);
            float min = float.MaxValue, max = float.MinValue; 
            foreach(var i in values)
            {
                min = Math.Min(i, min);
                max = Math.Max(i, max);
            }
            float d = (max - min) / 5;
            chart1.ChartAreas[0].AxisY.Minimum = Math.Round(min - d,3);
            chart1.ChartAreas[0].AxisY.Interval = Math.Round((max - min + 2 * d) / 5,3);
            chart1.ChartAreas[0].AxisY.Title = legend;
            chart1.ChartAreas[0].AxisY.Maximum = Math.Round(max + d,3);
            chart1.ChartAreas[0].AxisX.Title = "Радиус атома, \u00C5";

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
