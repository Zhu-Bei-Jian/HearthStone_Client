using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Google.Protobuf;
using System;
public class FClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("msgBox").gameObject.SetActive(true);
        MsgHandelr.toName = transform.Find("nick").GetComponent<Text>().text;
        GameObject.FindGameObjectWithTag("To").GetComponent<Text>().text = MsgHandelr.toName;
    }
}
