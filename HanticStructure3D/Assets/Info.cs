using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;

public static class Info
{
    public static StreamWriter logFile = new
        System.IO.StreamWriter(@"log.txt", false);
    public static List<ElementType> allElements = new List<ElementType>();
    public static GameObject borders;
    public static float alpha = 120;
    public static float beta = 90;
    public static float gamma = 90;
    public static float a = 9.819f;
    public static float b = 9.819f;
    public static float c = 7.987f;
    public static bool hideAll = true;
    public static bool hidden = false;

    public static float difference;


    public static void deleteAllChilds(GameObject obj)
    {
        for (int c = obj.transform.childCount - 1; c >= 0; c--)
        {
            Transform child = obj.transform.GetChild(c);
            GameObject.Destroy(child.gameObject);
        }
    }

    public static Mesh Triangle(GameObject obj, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
        var mesh = new Mesh
        {
            vertices = new[] { vertex0, vertex1, vertex2 },
            normals = new[] { normal, normal, normal },
            uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) },
            triangles = new[] { 0, 1, 2 },
        };
        return mesh;
    }

    public static void DrawLine(GameObject obj, Vector3 a, Vector3 b)
    {
        LineRenderer lr;
        if (obj.GetComponent<LineRenderer>() == null)
        {
            lr = obj.AddComponent<LineRenderer>();
        }
        else
        {
            lr = obj.GetComponent<LineRenderer>();
            lr.enabled = true;
        }
        lr.material = Info.borders.GetComponent<Material>();
        lr.material.color = Color.black;
        lr.startWidth = 0.04f;
        lr.endWidth = 0.04f;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { a, b });
    }

    public static void DrawDashLine(GameObject obj, Vector3 a, Vector3 b)
    {
        Transform triangle = obj.transform.parent.parent;
        Camera cam = Camera.main;
  
        Vector3 d = (b - a) / 31;

        for (int i = 0; i < 16; i++)
        {
            Vector3 st = a + (2 * i) * d, fin = a + (2 * i + 1) * d;

            
            while (i < 16)
            {
                Vector3 dv = (triangle.localPosition - fin) / 100;
                Ray r = cam.ScreenPointToRay(cam.WorldToScreenPoint(fin+dv));
     
                RaycastHit hit;
                if (!Physics.Raycast(r, out hit) || (Vector3.Distance(hit.point, fin+dv) <difference))
                {
                    i++;
                    if (i<16)
                        fin += 2 * d;
                }
                else
                    break;
            }

            GameObject dash = new GameObject();
            dash.name = "Dash";
            dash.transform.SetParent(obj.transform);
            DrawLine(dash, st, fin);
        }
    }


    public static float distance(Vector3 v1, Vector3 v2)
    {
        float d = Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x)
            + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));

        return d;
    }
    public static CreateElementsScript scr = null;

    /* Получает на вход вектор с координатами 0..1
    * А возвращет новый вектор (0..a,0..b,0..c)
    */
    public static void Scale(ref Vector3 vect)
    {
        vect.Set(vect.x * Info.a, vect.y * Info.b, vect.z * Info.c);
    }

    public static void normalize(ref Vector3 vect)
    {
        vect.x = vect.x < 0 ? vect.x + 1 : vect.x > 1 ? vect.x - 1 : vect.x;
        vect.y = vect.y < 0 ? vect.y + 1 : vect.y > 1 ? vect.y - 1 : vect.y;
        vect.z = vect.z < 0 ? vect.z + 1 : vect.z > 1 ? vect.z - 1 : vect.z;
    }



    public static void transToUSC(ref Vector3 vect)
    {
        float alphaInRad = 2 * Mathf.PI / 360 * alpha;
        vect.x = vect.x + vect.y * Mathf.Cos(alphaInRad);
        vect.y = vect.y * Mathf.Sin(alphaInRad);
    }

    public static void YZSwap(ref Vector3 vect)
    {
        float k = vect.z;
        vect.z = vect.y;
        vect.y = k;
    }

    public static void transToOrt(ref Vector3 vect)
    {
        float alphaInRad = 2 * Mathf.PI / 360 * alpha;
        vect.y = vect.y / Mathf.Sin(alphaInRad);
        vect.x = vect.x - vect.y * Mathf.Cos(alphaInRad);

    }

    internal static GameObject CreateTriangleInParent(Transform transform, MyColor color, Vector3 vector31, Vector3 vector32, Vector3 vector33)
    {
        var triangle = GameObject.CreatePrimitive(PrimitiveType.Plane);
        triangle.tag = "triangle";
        triangle.transform.SetParent(transform);
        triangle.name = "triangle";
        var mesh = triangle.GetComponent<MeshFilter>().mesh = Info.Triangle(triangle, vector31, vector32, vector33);
        triangle.GetComponent<MeshCollider>().sharedMesh = mesh;
        Renderer rend = triangle.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Transparent/Specular"));
        Material m = rend.material;
        m.color = new Color(color.r * 1f / 255, color.g * 1f / 255, color.b * 1f / 255, 155f / 255);

        return triangle;   
    }

    internal static void CreateBordersAroundTriangle(GameObject triangle)
    {
        Mesh mesh = triangle.GetComponent<MeshFilter>().mesh;
        Vector3 vector31 = mesh.vertices[0], 
            vector32 = mesh.vertices[1],
            vector33 = mesh.vertices[2];


        //Line 1
        GameObject line1 = new GameObject();
        line1.transform.SetParent(triangle.transform);
        line1.name = "Line";
        //DrawLine(line1, vector31, vector32);
        DrawDashLine(line1, vector31, vector32);
        var ls = line1.AddComponent<LineScript>();
        ls.a = vector31;
        ls.b = vector32;


        //Line 2
        GameObject line2 = new GameObject();
        line2.transform.SetParent(triangle.transform);
        line2.name = "Line";
        //DrawLine(line2, vector31, vector33 );
        DrawDashLine(line2, vector31, vector33);
        ls = line2.AddComponent<LineScript>();
        ls.a = vector31;
        ls.b = vector33;

        //Line 3
        GameObject line3 = new GameObject();
        line3.transform.SetParent(triangle.transform);
        line3.name = "Line";
        //DrawLine(line3, vector32, vector33 );
        DrawDashLine(line3, vector32, vector33);
        ls = line3.AddComponent<LineScript>();
        ls.a = vector32;
        ls.b = vector33;
   
    }

    public static void deScale(ref Vector3 vect)
    {
        vect.Set(vect.x / Info.a, vect.y / Info.b, vect.z / Info.c);
    }

    public static void sendClickDatatoWin(ClickData c)
    {
        if (File.Exists("data.xml"))
            Thread.Sleep(500);
        Int32 port = 2201;
        TcpClient client = new TcpClient("127.0.0.1", port);
        NetworkStream stream = client.GetStream();
        using (FileStream fstream = new FileStream("data.xml", FileMode.Create))
        {
            (new XmlSerializer(typeof(ClickData))).Serialize(fstream, c);
            fstream.Close();
        }
        stream.WriteByte(0);
        stream.Close();
        client.Close();
    }

    public static void sendContinuetoWin()
    {
        Int32 port = 2201;
        TcpClient client = new TcpClient("127.0.0.1", port);
        NetworkStream stream = client.GetStream();
        stream.WriteByte(111);
        stream.Close();
        client.Close();
    }
}
