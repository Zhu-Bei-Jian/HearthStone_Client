using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class arrow : MonoBehaviour
{
    public GameObject ar;


    // Update is called once per frame
    void Update()
    {
        FollowMouseRotate();
    }

    //物体跟随鼠标旋转
    private void FollowMouseRotate()
    {
        //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换  
        Vector3 mouse = Input.mousePosition;
 
        //获取物体坐标，物体坐标是世界坐标，将其转换成屏幕坐标，和鼠标一直  
        Vector3 obj = Camera.main.WorldToScreenPoint(transform.position);
        mouse.z = obj.z = 0;
        //屏幕坐标向量相减，得到指向鼠标点的目标向量，
        Vector3 direction = mouse - obj;

        float angel = Mathf.Atan2(direction.y, direction.x) / Mathf.PI * 180f;

        angel += 180;
        transform.rotation = Quaternion.Euler(0, 0, angel);
        ar.transform.rotation = Quaternion.Euler(0, 0, angel+90);
        
        float len =direction.magnitude;
        //len = gameObject.transform.InverseTransformPoint(mouse).magnitude;
        //Debug.LogFormat("长度={0},向量{1}", len,direction);
        GetComponent<RectTransform>().sizeDelta = new Vector2(len-40, 50f);
        int cnt = (int)(len / 40);
        int need = -gameObject.transform.childCount + cnt-1;
             
        if (need > 0)
        {
            for (int i = 0; i < need; i++)
            {
                GameObject go = Instantiate(ar);
                go.SetActive(true);
                go.transform.SetParent(transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            }
        }
        else
        {
            need = -need;
            if (transform.childCount == 1)
            {
                return;
            }
            for (int i = 0; i < need; i++)
            {
                Destroy(transform.GetChild(gameObject.transform.childCount - 1).gameObject);
            }
        }    
    }

}