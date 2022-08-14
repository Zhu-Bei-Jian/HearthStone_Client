using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class Skill : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence sequence = DOTween.Sequence();
        Tweener m2 = gameObject.transform.DOLocalRotate(new Vector3(0,90f,0),0.5f);
        sequence.Append(m2);
       
        Tweener m3 = gameObject.transform.DOLocalRotate(new Vector3(0, 180f, 0), 0.5f);
        sequence.Append(m3);
        StartCoroutine(setUse(true));
    }
    IEnumerator setUse(bool use)
    {
        yield return new WaitForSeconds(0.5f);
        transform.Find("hadUsed").gameObject.SetActive(use);
        transform.Find("notUse").gameObject.SetActive(!use);
    }
}
