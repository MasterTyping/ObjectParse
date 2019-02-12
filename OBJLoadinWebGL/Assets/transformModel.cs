using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class transformModel : MonoBehaviour
{

    public int obj_index; //transfom에 추가된 obj순서
    public bool Selected;
    float speed = 20.0f;
    ModelManager ModelManager;
    private void Start()
    {
        ModelManager = FindObjectOfType<ModelManager>();
        Selected = false;
    }
    private void Update()
    {
        if (Input.GetMouseButton(1) && Selected)
        {
            float temp_x_axis = Input.GetAxis("Mouse X") * 300.0f * Time.deltaTime;
            float temp_y_axis = Input.GetAxis("Mouse Y") * 300.0f * Time.deltaTime;
            transform.Rotate(temp_y_axis, -temp_x_axis, 0, Space.World);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") !=0 && Selected)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * 1.0f;
            transform.localScale += new Vector3(scroll, scroll, scroll);
        }

    }
    private GameObject GetClickedObject()
    {

        RaycastHit hit;
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //마우스 포인트 근처 좌표를 만든다. 

        if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))   //마우스 근처에 오브젝트가 있는지 확인
        {
            //있으면 오브젝트를 저장한다.
            target = hit.collider.gameObject;
            Debug.Log(target.name);
           
        }
        ResetSelect();
        target.GetComponent<transformModel>().Selected = true;
        return target;
    }

    void ResetSelect()
    {
        foreach (GameObject Model in ModelManager.OriginList)
        {
            Model.GetComponent<transformModel>().Selected = false;
        }
    }

    private void OnMouseDown()
    {        
        GetClickedObject();     
        
    }
    private void OnMouseDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 10);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition;
        }       
    }
}

