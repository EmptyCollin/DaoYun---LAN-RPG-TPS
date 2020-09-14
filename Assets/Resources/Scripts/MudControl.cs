using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class MudControl : NetworkBehaviour
{
    public float ScrollX = 0.5f;
    public float ScrollY = 0.5f;
    public float speed = 20f;
    public float grivaty = -9.8f;
    public float airFiction = 0.3f;
    public Vector3 toward;
    private float grivatySpeed = 0;
    private Material material;
    public int spellID = 10;
    private NetWorkAPI net;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        net = GetComponent<NetWorkAPI>();
    }

    // Update is called once per frame
    void Update()
    {
        // uv
        float offsetX = Time.time * ScrollX;
        float offsetY = Time.time * ScrollY;
        material.mainTextureOffset = new Vector2(offsetX, offsetY);


        // displacement
        grivatySpeed += grivaty * Time.deltaTime;
        Vector3 G = new Vector3(0, grivatySpeed, 0);
        toward = (toward.magnitude - airFiction * Time.deltaTime) * toward.normalized;
        transform.position += toward * speed * Time.deltaTime + G * Time.deltaTime;

        transform.forward = (toward * speed + G).normalized;
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isServer) return;

        if (!Common.IsInteractWithSpell(c.transform.tag)) return;

        c.gameObject.SendMessage("TakenSpell", spellID, SendMessageOptions.DontRequireReceiver);

        net.RpcDestoryObject(this.gameObject);
    }

    void OnDestroy()
    {
        if (!isServer) return;
        CmdShowBoom();

    }

    [Command]
    private void CmdShowBoom() {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Spell&Buff/WaterBoom"));
        obj.transform.position = transform.position;
        obj.transform.forward = -transform.forward;
        Destroy(obj, 5f);
        NetworkServer.Spawn(obj);
    }

    [ClientRpc]
    private void RpcShowBoom() {

    }
}
