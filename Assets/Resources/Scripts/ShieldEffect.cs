using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    public Material forceFieldMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        forceFieldMat.SetVector("_CameraPos", Camera.main.transform.position);
        double malti = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) / 4) * 20;
        forceFieldMat.SetFloat("_Malti", (float)malti);
        forceFieldMat.SetVector("_Center", transform.position);
    }
}
