using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class BulletController : NetworkBehaviour
{
    private int spellID;
    public float speed = 30.0f;
    public NetWorkAPI net;

    // Start is called before the first frame update
    void Start()
    {
        net = GetComponent<NetWorkAPI>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isServer) return;

        if (!Common.IsInteractWithSpell(c.transform.tag)) return;

        c.gameObject.SendMessage("TakenSpell", spellID, SendMessageOptions.DontRequireReceiver);
        
        net.RpcDestoryObject(this.gameObject);
    }

    public void CarrySpell(int s)
    {
        spellID = s;
    }
}
