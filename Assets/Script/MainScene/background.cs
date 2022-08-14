using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class background : MonoBehaviour
{
    Image image;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
       if (time>6)
        {
            ControlImage(image);
        }
       
        
    }

    public void ControlImage(Graphic graphic)
    {
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = graphic.transform.DOScale(new Vector3(0.6f, 0.6f, 1f), 2f);
        Tweener m2 = graphic.transform.DOScale(new Vector3(1f, 1f, 1f), 2f);
        sequence.Append(m1);
        sequence.AppendInterval(2f);
        sequence.Append(m2);
        time = 0;
    }
}
