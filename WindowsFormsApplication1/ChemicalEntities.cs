using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{

    [Serializable]
    public class MyColor
    {
        public int r, g, b;
        public MyColor()
        {

        }
    }

    [System.Serializable]
    public class Compound
    {
        public float a;
        public float b;
        public float c;
        public string name;
        public string group;
        public List<Element> elList;
        public Compound()
        {
            elList = new List<Element>();
        }
    }

    [System.Serializable]
    public class Element
    {
        public string elenemtName;
        public MyColor color = new MyColor();
        public int valence;
        public float r;
        public float xn, yn, zn, xd, yd, zd;
        public float x { get { return xn / xd; } }
        public float y { get { return yn / yd; } }
        public float z { get { return zn / zd; } }
    }

    [Serializable]
    public class ClickData
    {
        public string name;
        public float a, b, c, x, y, z;
        public bool beta;
        public ClickData ang1, ang3;
        public ClickData()
        {


        }
        public ClickData(float a, float b, float c, float x, float y, float z)
        {
            this.a = a;
            this.b = b;
            this.c = b;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class ClickDataArrive
    {
        public ClickData c = null;
        public event Action arrive;

        public void OnArrive(ClickData c)
        {
            this.c = c;
            arrive();
        }
    }
}
