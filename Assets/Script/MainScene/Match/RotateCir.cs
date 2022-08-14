using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCir : MonoBehaviour
{


    // Update is called once per frame

    public bool flag;   
    void Update()
    {
        if (flag)
        {
            transform.Find("i1").Rotate(Vector3.forward * 10f);
        }
        
    }

    private void OnDisable()
    {
        Debug.Log("Ondis");
        flag = false;
    }

    private void OnEnable()
    {
        Debug.Log("Oneable");
        flag = true;
    }

}
