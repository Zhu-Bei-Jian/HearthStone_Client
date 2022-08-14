using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Google.Protobuf;
using System;
using System.Threading;

public class Action : MonoBehaviour
{
    public Camera myCamera;

    private void Start()
    {
        cardClick.action = this;
        MsgHandelr.action = this;

    }
    public void DoAttack(GameObject srcCard, GameObject targetCard, bool sync)
    {
        if (sync)
        {
            CReqAct req = new CReqAct();
            req.ActionType = ActionType.Attack;
            try
            {
                if (srcCard.transform.gameObject.name == "hero_0")
                {
                    req.CardId = 0 ^ MsgHandelr.mySeatId;
                }
                else
                {
                    req.CardId = srcCard.GetComponent<shuxing>().cd.id;
                }
                if (targetCard.name == "hero_1")
                {
                    req.TargetId = 1 ^ MsgHandelr.mySeatId;
                }
                else
                {
                    req.TargetId = targetCard.transform.GetComponent<shuxing>().cd.id;
                }
            }
            catch (Exception ex)
            {
                Vector3 pos = srcCard.transform.localPosition;
                pos.z = 0;
                srcCard.transform.localPosition = pos;
                Debug.LogException(ex);
                return;
            }

            MsgHandelr.Send(req);

        }



        Sequence sequence = DOTween.Sequence();
        Tweener m1 = srcCard.transform.DOMove(targetCard.transform.position, 0.5f);
        Vector3 temp = srcCard.transform.localPosition;
        temp.z = 0f;
        Tweener m2 = srcCard.transform.DOLocalMove(temp, 0.5f);
        sequence.Append(m1);
        sequence.Append(m2);
        StartCoroutine(ShakeCamera(myCamera, targetCard));

    }

    IEnumerator ShakeCamera(Camera camera, GameObject card)
    {
        yield return new WaitForSeconds(0.35f);
        camera.GetComponent<shake>().enabled = true;
    }

    public void DoDamage(GameObject srcCard,GameObject targetCard,int value)
    {
        GameObject boom =Instantiate(GameObject.FindGameObjectWithTag("boom"),GameObject.FindGameObjectWithTag("Canvas").transform);
        boom.transform.position = srcCard.transform.position;
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = boom.transform.DOMove(targetCard.transform.position, 1.5f);
        sequence.Append(m1);
        StartCoroutine(DestroyGameObject(boom,1.5f,targetCard,value));
        
    }
    IEnumerator DestroyGameObject(GameObject go,float sec,GameObject x,int value)
    {
        yield return new WaitForSeconds(sec);
        Destroy(go);
        Debug.Log(x.name);
        if(x.name=="hero_0" || x.name == "hero_1")
        {
            x.transform.GetChild(0).gameObject.SetActive(true);
            x.transform.GetChild(0).GetChild(0).GetComponent<Text>().text=value.ToString();
            int armor = int.Parse(x.transform.Find("armor").GetChild(0).GetComponent<Text>().text);
            int hp= int.Parse(x.transform.Find("hp").GetChild(0).GetComponent<Text>().text);
            armor += value;
             
            if (armor <= 0)
            {
                x.transform.Find("armor").gameObject.SetActive(false);
                hp += armor;
                x.transform.Find("hp").GetChild(0).GetComponent<Text>().text = hp.ToString();
            }
            else
            {
                x.transform.Find("armor").GetChild(0).GetComponent<Text>().text = armor.ToString();
            }
            yield return new WaitForSeconds(1f);
            x.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            x.transform.Find("mask").Find("subHp").gameObject.SetActive(true);
            x.transform.Find("mask").Find("subHp").GetChild(0).GetComponent<Text>().text=value.ToString();
            int hp = int.Parse(x.transform.Find("mask").Find("hp").GetChild(0).GetComponent<Text>().text);
            x.transform.Find("mask").Find("hp").GetChild(0).GetComponent<Text>().text=(hp+value).ToString();
            yield return new WaitForSeconds(1f);
            x.transform.Find("mask").Find("subHp").gameObject.SetActive(false);
        }
    }

}
