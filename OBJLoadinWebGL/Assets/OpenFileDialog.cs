using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenFileDialog : MonoBehaviour, IPointerDownHandler
{

    public string path;
    System.Random random;

    void Start()
    {
        random = new System.Random();

        path = "";  
    }

    public void OpenFile()
    {
#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Load Obj", "", "obj");
#endif

        Application.ExternalEval(
           @"
var function_upload = function() {
    document.removeEventListener('click', function_upload);

    var fileuploader = document.getElementById('fileuploader');
    if (!fileuploader) 
    {
        fileuploader = document.createElement('input');
        fileuploader.setAttribute('style','display:none;');
        fileuploader.setAttribute('type', 'file');
        fileuploader.setAttribute('id', 'fileuploader');
        fileuploader.setAttribute('class', 'focused');
        document.getElementsByTagName('body')[0].appendChild(fileuploader);

        fileuploader.onchange = function(e) {
        var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
				URL.createObjectURL(f);
                SendMessage('" + gameObject.name + @"', 'FileDialogResult', URL.createObjectURL(f)); 
            }
        };
    }

    if (fileuploader.getAttribute('class') == 'focused') {
        fileuploader.setAttribute('class', '');
        fileuploader.click();
    }
}

var fileuploader = document.getElementById('fileuploader');
if(fileuploader)
{
	document.getElementById('fileuploader').disabled = true;
	document.removeEventListener('click', function_upload);
	fileuploader.parentNode.removeChild(fileuploader);
}

document.addEventListener('click', function_upload);
            ");
    

    }
    public void OnPointerDown(PointerEventData eventData)
    {

        Application.ExternalEval(
              @"
var fileuploader = document.getElementById('fileuploader');
if (fileuploader) {
    fileuploader.setAttribute('class', 'focused');
}
            ");

    }
    //public void CopyToOrigin()
    //{
    //    GameObject parent = GameObject.Find("Main Camera");
    //    GameObject CopyModel = Instantiate(Model, Model.transform.position, Quaternion.identity);
    //    CopyModel.name = "CopyedObject_" + ModelList.Count;
    //    ModelList.Add(CopyModel);
    //}
    //public void LoadObj()
    //{
        
    //    Debug.Log(path);
      
    //    string ObjectName = "OriginObject_"+ ModelList.Count;
    //    Model = OBJLoader.LoadOBJFile(path);
    //    var info = new FileInfo(path);
    //    Model.transform.localPosition = new Vector3(0,500,-490);
    //    Model.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    //    Model.transform.localScale = new Vector3(1f, 1f, 1f);
    //    Model.name = ObjectName;
    //    Model.AddComponent<transformModel>();
    //    Model.GetComponent<transformModel>().obj_index = ModelList.Count;
    //    Model.AddComponent<BoxCollider>();
    //    Model.GetComponent<BoxCollider>().size = new Vector3(3f,3f,3f);
    //    Model.AddComponent<Text>();
    //    Model.GetComponent<Text>().text = path;

    //    ModelList.Add(Model);
    //    Debug.Log("Loaded Object : " + Model.name);
    //    Debug.Log("Object Size : " + (info.Length/(1024))/1024+"Mb");
        
    //}
    
    public void FileDialogResult(string fileUrl)
    {
        Debug.Log("FileDialogResult");
        path = fileUrl;
        //StartCoroutine(OBJLoadCoroutine(fileUrl));
    }
    IEnumerator OBJLoadCoroutine(string file_path)
    {
        var www = new WWW(file_path);
        yield return www;

        if (string.IsNullOrEmpty(www.text))
        {
        }
        else
        {          
            path = Path.Combine(Application.streamingAssetsPath,file_path);
            Debug.Log(path);   
        }
    }

}
