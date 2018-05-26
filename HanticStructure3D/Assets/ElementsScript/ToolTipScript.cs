using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipScript : MonoBehaviour {

    public Text toolTip;
    public GameObject toolTipP;

    // Use this for initialization
    void Start () {
        toolTipP.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits =  Physics.RaycastAll(ray);
        bool flag = false;
        foreach (var h in hits)
            if (h.transform.gameObject.name != "triangle")
                flag = true;

        if (flag) //&& hit.collider.gameObject.name == "Yify")
        {
            float distance = float.MaxValue;
            GameObject target = null;
            foreach(var h in hits)
                if (h.transform.gameObject.name != "triangle" && h.distance<distance)
                {
                    distance = h.distance;
                    target = h.transform.gameObject;
                }
           

            string s = target.name + " ";
            Vector3 vect = new Vector3(target.transform.localPosition.x,
           target.transform.localPosition.y, target.transform.localPosition.z);
            Info.YZSwap(ref vect);
            Info.transToOrt(ref vect);
            s += " x/a: " + (vect.x / Info.a).ToString("F3") + " \n y/b: "
            + (vect.y / Info.b).ToString("F3") + " z/c: " + (vect.z / Info.c).ToString("F3");
            toolTip.text = s;
            toolTipP.SetActive(true);
            toolTipP.transform.Translate(Input.mousePosition - toolTipP.transform.position + new Vector3(-80, 25, 0));
        }
        else
        {
            toolTipP.SetActive(false);
            toolTip.text = "";
        }
    }
}
