using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class ObjectController : MonoBehaviour {


    //file path
    public List<string> obj_path;
    public List<string> mtl_path;
    public List<string> texture_path;

    //split find new 
    public static bool splitByMaterial = false;
    //face of Meshs
    public class OBJFace
    {
        public string materialName;
        public string meshName;
        public int[] indexes;
    }
    //Parse Line to Vector3
    public static Vector3 ParseVectorFromCMPS(string[] cmps)
    {
        float x = float.Parse(cmps[1]);
        float y = float.Parse(cmps[2]);
        if (cmps.Length == 4)
        {
            float z = float.Parse(cmps[3]);
            return new Vector3(x, y, z);
        }
        return new Vector2(x, y);
    }
    //Parse Line to Color
    public static Color ParseColorFromCMPS(string[] cmps, float scalar = 1.0f)
    {
        float Kr = float.Parse(cmps[1]) * scalar;
        float Kg = float.Parse(cmps[2]) * scalar;
        float Kb = float.Parse(cmps[3]) * scalar;
        return new Color(Kr, Kg, Kb);
    }
    //parsing Data
    public class ParsingData
    {
        //vector have normal?
        public bool hasNormals = false;
        //OBJ LISTS
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        //UMESH LISTS
        public List<Vector3> uvertices = new List<Vector3>();
        public List<Vector3> unormals = new List<Vector3>();
        public List<Vector2> uuvs = new List<Vector2>();
        //MESH CONSTRUCTION
        public List<string> materialNames = new List<string>();
        public List<string> objectNames = new List<string>();
        public Dictionary<string, int> hashtable = new Dictionary<string, int>();
        public List<OBJFace> faceList = new List<OBJFace>();
        public string cmaterial = "";
        public string cmesh = "default";
        //CACHE
        public Material[] materialCache = null;
        //Mesh info
        public string meshName = "";
        public FileInfo OBJFileInfo;
        public void CleanData()
        {

        }
    }

    //
    public ParsingData pData;
    // Use this for initialization
    void Start () {
        List<string> obj_path = new List<string>();
        List<string> mtl_path = new List<string>();
        List<string[]> texture_path = new List<string[]>();
        pData = new ParsingData();
    }

    public void AddObjpathfromString(string st)
    {
#if UNITY_EDITOR
        st = EditorUtility.OpenFilePanel("Get path from File.", "", "");
#endif
        obj_path.Add(st);
    }
    public void AddMtlpathfromString(string st)
    {
#if UNITY_EDITOR
        st = EditorUtility.OpenFilePanel("Get path from File.", "", "");
#endif
        mtl_path.Add(st);
    }
    public void GetTexturepath()
    {
        string fn = mtl_path.Last();
        FileInfo mtlFileInfo = new FileInfo(fn);
        string baseFileName = Path.GetFileNameWithoutExtension(fn);
        string mtlFileDirectory = mtlFileInfo.Directory.FullName + Path.DirectorySeparatorChar;
        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        s.Start();
        foreach (string ln in File.ReadAllLines(fn))
        {
            string l = ln.Trim().Replace("  ", " ");
            string[] cmps = l.Split(' ');
            string data = l.Remove(0, l.IndexOf(' ') + 1);
            if (cmps[0] == "newmtl")
            {
                //메시의 이름을 받아온다                               
            }
            if (cmps[0] == "map_Kd")
            {
                //텍스쳐의 경로를 받아서 저장
                string pth = OBJLoader.OBJGetFilePath(data, mtlFileDirectory, baseFileName);
                texture_path.Add(pth);
                Debug.Log(data);
            }
        }
    }
    public void LoadOBJFile()
    {

        Debug.Log("Load Obj File");
        //save this info for later
        string fn = obj_path.Last();
        pData.meshName = Path.GetFileNameWithoutExtension(fn);
        pData.OBJFileInfo = new FileInfo(fn);
        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        s.Start();
        foreach (string ln in File.ReadAllLines(fn))
        {
            if (ln.Length > 0 && ln[0] != '#')
            {
                string l = ln.Trim().Replace("  ", " ");
                string[] cmps = l.Split(' ');
                string data = l.Remove(0, l.IndexOf(' ') + 1);

                if (cmps[0] == "mtllib")
                {
                    //load cache
                    if (fn != null)
                        pData.materialCache = OBJLoader.LoadMTLFile(fn);

                }
                else if ((cmps[0] == "g" || cmps[0] == "o") && splitByMaterial == false)
                {
                    pData.cmesh = data;
                    if (!pData.objectNames.Contains(pData.cmesh))
                    {
                        pData.objectNames.Add(pData.cmesh);
                    }
                }
                else if (cmps[0] == "usemtl")
                {
                    pData.cmaterial = data;
                    if (!pData.materialNames.Contains(pData.cmaterial))
                    {
                        pData.materialNames.Add(pData.cmaterial);
                    }

                    if (splitByMaterial)
                    {
                        if (!pData.objectNames.Contains(pData.cmaterial))
                        {
                            pData.objectNames.Add(pData.cmaterial);
                        }
                    }
                }
                else if (cmps[0] == "v")
                {
                    //VERTEX
                    pData.vertices.Add(ParseVectorFromCMPS(cmps));
                }
                else if (cmps[0] == "vn")
                {
                    //VERTEX NORMAL
                    pData.normals.Add(ParseVectorFromCMPS(cmps));
                }
                else if (cmps[0] == "vt")
                {
                    //VERTEX UV
                    pData.uvs.Add(ParseVectorFromCMPS(cmps));
                }
                else if (cmps[0] == "f")
                {
                    int[] indexes = new int[cmps.Length - 1];
                    for (int i = 1; i < cmps.Length; i++)
                    {
                        string felement = cmps[i];
                        int vertexIndex = -1;
                        int normalIndex = -1;
                        int uvIndex = -1;
                        if (felement.Contains("//"))
                        {
                            //doubleslash, no UVS.
                            string[] elementComps = felement.Split('/');
                            vertexIndex = int.Parse(elementComps[0]) - 1;
                            normalIndex = int.Parse(elementComps[2]) - 1;
                        }
                        else if (felement.Count(x => x == '/') == 2)
                        {
                            //contains everything
                            string[] elementComps = felement.Split('/');
                            vertexIndex = int.Parse(elementComps[0]) - 1;
                            uvIndex = int.Parse(elementComps[1]) - 1;
                            normalIndex = int.Parse(elementComps[2]) - 1;
                        }
                        else if (!felement.Contains("/"))
                        {
                            //just vertex inedx
                            vertexIndex = int.Parse(felement) - 1;
                        }
                        else
                        {
                            //vertex and uv
                            string[] elementComps = felement.Split('/');
                            vertexIndex = int.Parse(elementComps[0]) - 1;
                            uvIndex = int.Parse(elementComps[1]) - 1;
                        }
                        string hashEntry = vertexIndex + "|" + normalIndex + "|" + uvIndex;
                        if (pData.hashtable.ContainsKey(hashEntry))
                        {
                            indexes[i - 1] = pData.hashtable[hashEntry];
                        }
                        else
                        {
                            //create a new hash entry
                            indexes[i - 1] = pData.hashtable.Count;
                            pData.hashtable[hashEntry] = pData.hashtable.Count;
                            pData.uvertices.Add(pData.vertices[vertexIndex]);
                            if (normalIndex < 0 || (normalIndex > (pData.normals.Count - 1)))
                            {
                                pData.unormals.Add(Vector3.zero);
                            }
                            else
                            {
                                pData.hasNormals = true;
                                pData.unormals.Add(pData.normals[normalIndex]);
                            }
                            if (uvIndex < 0 || (uvIndex > (pData.uvs.Count - 1)))
                            {
                                pData.uuvs.Add(Vector2.zero);
                            }
                            else
                            {
                                pData.uuvs.Add(pData.uvs[uvIndex]);
                            }

                        }
                    }
                    if (indexes.Length < 5 && indexes.Length >= 3)
                    {
                        OBJFace f1 = new OBJFace();
                        f1.materialName = pData.cmaterial;
                        f1.indexes = new int[] { indexes[0], indexes[1], indexes[2] };
                        f1.meshName = (splitByMaterial) ? pData.cmaterial : pData.cmesh;
                        pData.faceList.Add(f1);
                        if (indexes.Length > 3)
                        {

                            OBJFace f2 = new OBJFace();
                            f2.materialName = pData.cmaterial;
                            f2.meshName = (splitByMaterial) ? pData.cmaterial : pData.cmesh;
                            f2.indexes = new int[] { indexes[2], indexes[3], indexes[0] };
                            pData.faceList.Add(f2);
                        }
                    }
                }
            }
        }

        if (pData.objectNames.Count == 0)
            pData.objectNames.Add("default");
        s.Stop();
        Debug.Log("Data Parsing Time : " + s.ElapsedMilliseconds);
        
    }
    public void ViewinCamera()
    {
        GameObject Temp = SetObject(pData);
        Temp.transform.SetParent(GameObject.Find("Canvas3D").transform);
        Temp.transform.localScale = new Vector3(10, 10, 10);
        Temp.transform.localPosition = new Vector3(0, 0, 0);
        Temp.AddComponent<transformModel>();
        Temp.AddComponent<BoxCollider>();
        Temp.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
    }
    public GameObject SetObject(ParsingData Data)
    {
        GameObject parentObject = new GameObject(Data.meshName);
        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        s.Start();

        foreach (string obj in Data.objectNames)
        {
            GameObject subObject = new GameObject(obj);
            subObject.transform.parent = parentObject.transform;
            subObject.transform.localScale = new Vector3(-1, 1, 1);
            //Create mesh
            Mesh m = new Mesh();
            m.name = obj;
            //LISTS FOR REORDERING
            List<Vector3> processedVertices = new List<Vector3>();
            List<Vector3> processedNormals = new List<Vector3>();
            List<Vector2> processedUVs = new List<Vector2>();
            List<int[]> processedIndexes = new List<int[]>();
            Dictionary<int, int> remapTable = new Dictionary<int, int>();
            //POPULATE MESH
            List<string> meshMaterialNames = new List<string>();

            OBJFace[] ofaces = Data.faceList.Where(x => x.meshName == obj).ToArray();
            foreach (string mn in Data.materialNames)
            {
                OBJFace[] faces = ofaces.Where(x => x.materialName == mn).ToArray();
                if (faces.Length > 0)
                {
                    int[] indexes = new int[0];
                    foreach (OBJFace f in faces)
                    {
                        int l = indexes.Length;
                        System.Array.Resize(ref indexes, l + f.indexes.Length);
                        System.Array.Copy(f.indexes, 0, indexes, l, f.indexes.Length);
                    }
                    meshMaterialNames.Add(mn);
                    if (m.subMeshCount != meshMaterialNames.Count)
                        m.subMeshCount = meshMaterialNames.Count;

                    for (int i = 0; i < indexes.Length; i++)
                    {
                        int idx = indexes[i];
                        //build remap table
                        if (remapTable.ContainsKey(idx))
                        {
                            //ezpz
                            indexes[i] = remapTable[idx];
                        }
                        else
                        {
                            processedVertices.Add(Data.uvertices[idx]);
                            processedNormals.Add(Data.unormals[idx]);
                            processedUVs.Add(Data.uuvs[idx]);
                            remapTable[idx] = processedVertices.Count - 1;
                            indexes[i] = remapTable[idx];
                        }
                    }

                    processedIndexes.Add(indexes);
                }
                else
                {

                }
            }

            //apply stuff
            m.vertices = processedVertices.ToArray();
            m.normals = processedNormals.ToArray();
            m.uv = processedUVs.ToArray();

            for (int i = 0; i < processedIndexes.Count; i++)
            {
                m.SetTriangles(processedIndexes[i], i);
            }

            if (!Data.hasNormals)
            {
                m.RecalculateNormals();
            }
            m.RecalculateBounds();
            ;

            MeshFilter mf = subObject.AddComponent<MeshFilter>();
            MeshRenderer mr = subObject.AddComponent<MeshRenderer>();

            Material[] processedMaterials = new Material[meshMaterialNames.Count];
            for (int i = 0; i < meshMaterialNames.Count; i++)
            {

                if (Data.materialCache == null)
                {
                    processedMaterials[i] = new Material(Shader.Find("Standard (Specular setup)"));
                }
                else
                {
                    Material mfn = Array.Find(Data.materialCache, x => x.name == meshMaterialNames[i]); ;
                    if (mfn == null)
                    {
                        processedMaterials[i] = new Material(Shader.Find("Standard (Specular setup)"));
                    }
                    else
                    {
                        processedMaterials[i] = mfn;
                    }

                }
                processedMaterials[i].name = meshMaterialNames[i];
            }

            mr.materials = processedMaterials;
            mf.mesh = m;

        }

        s.Stop();
        Debug.Log("Set Object Time : "+s.ElapsedMilliseconds);
        return parentObject;
    }
}

