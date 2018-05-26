using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net.Sockets;
using System.IO;
using System.Xml.Serialization;

public class ClickScript : MonoBehaviour {

    bool f = false;

    void Update()
    {
        bool n = Input.GetMouseButton(0);
        if (!n)
        {
            f = n;
            return;
        }
        if (f)
            return;
        f = n;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        bool flag = false;
        foreach (var h in hits)
            if (h.transform.gameObject.name != "triangle")
                flag = true;

        if (flag) //&& hit.collider.gameObject.name == "Yify")
        {
            float distance = float.MaxValue;
            GameObject target = null;
            foreach (var h in hits)
                if (h.transform.gameObject.name != "triangle" && h.distance < distance)
                {
                    distance = h.distance;
                    target = h.transform.gameObject;
                }
            ClickData c = new ClickData();
            c.name = target.name;
            CoordScript scr = target.GetComponent<CoordScript>();
            c.a = Info.a;
            c.b = Info.b;
            c.c = Info.c;
            c.x = scr.x;
            c.y = scr.y;
            c.z = scr.z;
            c.beta = false;
            Info.sendClickDatatoWin(c);
        }
    }

}
