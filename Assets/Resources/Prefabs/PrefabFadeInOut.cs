using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Obsolete]
public class PrefabFadeInOut : NetworkBehaviour
{
    [SyncVar]
    private float inTime;
    [SyncVar]
    private float outTime;
    [SyncVar]
    private float lifeTime;

    private Color original;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTime(float intime, float outtime, float lifetime) {
        inTime = intime;
        outTime = outtime;
        lifeTime = lifetime;
    }
}
