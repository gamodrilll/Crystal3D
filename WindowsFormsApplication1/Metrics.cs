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


        bool distCalc = false;
        ClickData first = null;
        bool anglCalc = false;
        ClickData second = null;

        float calcDistance(ClickData a, ClickData b)
        {
            //float ax = a.x * a.a, ay = a.y * a.b, az = a.z * a.c;
           // float bx = b.x * b.a, by = b.y * b.b, bz = b.z * b.c;
            return (float)Math.Sqrt(a.a*a.a*((b.x - a.x)* (b.x - a.x) + (b.y - a.y)* (b.y - a.y)) 
                + a.c*a.c*(b.z - a.z)*(b.z - a.z) - a.a*a.a*(b.x - a.x) * (b.y - a.y));
        }

        float calcAngle(ClickData a, ClickData b,ClickData c)
        {
            float phi;
            float d1 = calcDistance(a, b), d3 = calcDistance(a, c), d2 = calcDistance(b, c);
            float x = (d1 * d1 + d2 * d2 - d3 * d3);
            phi = x / 2 / d1 / d2;
            return (float)(Math.Acos(phi) / Math.PI * 180);
        }

        private void Serv_onClick(ClickData c)
        {
            if(c.beta)
            {
                float angle = calcAngle(c.ang1,c,c.ang3);
                betaTable.Invoke((Action)delegate ()
                {
                    betaTable.Rows.Add(c.name, angle.ToString("F4"));
                    betas.Add(current.elList[0].r);
                    betasName.Add(current.elList[0].elenemtName.Substring(0, 2));

                });
                return;
            }



            if (distCalc)
            {
                if (first == null)
                {
                    first = c;
                    distButton.Invoke((Action)delegate { distButton.Text = "Select second"; });
                }
                else
                {
                    float dist = calcDistance(first, c);
                    distTable.Invoke((Action)delegate ()
                    {
                        distTable.Rows.Add(first.name, c.name, dist.ToString("F4"));
                        dists.Add(current.elList[0].r);
                        distsName.Add(current.elList[0].elenemtName.Substring(0,2));
                    });
                    first = null;
                    distButton.Invoke((Action)delegate { distButton.Text = "Select first"; });
                }
            }
            if (anglCalc)
            {
                if (first == null)
                {
                    first = c;
                    angButton.Invoke((Action)delegate { angButton.Text = "Select second"; });
                }
                else
                {
                    if (second == null)
                    {
                        second = c;
                        angButton.Invoke((Action)delegate { angButton.Text = "Select third"; });
                    }
                    else
                    {
                        float angle = calcAngle(first,second, c);
                        angleTable.Invoke((Action)delegate ()
                        {
                            angleTable.Rows.Add(first.name, second.name, c.name, angle.ToString("F4"));
                            angles.Add(current.elList[0].r);
                            anglesName.Add(current.elList[0].elenemtName.Substring(0, 2));
                        });
                        first = null;
                        second = null;
                        angButton.Invoke((Action)delegate { angButton.Text = "Select first"; });
                    }
                }
            }

            compoundRelTable.Invoke((Action)delegate () {
                if (compoundRelTable.RowCount == 0 || !(compoundRelTable.Rows[compoundRelTable.RowCount - 1].Cells[0].Value.ToString() == c.name &&
                 compoundRelTable.Rows[compoundRelTable.RowCount - 1].Cells[1].Value.ToString() == c.x.ToString("F4") &&
                 compoundRelTable.Rows[compoundRelTable.RowCount - 1].Cells[2].Value.ToString() == c.y.ToString("F4") &&
                 compoundRelTable.Rows[compoundRelTable.RowCount - 1].Cells[3].Value.ToString() == c.z.ToString("F4")))
                {
                    compoundRelTable.Rows.Add(c.name, c.x.ToString("F4"), c.y.ToString("F4"), c.z.ToString("F4"));
                    compoundAbsTable.Rows.Add(c.name, (c.x * c.a).ToString("F4"), (c.y * c.b).ToString("F4"), (c.z * c.c).ToString("F4"));
                }
            });

        }
 
        private void distButton_Click(object sender, EventArgs e)
        {
            if (distCalc)
                distButton.Text = "Distance";
            else
            {
                distButton.Text = "Select first";
                anglCalc = false;
                angButton.Text = "Angle";
            }
            first = null;
            second = null;
            distCalc = !distCalc;
        }

        private void angButton_Click(object sender, EventArgs e)
        {
            if (anglCalc)
            {
                angButton.Text = "Angle";
            }
            else
            {
                angButton.Text = "Select first";
                distButton.Text = "Distance";
                distCalc = false;
      
            }
            first = null;
            second = null;
            anglCalc = !anglCalc;
        }

    }
}
