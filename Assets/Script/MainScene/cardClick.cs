using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Google.Protobuf;
using System;

public class cardClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IDragHandler,IInitializePotentialDragHandler,IEndDragHandler,IPointerDownHandler,IPointerUpHandler
{
 
    public int orignalIndex;
    public Vector3 pos;
    public bool placed;
    public GameObject jiantou;
    public GameObject canvas;
    public GameObject p0SuiCong,p1SuiCong;
    public bool IsHero;

    public static Action action;
     
  
    public void OnDrag(PointerEventData eventData)
    {
        if (placed||IsHero)
        {
            return;
        }
        transform.localPosition += new Vector3(eventData.delta.x/ 1.5411f, eventData.delta.y/ 1.8091f, 0);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("拖拽结束");
        if (placed || IsHero)
        {
            return;
        }
        //仍在手牌区 ，复原位置
        if (transform.localPosition.y<110 && transform.localPosition.y>-10 && transform.localPosition.x>-300 && transform.localPosition.x < 300)
        {
            gameObject.transform.localPosition = pos;
        }
        else
        {      
            PlaceCard(0);          
        }
        
    }

    //将手牌里 随从置入战场
    public void PlaceCard(int opSeatId)
    {
        Debug.LogFormat("placecard  opSeatId ={0}",opSeatId);
        transform.localScale = new Vector3(1, 1, 1);
        switch (opSeatId)
        {
            case 0:
                gameObject.transform.SetParent(p0SuiCong.transform);
                CReqAct msg = new CReqAct();
                msg.ActionType =  ActionType.Place;
                msg.CardId = transform.GetComponent<shuxing>().cd.id;
                MsgHandelr.Send(msg);
                break;
            case 1:
                Debug.Log("敌方随从置入战场");
                
                gameObject.transform.SetParent(p1SuiCong.transform);
                break;
            default:
                break;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Find("mask").GetComponent<Mask>().enabled = true;

        transform.Find("mask").GetChild(1).gameObject.SetActive(true);
        //transform.Find("mask").GetComponent<Image>().enabled = true;
        transform.Find("mask").Find("attack").gameObject.SetActive(true);
        transform.Find("mask").Find("hp").gameObject.SetActive(true);
        placed = true;
        GetComponent<shuxing>().SyncAttackHP();
        GetComponent<shuxing>().ResetName();
    }
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (placed || IsHero)
        {
            return;
        }
        //Debug.Log("开始拖动");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("enter");
        if (placed || IsHero)
        {

        }
        else
        {
            //鼠标进入图片区域 显示 随从详情
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 100f, 0f);
            gameObject.transform.localScale = new Vector3(2f, 2f, 1f);
            int count = transform.parent.childCount - 1;
            transform.SetSiblingIndex(count);
        }
        
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exit");
        if (placed || IsHero)
        {

        }
        else
        {
            //鼠标离开图片区域，复原为简略显示
            gameObject.transform.localPosition = pos;
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            transform.SetSiblingIndex(orignalIndex);
        }
         
    }



    // Start is called before the first frame update
    void Start()
    {
        //placed = false;
        orignalIndex = transform.GetSiblingIndex();
        //Debug.LogFormat("POS inti {0}", transform.localPosition);
        if (pos == Vector3.zero)
        {
            pos = transform.localPosition;
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        if (placed || IsHero)
        {
            //鼠标选中随从，按下 
            //将随从抬起 至 空中
            Sequence sequence = DOTween.Sequence();
            
            Vector3 temp = transform.localPosition;
            temp.z = -100f;
            Tweener m2 = gameObject.transform.DOLocalMove(temp, 0.5f);
            sequence.Append(m2);

            jiantou.transform.position = transform.position;
            jiantou.SetActive(true);//显示出箭头
            //Vector3 ms = Input.mousePosition;
            //ms.y -= 20;
            //Debug.Log(ms);
            //jiantou.transform.localPosition = Camera.main.ScreenToWorldPoint(ms);
             
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if (placed || IsHero)
        {
            
            jiantou.SetActive(false);
           

            GameObject card = GetOverUI(canvas);
            if (card == null){
                Vector3 pos = transform.localPosition;
                pos.z = 0;
                transform.localPosition = pos;
                return;

            }
             
            action.DoAttack(gameObject,card,true);
                             
        }
    }

    

    
    
    public GameObject GetOverUI(GameObject canvas)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);
        if (results.Count != 0)
        {
            return results[0].gameObject;
        }

        return null;
    }


}
