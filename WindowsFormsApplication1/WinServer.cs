using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsFormsApplication1
{
    class WinServer
    {

        public delegate void clickEl(ClickData c);

        public event clickEl onClick;

        public delegate void Cont();

        public event Cont Continue;


        public void ReceiveCallback(IAsyncResult AsyncCall)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();


            Socket listener = (Socket)AsyncCall.AsyncState;
            Socket client = listener.EndAccept(AsyncCall);
            using (NetworkStream s = new NetworkStream(client))
            {
                int l = s.ReadByte();
                if (l != 0)     
                {
                    if (l == 111)
                    {
                        Continue();
                        s.Close();
                        client.Close();
                        listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
                        return;
                    }
                    byte[] buf = new byte[l];
                    s.Read(buf, 0, l);
                    
                }
                else
                {
                    //using (var st = File.OpenRead(@"F:\DIP\HanticStructure3D\data.xml"))
                    using (var st = File.OpenRead(@"data.xml"))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(ClickData));
                        ClickData dat = (ClickData)(ser.Deserialize(st));
                        onClick(dat);
                        st.Close();
                    }
                    //File.Delete(@"F:\DIP\HanticStructure3D\data.xml");
                    File.Delete(@"data.xml");
                }
                s.Close();
            }

            client.Close();

            // После того как завершили соединение, говорим ОС что мы готовы принять новое
            listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
        }


        // Use this for initialization
        public WinServer()
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipEndpoint = new IPEndPoint(localAddress, 2201);

            listenSocket.Bind(ipEndpoint);

            listenSocket.Listen(1);
            listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
        }
    }
}
