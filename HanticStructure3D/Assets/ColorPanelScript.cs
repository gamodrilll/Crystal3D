using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPanelScript : MonoBehaviour {

    public float height;

	// Use this for initialization
	void Start () {
        //height = GetComponent<RectTransform>().rect.height;
    }
	
	// Update is called once per frame
	void Update () {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width, height);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, height / 2 + 5);

    }
}
