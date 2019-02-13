using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyController : MonoBehaviour {


    public GameObject TxtListPrefab;
    public GameObject ModelListPrefab;
    ModelLoader Loader;
    // Use this for initialization
    void Start () {
        Loader = FindObjectOfType<ModelLoader>();
        foreach (Texture txt in Loader.MatSources)
        {
            GameObject tList = Instantiate(TxtListPrefab);
            tList.transform.localPosition = new Vector3(0, 0, 0);
            tList.transform.localScale = new Vector3(1, 1, 1);
            tList.transform.GetComponent<Text>().text = txt.name;
        }
    }
	

}
