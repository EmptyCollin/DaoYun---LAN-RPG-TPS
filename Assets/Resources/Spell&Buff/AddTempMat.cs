using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class AddTempMat : NetworkBehaviour
{
    public GameObject target;
    public string path;
    public Material newMat;
    public bool isBlink = false;
    public float maxAlpha = 0.4f;
    public float period = 2f;
    public float alpha = 0;
    private float deltaAlpha = 0;
    private bool fadeIn = true;
    // Start is called before the first frame update
    void Start()
    {
        deltaAlpha = maxAlpha / period;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlink) {
            if (alpha >= maxAlpha)
            {
                fadeIn = false;
                alpha -= deltaAlpha;
            }
            else if (alpha <= 0)
            {
                fadeIn = true;
                alpha += deltaAlpha;
            }
            if (fadeIn)
            {
                alpha += deltaAlpha * Time.deltaTime;
            }
            else
            {
                alpha -= deltaAlpha * Time.deltaTime;
            }
            newMat.color = new Vector4(newMat.color.r, newMat.color.g, newMat.color.b, alpha);
        }

    }

    // basic type, constantly render the effect
    public void Intial(GameObject t, string path)
    {
        target = t;
        newMat = Resources.Load<Material>(path);
        this.path = path;

        Material[] oldMatList = target.GetComponent<SkinnedMeshRenderer>().materials;
        Material[] newMatList = new Material[oldMatList.Length + 1];
        for (int i = 0; i < oldMatList.Length; i++)
        {
            newMatList[i] = oldMatList[i];
        }
        newMatList[newMatList.Length - 1] = newMat;
        target.GetComponent<SkinnedMeshRenderer>().materials = newMatList;

    }
    // the effect got lifetime
    public void Intial(GameObject t, string path,float maxAlpha, float period) {
        Intial(t, path);
        this.maxAlpha = maxAlpha;
        this.period = period;
        fadeIn = true;
        alpha = 0;
        deltaAlpha = maxAlpha / period;
        isBlink = true;

    }

    void OnDestroy() {

        Material[] oldMatList = target.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
        Material[] newMatList = new Material[oldMatList.Length - 1];
        for(int i = 0, j = 0; i < oldMatList.Length;i++) {
            if (oldMatList[i] == newMat) continue;
            newMatList[j] = oldMatList[i];
            j++;
        }
        target.GetComponent<SkinnedMeshRenderer>().materials = newMatList;
    }
}
