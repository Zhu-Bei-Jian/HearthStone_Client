using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class login : MonoBehaviour
{
    public void BLogin()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Login").localPosition = new Vector3(100, 0, 0);
        canvas.transform.Find("SelectHero").localPosition=new Vector3(0, 0, 0);

    }
}
