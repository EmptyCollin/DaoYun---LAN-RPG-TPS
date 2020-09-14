using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class MeteoriteController : NetworkBehaviour
{
    [SyncVar]
    private int spellID = 25;
    public float speed = 15f;

    NetWorkAPI net;

    // Start is called before the first frame update
    void Start()
    {
        net = GetComponent<NetWorkAPI>();
        GameObject fireCloud = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Spell&Buff/CloudSimulate"));
        fireCloud.transform.position = this.transform.position;
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

        if (c.gameObject.tag == "Terrain") {
            net.RpcDestoryObject(this.gameObject);
            ShowExplosion();
        }
    }
    public void CarrySpell(int s)
    {
        //whose = w;
        spellID = s;
    }

    private void ShowExplosion()
    {
        
        Vector3 pos = transform.position + new Vector3(0.0f, -2.0f, 0.0f);
        net.CmdCreateExplosion("Spell&Buff/Explosion", pos, 3f);
        net.CmdCreateExplosion("Spell&Buff/ExplosionFireEmission", pos, 3f);
       
    }
}
