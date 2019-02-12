using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateDebugText : MonoBehaviour {

    ModelManager ModelManager;
	// Use this for initialization
	void Start () {
        ModelManager = FindObjectOfType<ModelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("DebugText"))
        {
            GameObject.Find("DebugText").transform.GetComponent<Text>().text = "Model in List : " + ModelManager.GetModelCount();
        }
	}
}
