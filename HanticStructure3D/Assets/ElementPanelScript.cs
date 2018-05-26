using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanelScript : MonoBehaviour {

    public string elName;
    public float r;
    public float g;
    public float b;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        GetComponentInChildren<Text>().text = elName;
        Image img = GetComponentsInChildren<Image>()[1];
        img.color = new Color(r, g, b);
    }
}
