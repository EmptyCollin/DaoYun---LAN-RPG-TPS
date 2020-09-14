using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lerpColor : MonoBehaviour
{
    MeshRenderer TheRender;
    [SerializeField] [Range(0f, 1f)] float lerpTime;
    [SerializeField] Color[] theColor;
    int Index = 0;
    float t = 0f;
    int len;
    // Start is called before the first frame update
    void Start()
    {
        TheRender = GetComponent<MeshRenderer>();
        len = theColor.Length;
    }

    // Update is called once per frame
    void Update()
    {
        TheRender.material.color = Color.Lerp(TheRender.material.color, theColor[Index], lerpTime);

        t = Mathf.Lerp(t, 1f, lerpTime);
        if (t > 0.9f)
        {
            t = 0f;
            Index++;
            Index = (Index >= theColor.Length) ? 0 : Index;
        }
    }
}
