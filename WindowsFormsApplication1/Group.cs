using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable]
    public class Group
    {
        public string name;
        public float alpha, beta, gamma;
        public List<Replication> reps;

        public Group()
        {
            reps = new List<Replication>();
        }

    }

    [Serializable]
    public class Replication
    {
        public int number;
        public CoordComponent x, y, z;
        public Replication()
        {

        }

        public Replication(CoordComponent x, CoordComponent y, CoordComponent z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override string ToString()
        {
            string s = number.ToString() + "\t x: " + x.ToString() +
                "\t y: " + y.ToString() + "\t z: " + z.ToString();

            return s;
        }
    }

    [Serializable]
    public class CoordComponent
    {
        public float xn, xd, yn, yd, zn, zd, cn, cd;
        public float x { get { return xn / xd; } }
        public float y { get { return yn / yd; } }
        public float z { get { return zn / zd; } }
        public float c { get { return cn / cd; } }
        public CoordComponent()
        {

        }
        public CoordComponent(int xn, int xd, int yn, int yd, int zn, int zd, int cn, int cd)
        {
            this.xn = xn;
            this.xd = xd;
            this.yn = yn;
            this.yd = yd;
            this.zn = zn;
            this.zd = zd;
            this.cn = cn;
            this.cd = cd;

        }

        public override string ToString()
        {
            string s="";
            if (x!=0)
            {
                s += x.ToString()+" x ";
            }
            if (y != 0)
            {
                s += y.ToString() + " y ";
            }
            if (z != 0)
            {
                s += z.ToString() + " z ";
            }
            if (c != 0)
            {
                s += c.ToString("F4");
            }

            return s;
        }

    }
}
