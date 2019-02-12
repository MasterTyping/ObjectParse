using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateModelList : MonoBehaviour {

    ModelManager ModelManager;
	// Use this for initialization
	void Start () {
        ModelManager = FindObjectOfType<ModelManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
        
        GetComponent<Text>().text = "Model in List : "+ ModelManager.GetModelCount();
    }
}
