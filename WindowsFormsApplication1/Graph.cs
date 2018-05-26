using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Compound current = null;

        List<float> dists = new List<float>();
        List<float> angles = new List<float>();
        List<float> betas = new List<float>();
        List<string> distsName = new List<string>();
        List<string> anglesName = new List<string>();
        List<string> betasName = new List<string>();

        private void TableRel_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            compoundRelTable.Rows.RemoveAt(e.RowIndex);
        }

        private void TableAbs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            compoundAbsTable.Rows.RemoveAt(e.RowIndex);
        }

        private void TableDist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            distTable.Rows.RemoveAt(e.RowIndex);
            dists.RemoveAt(e.RowIndex);
            distsName.RemoveAt(e.RowIndex);
        }

        private void TableAngl_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            angleTable.Rows.RemoveAt(e.RowIndex);
            angles.RemoveAt(e.RowIndex);
            anglesName.RemoveAt(e.RowIndex);
        }

        private void TableBeta_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            betaTable.Rows.RemoveAt(e.RowIndex);
            betas.RemoveAt(e.RowIndex);
            betasName.RemoveAt(e.RowIndex);
        }

        private void clearTableButton_Click(object sender, EventArgs e)
        {
            TabPage page = tabControl1.SelectedTab;
            DataGridView grid = (DataGridView)page.Controls[0];
            grid.Rows.Clear();
        }

        private void claerAllTableButton_Click(object sender, EventArgs e)
        {
            foreach (var t in tabControl1.TabPages)
            {
                DataGridView grid = (DataGridView)((TabPage)t).Controls[0];
                grid.Rows.Clear();
            }
            dists.Clear();
            angles.Clear();
            betas.Clear();
            distsName.Clear();
            anglesName.Clear();
            betasName.Clear();
        }

        private void graphButton_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage page = tabControl1.SelectedTab;
                DataGridView grid = (DataGridView)page.Controls[0];
                int n = grid.Rows.Count;
                if (n < 2)
                    throw new Exception();
                float[] keys;
                string[] names;
                string name;
                if (tabControl1.SelectedIndex == 2)
                {
                    keys = dists.ToArray();
                    names = distsName.ToArray();
                    name = "Расстояние, \u00C5";
                }
                else
                    if (tabControl1.SelectedIndex == 3)
                {
                    keys = angles.ToArray();
                    names = anglesName.ToArray();
                    name = "Угол, градусы";
                }
                else
                {
                    keys = betas.ToArray();
                    names = betasName.ToArray();
                    name = "Угол \u03C6, градусы";
                }

                float[] keys1 = new float[keys.Length];
                Array.Copy(keys, keys1, keys.Length);

                float[] values = new float[n];
                int j = grid.Columns.Count - 1;
                for (int i = 0; i < n; i++)
                    values[i] = float.Parse(grid[j, i].Value.ToString());

                Array.Sort(keys, values);
                Array.Sort(keys1, names);

                GraphForm graph = new GraphForm(names, values, name);
                graph.Show();
            }
            catch
            {
                MessageBox.Show("Нельзя построить график");
            }
        }
    }
}
