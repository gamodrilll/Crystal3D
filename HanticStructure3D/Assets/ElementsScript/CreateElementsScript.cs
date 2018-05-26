using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

public class ElementType
{
    public string elementName;
    public int valence;
    public MyColor color;
    public List<GameObject> allInstases = new List<GameObject>();

}

public class CreateElementsScript : MonoBehaviour {

    CreateElementsScript():base()
    {
        compounds = new List<Compound>();
    }

    public List<Compound> compounds;
    public GameObject spherePrefab;
    public GameObject colorPanel;
    public GameObject elNamePrefab;
    public GameObject hantic;
    public GameObject elements;
    public GameObject borders;
    public GameObject axePrefab;
    


    public delegate Vector3 ReplicationItem(Vector3 vect);

    public List<ReplicationItem> replications = new List<ReplicationItem>();

    void repCreating(Group g)
    {
        Info.alpha = g.alpha;
        Info.beta = g.beta;
        Info.gamma = g.gamma;
        replications.Clear();

        foreach (var i in g.reps)
            replications.Add((Vector3 vect) => (new Vector3(i.x.x * vect.x + i.x.y * vect.y + i.x.z * vect.z + i.x.c,
                i.y.x * vect.x + i.y.y * vect.y + i.y.z * vect.z + i.y.c,
               i.z.x * vect.x + i.z.y * vect.y + i.z.z * vect.z + i.z.c)));
    }

    List<GameObject> ReplicateItem(GameObject obj,Vector3 locate, bool addHiden, string Name)
    {
        List<GameObject> thisAtoms = new List<GameObject>();
        List<Vector3> list = new List<Vector3>();
        List<int> listInd = new List<int>();
        foreach (var rep in replications)
            list.Add(rep(locate));
        for (int i = 1; i <= replications.Count; i++)
            listInd.Add(i);
        
        list = addAxesElement(list, listInd);
        for(int i=1; i <=list.Count; i++)
        {
            Vector3 el = list[i - 1];
            Info.normalize(ref el);
            if (HaveElement(el, thisAtoms))
                continue;
            Info.Scale(ref el);
            Info.transToUSC(ref el);
            Info.YZSwap(ref el);
            
            if (addHiden == false)
            {
                GameObject NewObj = (GameObject)Instantiate(obj, el, Quaternion.identity);
                NewObj.name = "";
                NewObj.name = Name + " " + listInd[i - 1].ToString();
                NewObj.transform.parent = this.transform;
                thisAtoms.Add(NewObj);
            }
            else
            {
                for (float x = -1; x <= 1; x++)
                    for (float y = -1; y <= 1; y++)
                        for (float z = -1; z <= 1; z++)
                        {
                            Vector3 add = new Vector3(x, y, z);
                            Info.Scale(ref add);
                            Info.transToUSC(ref add);
                            Info.YZSwap(ref add);
                            add += el;
                            GameObject NewObj = (GameObject)Instantiate(obj, add, Quaternion.identity);
                            NewObj.name = "";
                            Vector3 addSc = new Vector3(add.x,add.y,add.z);
                            Info.YZSwap(ref addSc);
                            Info.transToOrt(ref addSc);
                            Info.deScale(ref addSc);
                            if (addSc.x >= -0.001 && addSc.x <= 1.001 && addSc.y >= -0.001
                                    && addSc.y <= 1.001 && addSc.z >= -0.001 && addSc.z <= 1.001)
                                NewObj.SetActive(true);
                            else
                                NewObj.SetActive(false);
                            NewObj.name = Name + " " + listInd[i - 1].ToString();
                            NewObj.transform.parent = this.transform;
                            CoordScript scr = NewObj.GetComponentInChildren<CoordScript>();
                            Vector3 el1 = list[i - 1];
                            Info.normalize(ref el1);
                            scr.x = el1.x + x; scr.y = el1.y+y; scr.z = el1.z+ z;
                            thisAtoms.Add(NewObj);
                 
                        }
            }
        }
        return thisAtoms;
    }

    private List<Vector3> addAxesElement(List<Vector3> list, List<int> ind)
    {
        List<Vector3> nList = new List<Vector3>();
        for (int i = 0; i < list.Count; i++)
            nList.Add(list[i]);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].x == 0)
            {
                nList.Add(new Vector3(1, list[i].y, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].y == 0)
            {
                nList.Add(new Vector3(list[i].x, 1, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].z == 0)
            {
                nList.Add(new Vector3(list[i].x, list[i].y, 1));
                ind.Add(i+1);
            }
            if (list[i].x == 0 && list[i].y == 0)
            {
                nList.Add(new Vector3(1, 1, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].x == 0 && list[i].z == 0)
            {
                nList.Add(new Vector3(1, list[i].y, 1));
                ind.Add(i+1);
            }
            if (list[i].y == 0 && list[i].z == 0)
            {
                nList.Add(new Vector3(list[i].x, 1, 1));
                ind.Add(i+1);
            }
            if (list[i].x == 1)
            {
                nList.Add(new Vector3(0, list[i].y, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].y == 1)
            {
                nList.Add(new Vector3(list[i].x, 0, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].z == 1)
            {
                nList.Add(new Vector3(list[i].x, list[i].y, 0));
                ind.Add(i+1);
            }
            if (list[i].x == 1 && list[i].y == 1)
            {
                nList.Add(new Vector3(0, 0, list[i].z));
                ind.Add(i+1);
            }
            if (list[i].x == 1 && list[i].z == 1)
            {
                nList.Add(new Vector3(0, list[i].y, 0));
                ind.Add(i+1);
            }
            if (list[i].y == 1 && list[i].z == 1)
            {
                nList.Add(new Vector3(list[i].x, 0, 0));
                ind.Add(i+1);
            }
            if (list[i].y == 1 && list[i].z == 1)
            {
                nList.Add(new Vector3(list[i].x, 0, 0));
                ind.Add(i+1);
            }
            if (list[i].x == 0 && list[i].y == 0 && list[i].z == 0)
            {
                nList.Add(new Vector3(1, 1, 1));
                ind.Add(i + 1);
            }
            if (list[i].x == 1 && list[i].y == 1 && list[i].z == 1)
            {
                nList.Add(new Vector3(0, 0, 0));
                ind.Add(i + 1);
            }
        }
        return nList;
    }

    private bool HaveElement(Vector3 el, List<GameObject> list)
    {
       
        foreach (var e in list)
        {
            CoordScript scr = e.GetComponent<CoordScript>();
            if (Math.Abs(scr.x - el.x) < 0.05
                && Math.Abs(scr.y - el.y) < 0.05
                && Math.Abs(scr.z - el.z) < 0.05)
                return true;
        }
       
        return false;
    }

    public void setCenter(Vector3 vec)
    {
        hantic.transform.localPosition = vec;
        elements.transform.localPosition = -vec;
        borders.transform.localPosition = -vec;
    }

    public void setCenter()
    {
        Vector3 vec = new Vector3(0.5f, 0.5f, 0.5f);
        Info.Scale(ref vec);
        Info.transToUSC(ref vec);
        Info.YZSwap(ref vec);
        hantic.transform.localPosition = vec;
        elements.transform.localPosition = -vec;
        borders.transform.localPosition = -vec;
    }



    

    // Use this for initialization
    void Start ()
    {

        Info.scr = this;
        Info.borders = GameObject.FindGameObjectWithTag("Borders");
        Info.logFile.AutoFlush = true;
        Info.sendContinuetoWin();
    }


    // Update is called once per frame
    void Update () {

        if(Server.msg == "One")
        {
            Info.hideAll = true;
            Server.msg = "Reset";
        }
        if (Server.msg == "All")
        {
            Info.hideAll = false;
            Server.msg = "Reset";
        }

        if (Server.compToVis!=null)
        {
            Info.borders.SetActive(true);
            CreateCompound(Server.compToVis);
            Server.compToVis = null;
            Info.sendContinuetoWin();
        }
        if (Server.groupToVis != null)
        {
            colorPanel.SetActive(false);
            Info.deleteAllChilds(borders);
            Info.deleteAllChilds(elements);
            repCreating(Server.groupToVis);
            Server.groupToVis = null;
            Info.sendContinuetoWin();
        }
    }

    internal void CreateCompound(Compound cur)
    {
        Info.deleteAllChilds(elements);
        Info.a = cur.a;
        Info.b = cur.b;
        Info.c = cur.c;
        Info.allElements.Clear();
        Info.hidden = false;

        createBorders();

        foreach (var el in cur.elList)
        {
            ElementType type = new ElementType();
            type.valence = el.valence;
            type.elementName = el.elenemtName;
            type.color = el.color;
            GameObject elementPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            elementPrefab.AddComponent<CoordScript>();
            elementPrefab.AddComponent<DblClickScript>();
            elementPrefab.transform.localScale = new Vector3(el.r, el.r, el.r);
            Renderer rend = elementPrefab.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            Material m = rend.material;
            m.color = new Color(el.color.r*1.0f/255, el.color.g * 1.0f / 255, el.color.b * 1.0f / 255);
            type.allInstases = ReplicateItem(elementPrefab, new Vector3(el.x, el.y, el.z), true, el.elenemtName);
            Info.allElements.Add(type);
            Destroy(elementPrefab);
        }

        colorPanel.SetActive(true);
        Info.deleteAllChilds(colorPanel);
        
        List<Element>  els = new List<Element>(from el in cur.elList
                  where el.valence != 0
                  select el);

        float h = colorPanel.GetComponent<ColorPanelScript>().height = 15 + (els.Count + 1) * 35;

        int i;
        for (i = 0; i<els.Count; i++)
        {
            GameObject curEl = Instantiate(elNamePrefab,new Vector3(50,h - (35*i+20) ),Quaternion.identity, colorPanel.transform);
            curEl.name = "Elem";
            curEl.GetComponentInParent<ElementPanelScript>().elName = els[i].elenemtName;
            curEl.GetComponentInParent<ElementPanelScript>().r = els[i].color.r * 1.0f / 255;
            curEl.GetComponentInParent<ElementPanelScript>().g = els[i].color.g * 1.0f / 255;
            curEl.GetComponentInParent<ElementPanelScript>().b = els[i].color.b * 1.0f / 255;
        }
        Element oxy = cur.elList.Find((el) => (el.valence == 0));
        GameObject curElOx = Instantiate(elNamePrefab, new Vector3(50, 30), Quaternion.identity, colorPanel.transform);
        curElOx.name = "Elem";
        curElOx.GetComponentInParent<ElementPanelScript>().elName = oxy.elenemtName[0].ToString();
        curElOx.GetComponentInParent<ElementPanelScript>().r = oxy.color.r * 1.0f / 255;
        curElOx.GetComponentInParent<ElementPanelScript>().g = oxy.color.g * 1.0f / 255;
        curElOx.GetComponentInParent<ElementPanelScript>().b = oxy.color.b * 1.0f / 255;
        setCenter();
    }

    private void createBorders()
    {
        var children = new List<GameObject>();
        foreach (Transform child in borders.transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));


        for (int i = 0; i < 4; i++)
        {
            Vector3 axeXloc = new Vector3(Info.a / 2, 0, 0) + i%2*(new Vector3(0,Info.b,0)) + i/2*(new Vector3(0, 0, Info.c));
            Info.transToUSC(ref axeXloc);
            Info.YZSwap(ref axeXloc);
            GameObject axeX = (GameObject)Instantiate(axePrefab, axeXloc, Quaternion.identity,borders.transform);
            axeX.transform.localScale = new Vector3(Info.a, 0.1f, 0.1f);
        }
        for (int i = 0; i < 4; i++)
        {
            Vector3 axeYloc = new Vector3(0, Info.b / 2, 0) + i % 2 * (new Vector3(Info.a, 0, 0)) + i / 2 * (new Vector3(0, 0, Info.c));
            Info.transToUSC(ref axeYloc);
            Info.YZSwap(ref axeYloc);
            GameObject axeY = (GameObject)Instantiate(axePrefab, axeYloc, Quaternion.Euler(0, 90 - Info.alpha, 0), borders.transform);
            Vector3 scY = new Vector3(0.1f, Info.b, 0.1f);
            Info.YZSwap(ref scY);
            Info.transToOrt(ref scY);
            axeY.transform.localScale = scY;
        }
        for (int i = 0; i < 4; i++)
        {
            Vector3 axeZloc = new Vector3(0, 0, Info.c / 2) + i % 2 * (new Vector3(Info.a, 0, 0)) + i / 2 * (new Vector3(0, Info.b, 0));
            Info.transToUSC(ref axeZloc);
            Info.YZSwap(ref axeZloc);
            GameObject axeZ = (GameObject)Instantiate(axePrefab, axeZloc, Quaternion.Euler(0, 0, 0), borders.transform);
            Vector3 scZ = new Vector3(0.1f, 0.1f, Info.c);
            Info.transToUSC(ref scZ);
            Info.YZSwap(ref scZ);
            axeZ.transform.localScale = scZ;
        }

    }
}
