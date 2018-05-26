using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResetScript : MonoBehaviour {



    private void ShowAll()
    {
        Info.hidden = false;
        List<GameObject> allElements = new List<GameObject>();
        foreach (var t in Info.allElements)
            allElements.AddRange(t.allInstases);
        foreach (var el in allElements)
        {
            Vector3 loc = new Vector3(el.transform.localPosition.x, el.transform.localPosition.y,
                el.transform.localPosition.z);
            Info.YZSwap(ref loc);
            Info.transToOrt(ref loc);
            Info.deScale(ref loc);
            if (loc.x >= -0.001 && loc.x <= 1.001 && loc.y >= -0.001
                && loc.y <= 1.001 && loc.z >= -0.001 && loc.z <= 1.001)
                el.SetActive(true);
            else
                el.SetActive(false);
        }
        Info.borders.SetActive(true);

    }


    // Update is called once per frame
    void Update() {
        if (Server.msg == "Reset")
        {
            ShowAll();
            #region Hide All polyhedra
            foreach (var t in from e in Info.allElements
                              where e.valence != 0
                              select e)
                foreach (var el in t.allInstases)
                {
                    Info.deleteAllChilds(el);
                }

            #endregion
            Info.scr.setCenter();
            if (Server.msg == "Reset")
                Server.msg = "";
        }
    }
}
