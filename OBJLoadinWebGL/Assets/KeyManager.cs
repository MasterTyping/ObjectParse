using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.O))
        {
            GameObject.Find("ObjUpload_Button").GetComponent<Button>().onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftAlt))
        {
            GameObject.Find("CopyModel_Button").GetComponent<Button>().onClick.Invoke();
        }
	}
}
