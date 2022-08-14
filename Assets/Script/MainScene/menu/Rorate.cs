using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class Rorate : MonoBehaviour
{

    private void Start()
    {
        
    }
    IEnumerator Fun(int i)
    {
        yield return new WaitForSeconds(0.1f);
        if (i == 1)
        {
            RotateMenu();
        }
        else
        {
            ControlMenuPosition();
        }
         
    }
    private void OnEnable()
    {
        RotateMenu();
        MsgHandelr.Send(new CReqUserInfo());

    }
    void RotateMenu()
    {
        transform.rotation = Quaternion.Euler(30f, 15f, 0);
        transform.localPosition = new Vector3(-1, 0, -200f);
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = transform.DORotate(new Vector3(0,0,0),4f);
        Tweener m2 = transform.DOLocalMove(new Vector3(-1f, 0, 0), 4f);
        sequence.Append(m2);
        sequence.Join(m1);
      
    }
    void ControlMenuPosition()
    {
        
        Sequence sequence = DOTween.Sequence();
         
    }
}
