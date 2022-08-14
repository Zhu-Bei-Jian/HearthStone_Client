using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class VS : MonoBehaviour
{
    public static VS vsInstance;
    public Image vsImg;
    public Image p0;
    public Image p1;
    public GameObject vsPanel;
    public static string eHero;

    // Start is called before the first frame update
    void Start()
    {
        vsInstance = this;
    }


    public void Move()
    {
        Debug.Log("Move");
        vsPanel.SetActive(true);
        p0.gameObject.SetActive(true);
        p1.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();

        vsImg.transform.DOScale(new Vector3(1, 1, 1), 1);
        Tweener m1 = p0.transform.DOLocalMove(new Vector3(7.8f, -108f, 0f), 1f);
        Tweener m2 = p0.transform.DOScale(new Vector3(0.55f, 0.5f, 1f), 1f);
        sequence.Append(m2);
        sequence.AppendInterval(0.5f);
        sequence.Append(m1);
    
        Sequence sequence1 = DOTween.Sequence();
        Tweener t1 = p1.transform.DOLocalMove(new Vector3(9f, 135f, 0f), 1f);
        Tweener t2 = p1.transform.DOScale(new Vector3(0.55f, 0.45f, 1f), 1f);
        sequence1.Append(t2);
        sequence1.AppendInterval(0.5f);
        sequence1.Append(t1);


        StartCoroutine(SetVsPanelActive());
    }
    
    IEnumerator SetVsPanelActive()
    {
        yield return new WaitForSeconds(2f);
        vsPanel.SetActive(false);
    }
    public void BExit()
    {
        MsgHandelr.Send(new CNoticeExit());
    }
    public void BAddFriend()
    {
  
        GameObject box = GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("operation").Find("addBox").gameObject;
        if (!box.activeSelf)
        {
            box.SetActive(true);
        }
        else
        {
            box.SetActive(false);
        }
    }
    public void CReqAddFriend()
    {
        string nick = GameObject.FindGameObjectWithTag("friendName").transform.Find("Text").GetComponent<Text>().text;
        CReqUpdateFriend msg = new CReqUpdateFriend();
        msg.Nickname = nick;
        msg.Del = false;
        MsgHandelr.Send(msg);

    }

    public void GuanBiChuangKou()
    {
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Wjieshou").gameObject.SetActive(false);
    }

    public static string SShero;
    public void SHero(string hero)
    {
        GameObject.FindGameObjectWithTag(hero).transform.SetAsLastSibling();
        string name = hero;
        if (hero == "mushi")
        {
            name = "圣光牧师";
        }
        if (hero == "fashi")
        {
            name = "吉安娜法师";
        }
        if (hero == "saman")
        {
            name = "萨满";
        }
        if (hero == "zhanshi")
        {
            name = "地狱咆哮";
        }
        if (hero == "deluyi")
        {
            name = "自然守护者";
        }
        SShero = hero;
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("SelectHero").Find("HeroID").GetComponent<Text>().text = name;
    }
    public void GuanBiMsgBox()
    {
        GameObject.FindGameObjectWithTag("msgBox").SetActive(false);
    }

    public void back2(string back)
    {
        for(int i= GameObject.FindGameObjectWithTag("Canvas").transform.childCount - 1; i >= 0; i--)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject.FindGameObjectWithTag("Canvas").transform.Find(back).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find(back).localPosition = Vector3.zero;
         
    }
    public void Haoyoukaiguan()
    {
        if (GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("list").gameObject.activeSelf)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("list").gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("operation").gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("myInfo").gameObject.SetActive(false);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("list").gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("operation").gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("myInfo").gameObject.SetActive(true);

        }
        
    }

    public void menu2Select()
    {
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("SelectHero").localPosition = Vector3.zero;
    }
    public void AcFriend()
    {
        SNoticeAccept msg =new SNoticeAccept();
        msg.Nickname = MsgHandelr.faqiren;
        msg.BName = MsgHandelr.myName;
        MsgHandelr.Send(msg);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("BWindow").gameObject.SetActive(false);
    }

    public void SendChatMsg()
    {
        ChatMsg msg = new ChatMsg();
        msg.From = MsgHandelr.myName;
        msg.To = GameObject.FindGameObjectWithTag("To").GetComponent<Text>().text;
        msg.Content = GameObject.FindGameObjectWithTag("msgText").GetComponent<Text>().text;
    
        GameObject.FindGameObjectWithTag("msgShow").GetComponent<Text>().text += "\n" + msg.From +"   "+ DateTime.Now.ToLocalTime().ToString() + " \n" + msg.Content;
        GameObject.FindGameObjectWithTag("msgText").GetComponent<Text>().text = "";
        MsgHandelr.Send(msg);
    }
}
