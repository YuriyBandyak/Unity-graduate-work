using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsTest : MonoBehaviour
{
    //stats
    [Range(0,1)]
    public float r=1;
    [Range(0, 1)]
    public float g = 1;
    [Range(0, 1)]
    public float b = 1;
    [Range(0, 1)]
    public float a = 1;

    private void Update()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, a);
    }
}
