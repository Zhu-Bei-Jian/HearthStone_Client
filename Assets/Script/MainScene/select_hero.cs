using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class select_hero : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Text text;

    public void OnPointerClick(PointerEventData eventData)
    {

        image.sprite = eventData.pointerEnter.GetComponent<Image>().sprite;
        switch (eventData.pointerEnter.name)
        {
            case "hero1":
                text.text = "法师";
                break;
            case "hero2":
                text.text = "猎人";
                break;
            case "hero3":
                text.text = "圣骑士";
                break;
            case "hero4":
                text.text = "战士";
                break;
            case "hero5":
                text.text = "德鲁伊";
                break;
            case "hero6":
                text.text = "术士";
                break;
            case "hero7":
                text.text = "萨满";
                break;
            case "hero8":
                text.text = "牧师";
                break;
            case "hero9":
                text.text = "盗贼";
                break;
            default:
                break;

        }
        Debug.Log(VS.vsInstance.p0);
        Debug.Log(VS.vsInstance.p0);
        VS.vsInstance.p0.sprite = image.sprite;
        VS.vsInstance.p1.sprite = image.sprite;
    }
}
