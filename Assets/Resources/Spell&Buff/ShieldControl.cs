using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class ShieldControl : NetworkBehaviour
{
    public float ScrollX = 0.15f;
    public float ScrollY = 0.15f;
    public Material material;
    public float coolDown = -Common.DistinctlyLargeNumber;
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
        // uv
        float offsetX = Time.time * ScrollX;
        float offsetY = Time.time * ScrollY;
        material.mainTextureOffset = new Vector2(offsetX, offsetY);


        // alpha
        if (alpha >= maxAlpha)
        {
            fadeIn = false;
            alpha -= deltaAlpha;
        }
        else if (alpha <= 0) {
            fadeIn = true;
            alpha += deltaAlpha;
        }
        if (fadeIn)
        {
            alpha += deltaAlpha * Time.deltaTime;
        }
        else {
            alpha -= deltaAlpha * Time.deltaTime;
        }
        material.color = new Vector4(1,1,1,alpha);

        // turn on the shield after cd
        coolDown -= Time.deltaTime;
        if (coolDown <= 0 && coolDown > -Common.DistinctlyLargeNumber && isServer) {
            RpcTurnOn(this.gameObject);
            coolDown = -Common.DistinctlyLargeNumber;
        }
    }


    public void TakenSpell(int spellID) {
        if (!isServer) return;
        RpcTurnOff(this.gameObject);
        coolDown = 8f;
    }

    [Command]
    private void CmdTurnOn(GameObject target) {
        RpcTurnOn(target);
    }

    [Command]
    private void CmdTurnOff(GameObject target)
    {
        RpcTurnOff(target);
    }

    [ClientRpc]
    private void RpcTurnOn(GameObject target) {
        target.GetComponent<MeshRenderer>().enabled = true;
        target.GetComponent<Collider>().enabled = true;
    }

    [ClientRpc]
    private void RpcTurnOff(GameObject target)
    {
        target.GetComponent<MeshRenderer>().enabled = false;
        target.GetComponent<Collider>().enabled = false;
    }
}
