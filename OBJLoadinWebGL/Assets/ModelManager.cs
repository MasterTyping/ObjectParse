using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ModelManager : MonoBehaviour {

    //데이터 저장 및 로드를 위한 직렬화된 데이터 필드 모델 데이터를 선언한다.
    [Serializable]
    public struct ModelData
    {
       public string path;
       public string name;
       public float[] Location;
       public float[] Rotation;
       public float[] Scale;
       public int Idx;
       public float[] ColliderSize;
       public bool Origin;
       public string OriginName;
    }
    //obj 파일 파싱데이터를 가지고 있는 origin Model
    //파싱한 데이터를 인스턴싱한 Copyed Model의 리스트를 각각 선언해준다
    [Serializable]
    public struct ModelDataList
    {
        public ModelData[] ModelList;
    }
    public List<ModelData> DataList;
    public List<GameObject> OriginList;
    public ModelDataList ModelList;
    public GameObject Prefab;
    OpenFileDialog Dialog;
    string Savepath;
    string path;
    // Use this for initialization
    void Start() {
        // 각각의 리스트를 초기화 해준다
        DataList = new List<ModelData>();                       
        OriginList = new List<GameObject>();
        Dialog = FindObjectOfType<OpenFileDialog>();
        Savepath = Application.dataPath + "/ModelDataList.bin";
        ModelList = new ModelDataList();
    }

    public ModelData SetModelData(GameObject Model)
    {

       ModelData Temp = new ModelData();
       Temp.Location = new float[3];
       Temp.Rotation = new float[3];
       Temp.Scale = new float[3];
       Temp.ColliderSize = new float[3];
       //모델의 이름과 파일 경로
       Temp.path = Model.GetComponent<Text>().text;
       Temp.name = Model.name;
       //모델의 위치
       Temp.Location[0] = Model.transform.localPosition.x;
       Temp.Location[1] = Model.transform.localPosition.y;
       Temp.Location[2] = Model.transform.localPosition.z;
       //모델의 회전
       Temp.Rotation[0] = Model.transform.rotation.x;
       Temp.Rotation[1] = Model.transform.rotation.y;
       Temp.Rotation[2] = Model.transform.rotation.z;
       //모델의 스케일
       Temp.Scale[0] = Model.transform.localScale.x;
       Temp.Scale[1] = Model.transform.localScale.y;
       Temp.Scale[2] = Model.transform.localScale.z;
       //인덱스
       Temp.Idx = Model.GetComponent<transformModel>().obj_index;
       //콜라이더 스케일
       Temp.ColliderSize[0] = Model.GetComponent<Collider>().transform.localScale.x;
       Temp.ColliderSize[1] = Model.GetComponent<Collider>().transform.localScale.y;
       Temp.ColliderSize[2] = Model.GetComponent<Collider>().transform.localScale.z;

        return Temp;
    }
    public void SaveDataList()
    {
        foreach (GameObject Model in OriginList)
        {
            ModelData Temp = new ModelData();
            Temp.Location = new float[3];
            Temp.Rotation = new float[3];
            Temp.Scale = new float[3];
            Temp.ColliderSize = new float[3];
            //모델의 이름과 파일 경로
            Temp.path = Model.GetComponent<Text>().text;
            Temp.name = Model.name;
            //모델의 위치
            Temp.Location[0] = Model.transform.localPosition.x;
            Temp.Location[1] = Model.transform.localPosition.y;
            Temp.Location[2] = Model.transform.localPosition.z;
            //모델의 회전
            Temp.Rotation[0] = Model.transform.rotation.x;
            Temp.Rotation[1] = Model.transform.rotation.y;
            Temp.Rotation[2] = Model.transform.rotation.z;
            //모델의 스케일
            Temp.Scale[0] = Model.transform.localScale.x;
            Temp.Scale[1] = Model.transform.localScale.y;
            Temp.Scale[2] = Model.transform.localScale.z;
            //인덱스
            Temp.Idx = Model.GetComponent<transformModel>().obj_index;
            //콜라이더 스케일
            Temp.ColliderSize[0] = Model.GetComponent<Collider>().transform.localScale.x;
            Temp.ColliderSize[1] = Model.GetComponent<Collider>().transform.localScale.y;
            Temp.ColliderSize[2] = Model.GetComponent<Collider>().transform.localScale.z;
            if (Temp.name.Contains("Origin"))
            {
                Temp.Origin = true;
                Temp.OriginName = "";
            }
            else
            {
                Temp.Origin = false;
                foreach (var originModel in DataList)
                {
                    if (originModel.path == Temp.path && originModel.name.Contains("Origin"))
                    {
                        Temp.OriginName = originModel.name;
                    }
                }
            }
           
            DataList.Add(Temp);
        }
        
        ModelList.ModelList = DataList.ToArray();       
        string toJson = JsonUtility.ToJson(ModelList,true);
        Debug.Log(toJson);
        File.WriteAllText(Application.dataPath + "/Saves/data.json", toJson);
        
       
    }
    public void LoadData()
    {
        Debug.Log("LoadData");
        string jsonString = File.ReadAllText(Application.dataPath + "/Saves/data.json");
        var data = JsonUtility.FromJson<ModelDataList>(jsonString);
        OriginList.Clear();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Debug.Log("leng" + data.ModelList.Length);
        foreach (var model in data.ModelList)
        {

            if (model.Origin)
            {
                GameObject Temp = OBJLoader.LoadOBJFile(model.path);
                Temp.name = model.name;
                Temp.transform.localPosition = new Vector3(model.Location[0], model.Location[1], model.Location[2]);
                Temp.transform.localRotation = Quaternion.Euler(new Vector3(model.Rotation[0], model.Rotation[1], model.Rotation[2]));
                Temp.transform.localScale = new Vector3(model.Scale[0], model.Scale[1], model.Scale[2]);
                Temp.AddComponent<transformModel>();
                Temp.AddComponent<BoxCollider>();
                Temp.GetComponent<BoxCollider>().size = new Vector3(model.ColliderSize[0], model.ColliderSize[1], model.ColliderSize[2]);
                Temp.AddComponent<Text>();
                Temp.GetComponent<Text>().text = model.path;
                Temp.GetComponent<transformModel>().obj_index = model.Idx;
                OriginList.Add(Temp);
            }
            else
            {
                
                GameObject CopyTemp = Instantiate(GameObject.Find(model.OriginName));
                CopyTemp.name = model.name;
                CopyTemp.transform.localPosition = new Vector3(model.Location[0], model.Location[1], model.Location[2]);
                CopyTemp.transform.localRotation = Quaternion.Euler(new Vector3(model.Rotation[0], model.Rotation[1], model.Rotation[2]));
                CopyTemp.transform.localScale = new Vector3(model.Scale[0], model.Scale[1], model.Scale[2]);
                CopyTemp.GetComponent<BoxCollider>().size = new Vector3(model.ColliderSize[0], model.ColliderSize[1], model.ColliderSize[2]);
                CopyTemp.GetComponent<Text>().text = model.path;
                CopyTemp.GetComponent<transformModel>().obj_index = model.Idx;
                OriginList.Add(CopyTemp);
            }
        }
        Debug.Log("Loaded Time : " + sw.ElapsedMilliseconds + "ms");
        sw.Stop();
    }
    public void LoadOriginObj()
    {

        GameObject Model = OBJLoader.LoadOBJFile(Dialog.path);
        var info = new FileInfo(Dialog.path);
        Model.transform.localPosition = new Vector3(0, 500, -490);
        Model.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        Model.transform.localScale = new Vector3(1f, 1f, 1f);
        Model.AddComponent<transformModel>();
        Model.AddComponent<BoxCollider>();
        Model.GetComponent<BoxCollider>().size = new Vector3(50f, 50f, 50f);
        Model.AddComponent<Text>();
        Model.GetComponent<Text>().text = Dialog.path;
        string ObjectName = "OriginObject_" + OriginList.Count;
        Model.GetComponent<transformModel>().obj_index = OriginList.Count;
        Model.name = ObjectName;
        OriginList.Add(Model);

        Debug.Log("Loaded Object : " + Model.name);
        Debug.Log("Object Size : " + (info.Length / (1024)) / 1024 + "Mb");

    }
    public void CopyToOrigin(string OriginName)
    {
        GameObject Origin = GameObject.Find(OriginName);
        if (GameObject.Find(OriginName))
        {          
            GameObject CopyModel = Instantiate(Origin, Origin.transform.position, Quaternion.identity);
            CopyModel.name = "CopyedObject_" + OriginList.Count;
            CopyModel.GetComponent<transformModel>().obj_index = OriginList.Count;
            OriginList.Add(CopyModel);
        }
        else
        {
            Debug.Log("Can't found OriginObject.");
        }
    }

    public void SetModelViewList()
    {
        foreach (GameObject Model in OriginList)
        {
            GameObject mList = (GameObject)Instantiate(Prefab);
            mList.transform.SetParent(GameObject.Find("ModelList").transform);
            mList.name = "mList_" + Model.GetComponent<transformModel>().obj_index;
            if (mList.transform.Find("Text"))
            {
                mList.transform.Find("Text").GetComponent<Text>().text = Model.name;
            }
            mList.transform.localScale = new Vector3(1, 1, 1);
            mList.transform.localPosition = new Vector3(0, Model.GetComponent<transformModel>().obj_index*45, 0);
        }
    }
    public int GetModelCount()
    {
        return OriginList.Count;
    }
    public void DeleteModel()
    {
        foreach (GameObject Model in OriginList)
        {
            DestroyImmediate(Model);
        }
    }
}
