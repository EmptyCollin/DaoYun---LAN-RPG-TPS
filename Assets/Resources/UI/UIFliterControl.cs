using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFliterControl : MonoBehaviour
{
    public float alpha;
    public bool fadeIn;
    public Color fliterColor;
    public float deltaAlpha;
    public float maxAlpha;
    public float sleepTime;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
        fadeIn = false;
        fliterColor = new Color();
        maxAlpha = 0;
        deltaAlpha = 0;
        mat = GetComponent<MeshRenderer>().material;
        sleepTime = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (fadeIn)
        {
            if (alpha >= maxAlpha)
            {
                fadeIn = false;
                return;
            }
            alpha += deltaAlpha * Time.deltaTime;
        }
        else {
            alpha = alpha - deltaAlpha * Time.deltaTime < 0 ? 0 : alpha - deltaAlpha * Time.deltaTime;
        }
        fliterColor.a = alpha;
        mat.color = fliterColor;

    }

    public void ChangeAnchor(Transform target) {
        transform.SetParent(target);
        transform.position = target.position;
    }

    public void Blink(Color c, float time, float maxAlpha) {
        fadeIn = true;
        fliterColor = c;
        this.maxAlpha = maxAlpha;
        deltaAlpha = maxAlpha / time;
    }
}
