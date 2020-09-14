using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class HardToMove : NetworkBehaviour
{
    // Start is called before the first frame update
    private NetWorkAPI net;
    private BuffManager bm;

    void Start()
    {
        net = GetComponent<NetWorkAPI>();
        bm = GameObject.Find("Managers").GetComponent<BuffManager>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isServer) return;
        if (c.gameObject.tag == "Player")
        {
            Common.Buff mudEffect = bm.FindBuff(108);
            Common.Buff b = new Common.Buff {
                name = mudEffect.name,
                id = mudEffect.id,
                iconPath = mudEffect.iconPath,
                cd = mudEffect.cd,
                intialDuration = -1000,
                remainsTime = mudEffect.remainsTime,
                description = mudEffect.description,
                effectPrefabPath = mudEffect.effectPrefabPath,
                functionPrefabPath = mudEffect.functionPrefabPath,
                damage = mudEffect.damage,
                died = false,
                effectGameObject = null,
                functionGameObject = null
            };

            net.CmdAddBuff(b, c.gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (!isServer) return;
        if (c.gameObject.tag == "Player")
        {
            net.RpcRemoveBuff(c.gameObject, 108);
        }
    }
}
