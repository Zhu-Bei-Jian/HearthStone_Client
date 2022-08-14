using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class shuxing : MonoBehaviour
{
    public class Card
    {
        public int id;
        public string name;
        public string rawName;
        public string description;
        public int attack;
        public int hp;
        public Card()
        {
            id = 0;
        }
    }

    public Text UIhp;
    public Text UIattack;
    public Image img;
    public Card cd;

    public static int cardIndex;

    public void SyncUI()
    {
        UIhp.text = cd.hp.ToString();
        UIattack.text = cd.attack.ToString();
      
        img.sprite = Resources.Load<Sprite>("海盗战/" + cd.name);
    }
    public void SyncAttackHP()
    {
        UIhp.text = cd.hp.ToString();
        UIattack.text = cd.attack.ToString();
    }
    
    public void ResetName()
    {
        cd.name = cd.rawName;
        img.sprite = Resources.Load<Sprite>("海盗战/" + cd.rawName);
    }
}
