using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private float alpha;
    private float changingSpeed;
    private Common.ShowingType showingType;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
        changingSpeed = 0.25f;
        showingType = Common.ShowingType.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (showingType == Common.ShowingType.FadeIn)
        {
            alpha = alpha + changingSpeed * Time.deltaTime > 1 ? 1 : alpha + changingSpeed * Time.deltaTime;
        }
        else if (showingType == Common.ShowingType.FadeOut) {
            alpha = alpha - changingSpeed * Time.deltaTime < 0 ? 0 : alpha - changingSpeed * Time.deltaTime;
        }

        GetComponent<RawImage>().canvasRenderer.SetAlpha(alpha);
    }

    public void SetTexture(string path) {
        GetComponent<RawImage>().texture = Resources.Load(path) as Texture2D;
    }

    public void FadeIn() {
        showingType = Common.ShowingType.FadeIn;
        alpha = 0;
    }

    public void FadeOut(float al) {
        showingType = Common.ShowingType.FadeOut;
        alpha = al;
    }
}
