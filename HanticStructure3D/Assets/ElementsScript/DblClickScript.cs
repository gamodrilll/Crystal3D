using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

public class DblClickScript : MonoBehaviour
{

    float doubleClickStart = -100;

    public ElementType thisType;


    void OnMouseDown()
    {
        if (((Time.time - doubleClickStart) < 0.3f))
        {
            OnDoubleClick();
            doubleClickStart = -1;
        }
        else
        {
            doubleClickStart = Time.time;
        }
    }

    static float distance(Vector3 v1, Vector3 v2)
    {
        float d = Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x)
            + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));

        return d;
    }

    static float distance(Vector2 v1, Vector2 v2)
    {
        float d = Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x)
            + (v1.y - v2.y) * (v1.y - v2.y));

        return d;
    }

    void OnDoubleClick()
    {
        

        thisType = Info.allElements.Find((t) => (t.allInstases.Contains(this.gameObject)));
        if (thisType.valence == 0)
        {
            return;
        }
        if (Info.hideAll)
        {
            if (!Info.hidden)
            {
                HideElements();
                Info.hidden = true;
            }
        }
        else
        {
            if (!Info.hidden)
            {
                HideOxygens();
                Info.hidden = true;
            }
            HideCurrentTypeElements(thisType);
        }
        List<GameObject> oxList = new List<GameObject>();
        foreach (var el in from e in Info.allElements
                           where e.valence == 0
                           select e)
            oxList.AddRange(el.allInstases);
        GameObject[] oxygens = oxList.ToArray();
        float[] dists = new float[oxygens.Length];
        for (int i = 0; i < oxygens.Length; i++)
        {
            dists[i] = distance(this.transform.position, oxygens[i].transform.position);
        }
        Array.Sort(dists, oxygens);

        int count = thisType.valence;

        for (int i = 0; i < count; i++)
        {
            oxygens[i].SetActive(true);
        }       
        
        List<Vector3> verticies = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            verticies.Add(getVec(oxygens[i]));
        }

        List<Vector3> allPlanes = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                for (int k = j + 1; k < count; k++)
                {
                    allPlanes.Add(new Vector3(i, j, k));
                }
            }
        }
        List<Vector3> planes = new List<Vector3>();
        foreach (var plane in allPlanes)
        {
            Vector3 normal = Vector3.Cross((verticies[(int)plane.y] - verticies[(int)plane.x]).normalized, (verticies[(int)plane.z] - verticies[(int)plane.x]).normalized);
            bool positive = true;
            bool negative = true;
            for (int i = 0; i < count; i++)
            {
                if (i != (int)plane.x && i != (int)plane.y && i != (int)plane.z)
                {
                    float dot = Vector3.Dot(normal, (verticies[i] - verticies[(int)plane.x]).normalized);
                    positive = positive && (dot >= 0);
                    negative = negative && (dot <= 0);
                }
            }
            if (negative)
                planes.Add(plane);
            if (positive)
                planes.Add(new Vector3(plane.y, plane.x, plane.z));
        }

        

        List<GameObject> triangles = new List<GameObject>();

        foreach (var plane in planes)
        {
            triangles.Add(Info.CreateTriangleInParent(gameObject.transform, thisType.color, verticies[(int)plane.x], verticies[(int)plane.y], verticies[(int)plane.z]));          
        }

        Camera.main.GetComponent<MouseOrbit>().isChanged = true;
        SendBetaClickData();

        /*  )
         #region La 
         {
             for (int i = 0; i < 6; i++)
             {
                 oxygens[i].SetActive(true);
             }
             GameObject[] ox = new GameObject[6];
             for (int i = 0; i <= 5; i++)
             {
                 ox[i] = oxygens[i];
             }
             float[] dist = new float[6];
             for (int i = 0; i <= 5; i++)
                 dist[i] = ox[i].transform.localPosition.y;
             Array.Sort(dist, ox);

             GameObject[] ar1 = new GameObject[3];
             GameObject[] ar2 = new GameObject[3];
             for (int i = 0; i < 3; i++)
             {
                 ar1[i] = ox[i];
                 Vector2 A, B1, B2, B3;
                 A = new Vector2(ox[i].transform.localPosition.x, ox[i].transform.localPosition.z);
                 B1 = new Vector2(ox[3].transform.localPosition.x, ox[3].transform.localPosition.z);
                 B2 = new Vector2(ox[4].transform.localPosition.x, ox[4].transform.localPosition.z);
                 B3 = new Vector2(ox[5].transform.localPosition.x, ox[5].transform.localPosition.z);
                 ar2[i] = distance(A, B1) < distance(A, B2) ? distance(A, B1) < distance(A, B3) ?
                     ox[3] : ox[5] : distance(A, B2) < distance(A, B3) ? ox[4] : ox[5];
             }
             Vector3[] coords = new Vector3[]
             {
                 getVec(ar1[0]),getVec(ar1[1]), getVec(ar2[1]),
                 getVec(ar1[1]),getVec(ar1[2]),getVec(ar2[2]),
                 getVec(ar1[2]),getVec(ar1[0]),getVec(ar2[0]),
                 getVec(ar2[1]),getVec(ar2[2]),getVec(ar2[0])
             };
             Info.DrawLine(this.gameObject, coords);
             SendBetaClickData();
         }
         #endregion
         else
         if ( thisType.elementName.StartsWith("Sc") )
         {
             for (int i = 0; i < 6; i++)
             {
                 oxygens[i].SetActive(true);
             }
             GameObject[] ox = new GameObject[6];
             for (int i = 0; i <= 5; i++)
             {
                 ox[i] = oxygens[i];
             }
             float[] dist = new float[6];
             for (int i = 0; i <= 5; i++)
                 dist[i] = (ox[i].transform.localPosition).magnitude;
             Array.Sort(dist, ox);
             Vector3 vec = getVec(ox[2]) - getVec(ox[1]), vec2 = getVec(ox[3]) - getVec(ox[2]);
             if (vec2.magnitude<vec.magnitude)
             {
                 GameObject temp = ox[1];
                 ox[1] = ox[3];
                 ox[3] = temp;
             }
             vec = getVec(ox[3]) - getVec(ox[2]); vec2 = getVec(ox[4]) - getVec(ox[2]);
             if (vec2.magnitude < vec.magnitude)
             {
                 GameObject temp = ox[4];
                 ox[4] = ox[3];
                 ox[3] = temp;
             }

             GameObject[] ar1 = new GameObject[3];
             Vector3[] coords = new Vector3[]
             {
                 getVec(ox[0]),getVec(ox[1]), getVec(ox[2]),
                 getVec(ox[0]),getVec(ox[3]),getVec(ox[4]),
                 getVec(ox[0]),getVec(ox[4]),
                 getVec(ox[5]),getVec(ox[3]),getVec(ox[2]),
                 getVec(ox[5]),getVec(ox[1]),getVec(ox[4])
             };
             Info.DrawLine(this.gameObject, coords);
             List<Vector3> verticies = new List<Vector3>();
             for (int i = 0; i <= 5; i++)
             {
                 verticies.Add(getVec(ox[i]));
             }

             List<Vector3> allPlanes = new List<Vector3>();
             for (int i = 0; i <= 5; i++)
             {
                 for (int j = i+1; j <= 5; j++)
                 {
                     for (int k = j+1; k <= 5; k++)
                     {
                         allPlanes.Add(new Vector3(i, j, k));
                     }
                 }
             }
             List<Vector3> planes = new List<Vector3>();
             foreach(var plane in allPlanes)
             {
                 Debug.Log(String.Format("NewPlane {0:F0} {1:F0} {2:F0}", plane.x, plane.y, plane.z));
                 Vector3 normal = Vector3.Cross((verticies[(int)plane.y] - verticies[(int)plane.x]).normalized, (verticies[(int)plane.z] - verticies[(int)plane.x]).normalized);
                 bool positive = true;
                 bool negative = true;
                 for (int i = 0; i <= 5; i++)
                 {
                     if (i != (int)plane.x && i != (int)plane.y && i != (int)plane.z)
                     {
                         float dot = Vector3.Dot(normal, (verticies[i] - verticies[(int)plane.x]).normalized);
                         Debug.Log(i);
                         Debug.Log(dot);
                         positive = positive && (dot >= 0);
                         negative = negative && (dot <= 0);
                     }
                 }
                 if (negative)
                 {
                     planes.Add(plane);
                     Debug.Log("-");
                 }
                 if (positive)
                 {
                     planes.Add(new Vector3(plane.y, plane.x, plane.z));
                     Debug.Log("+");
                 }
             }

             foreach (var plane in planes)
             {
                 var triangle = GameObject.CreatePrimitive(PrimitiveType.Plane);
                 triangle.transform.SetParent(gameObject.transform);
                 triangle.name = "triangle";
                 triangle.GetComponent<MeshFilter>().mesh = Info.Triangle(verticies[(int)plane.x], verticies[(int)plane.y], verticies[(int)plane.z]);
                 Renderer rend = triangle.GetComponent<Renderer>();
                 rend.material = new Material(Shader.Find("Transparent/Specular"));
                 Material m = rend.material;
                 m.color = new Color(0, 154f / 255, 0, 155f / 255);
             }

         }
         else
         {
             for (int i = 0; i < 3; i++)
             {
                 oxygens[i].SetActive(true);
             }

             Vector3[] coords = new Vector3[]
             {
                 getVec(oxygens[0]),getVec(oxygens[1]), getVec(oxygens[2]),
                 getVec(oxygens[0])};
             Info.DrawLine(this.gameObject, coords);
         }*/

        if (Info.hideAll)
            Info.scr.setCenter(transform.localPosition);
        else
        {
            List<GameObject> allActive = new List<GameObject>();

            foreach (var t in Info.allElements)
                foreach (var el in t.allInstases)
                    if (el.activeSelf)
                        allActive.Add(el);
            Vector3 average = new Vector3();
            foreach (var v in allActive)
                average += v.transform.localPosition;
            average /= allActive.Count;
            Info.scr.setCenter(average);
        }
        
        
        
    }

    private void HideCurrentTypeElements(ElementType thisType)
    {
        foreach (var el in thisType.allInstases)
            if (el.transform.position != this.transform.position)
                el.SetActive(false);
    }

    private void HideOxygens()
    {
        foreach (var t in from el in Info.allElements
                          where el.valence==0
                          select el)
            foreach (var el in t.allInstases)
                if (el.transform.position != this.transform.position)
                    el.SetActive(false);
        Info.borders.SetActive(false);
    }

    private void SendBetaClickData()
    {
        if (thisType.valence == 6 && (!thisType.elementName.StartsWith("Sc")))
        {
            List<GameObject> activeOxygens = new List<GameObject>();
            foreach(var t in from el in Info.allElements
                             where el.valence == 0
                             select el)
            {
                foreach(var e in t.allInstases)
                {
                    if (e.activeSelf)
                        activeOxygens.Add(e);
                }
            }
            GameObject[] oxygens = activeOxygens.ToArray();
            float[] distances = new float[oxygens.Length];
            for (int i = 0; i < oxygens.Length; i++)
            {
                distances[i] = Vector3.Distance(transform.localPosition, oxygens[i].transform.localPosition);
            }
            Array.Sort(distances, oxygens);
            GameObject a = oxygens[0];
            GameObject b = null;
            for (int i = 1; i < 6; i++)
            {
                if (oxygens[i].transform.position.y == a.transform.position.y)
                    continue;
                if (b == null)
                {
                    b = oxygens[i];
                    continue;
                }
                if ((a.transform.position-b.transform.position).magnitude > (a.transform.position - oxygens[i].transform.position).magnitude)
                {
                    b = oxygens[i];
                }
            }
            CoordScript scr = GetComponent<CoordScript>();
            float z = scr.z;
            ClickData beta = new ClickData(Info.a, Info.b, Info.c, scr.x, scr.y, z);
            beta.name = this.name;
            beta.beta = true;
            scr = a.GetComponent<CoordScript>();
            beta.ang1 = new ClickData(Info.a, Info.b, Info.c, scr.x, scr.y, z);
            scr = b.GetComponent<CoordScript>();
            beta.ang3 = new ClickData(Info.a, Info.b, Info.c, scr.x, scr.y, z);
            Info.sendClickDatatoWin(beta);
        }
    }

    private Vector3 getVec(GameObject gameObject)
    {
        return gameObject.transform.localPosition;
    }

    private void HideElements()
    {
        foreach (var t in Info.allElements)
            foreach (var el in t.allInstases)
                if (el.transform.position != this.transform.position)
                    el.SetActive(false);
        Info.borders.SetActive(false);

    }
}
