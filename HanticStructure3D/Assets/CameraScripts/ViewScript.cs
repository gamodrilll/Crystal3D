using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewScript : MonoBehaviour {

    private Camera mainCam;
	// Use this for initialization
	void Start () {
        mainCam = GetComponentInParent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        switch (Server.msg)
        {
            case "XZ":
                mainCam.transform.rotation = Quaternion.Euler(0, 120, 0);
                mainCam.transform.position = new Vector3(-25f, 5f, 20f);
                mainCam.orthographicSize = 8;
                Server.msg = "";
                break;
            case "XY":
                mainCam.transform.rotation = Quaternion.Euler(90, 210, 0);
                mainCam.transform.position = new Vector3(3f, 20f, 4.5f);
                mainCam.orthographicSize = 9;
                Server.msg = "";
                break;
            case "3D":
                mainCam.transform.rotation = Quaternion.Euler(30, -150, 0);
                mainCam.transform.position = new Vector3(23.4f, 28.1f, 38.4f);
                mainCam.orthographicSize = 10;
                Server.msg = "";
                break;
        }
		
	}
}
