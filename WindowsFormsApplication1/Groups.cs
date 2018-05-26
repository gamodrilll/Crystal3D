using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApplication1
{
    static class Groups
    {
        public static List<Group> allGroups = new List<Group>();
        static Groups()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Group));
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Groups\", "*.gr");
            foreach (var fileName in files)
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    Group x = (Group)ser.Deserialize(s);
                    allGroups.Add(x);
                    s.Close();
                }
            }
        }
    }
}
