using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.IO;
using UnityEngine;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System.Xml.Serialization;

public class Server : MonoBehaviour {

    public static string msg = "";

    public static Compound compToVis = null;
    public static Group groupToVis = null;

    string ByteArrayToString(byte[] val)
    {
        string b = "";
        int len = val.Length;
        for (int i = 0; i < len; i++)
        {
            b += (char)val[i];
        }
        return b;
    }



    public void ReceiveCallback(IAsyncResult AsyncCall)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();


        Socket listener = (Socket)AsyncCall.AsyncState;
        Socket client = listener.EndAccept(AsyncCall);
        using (NetworkStream s = new NetworkStream(client))
        {
            int l = s.ReadByte();
            switch (l)
            {
                case 0:
                    //using (var st = File.OpenRead(@"F:\DIP\Crystal3D\WindowsFormsApplication1\bin\Debug\compound.xml"))
                    using (var st = File.OpenRead(@"compound.xml"))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(Compound));
                        compToVis = (Compound)(ser.Deserialize(st));
                        Debug.Log("f");
                        st.Close();
                    }
                    
                    //File.Delete(@"F:\DIP\Crystal3D\WindowsFormsApplication1\bin\Debug\compound.xml");
                    File.Delete(@"compound.xml");
                    break;
                case 255:
                    //using (var st = File.OpenRead(@"F:\DIP\Crystal3D\WindowsFormsApplication1\bin\Debug\group.xml"))
                    using (var st = File.OpenRead(@"group.xml"))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(Group));
                        groupToVis = (Group)(ser.Deserialize(st));
                        Debug.Log("fG");
                        st.Close();
                    }
                    //File.Delete(@"F:\DIP\Crystal3D\WindowsFormsApplication1\bin\Debug\group.xml");
                    File.Delete(@"group.xml");

                    break;
                default:
                    byte[] buf = new byte[l];
                    s.Read(buf, 0, l);
                    msg = ByteArrayToString(buf);
                    break;
            }
            s.Close();
        }

        client.Close();

        // После того как завершили соединение, говорим ОС что мы готовы принять новое
        listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
    }


    // Use this for initialization
    void Start () {
        IPAddress localAddress = IPAddress.Parse("127.0.0.1");

        Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ipEndpoint = new IPEndPoint(localAddress, 2200);

        listenSocket.Bind(ipEndpoint);

        listenSocket.Listen(1);
        listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
        msg = "Server started";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

public sealed class ClassBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
        {
            Type typeToDeserialize = null;
            assemblyName = Assembly.GetExecutingAssembly().FullName;
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            return typeToDeserialize;
        }
        return null;
    }
}