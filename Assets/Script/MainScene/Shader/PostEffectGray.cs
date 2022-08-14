using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectGray : MonoBehaviour
{
    public Material grayMaterial;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, grayMaterial);
    }
}


