using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Google.Protobuf;
using System;

public class HuaDong : MonoBehaviour,IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        float nowY = transform.localPosition.y;
        if(nowY>90.0  )
        {
            transform.localPosition+= new Vector3(0, -5, 0);
            return;
        }
        if (nowY < -125)
        {
            transform.localPosition += new Vector3(0, 5, 0);
            return;
        }
        transform.localPosition += new Vector3(0, eventData.delta.y, 0);
        int cnt = transform.parent.GetChild(0).GetChild(0).childCount;
        float percent = (90f - nowY) / 215f;
        //cnt*60*percent+89
        transform.parent.GetChild(0).GetChild(0).localPosition = new Vector3(0, (cnt-5) * 50 * percent,0);
        // 滑动条 90 ~  -125   总高度 215
        // 好友列表 单个info 高度60  总高度 cnt*60
        // 百分比同步
        //
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
