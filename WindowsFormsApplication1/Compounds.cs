using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApplication1
{
    static class Compounds
    {
        public static List<Compound> allCompounds = new List<Compound>();
        static Compounds()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Compound));
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Elements\", "*.el");
            foreach (var fileName in files)
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    Compound x = (Compound)ser.Deserialize(s);
                    allCompounds.Add(x);
                    s.Close();
                }
            }
        }

    }
}
