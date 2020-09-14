using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class TrapController : NetworkBehaviour
{

    public Animator ani;
    public int spellID = 13;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isServer) return;
        if (c.gameObject.layer != 9) return;    // layer 9 = character
        RpcPlayAnimation();
        c.gameObject.SendMessage("TakenSpell", spellID, SendMessageOptions.DontRequireReceiver);
    }

    public void DestoryThisObject() {
        Destroy(this.gameObject);
    }

    [ClientRpc]
    private void RpcPlayAnimation()
    {
        ani.SetBool("trigger", true);
    }
}
