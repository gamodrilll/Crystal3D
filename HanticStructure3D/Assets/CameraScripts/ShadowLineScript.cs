using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowLineScript : MonoBehaviour {

    public float differense;
	// Use this for initialization
	void Start () {
		
	}
    public bool flag = true;
    void Update()
    {
        Info.difference = differense;

        if (Camera.main.GetComponent<MouseOrbit>().isChanged)
        {
            flag = true;
            Camera.main.GetComponent<MouseOrbit>().isChanged = false;
            return;
        }
        if (!flag)
            return;
        flag = false;
        foreach (var t in Info.allElements)
            foreach (var e in t.allInstases)
                e.layer = 2;

        GameObject[] allTriangles = GameObject.FindGameObjectsWithTag("triangle");
        foreach ( var t in allTriangles)
        {
            Info.deleteAllChilds(t);
            Info.CreateBordersAroundTriangle(t);
        }

        foreach (var t in Info.allElements)
            foreach (var e in t.allInstases)
                e.layer = 0;
    }

}
