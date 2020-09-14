using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class NetWorkAPI : NetworkBehaviour
{
    [Command]
    public void CmdAddBuff(Common.Buff buff, GameObject character)
    {
        RpcAddBuff(buff, character);
    }

    [ClientRpc]
    public void RpcAddBuff(Common.Buff buff, GameObject character)
    {
        character.GetComponent<UnitInfo>().AddBuff(buff);
    }

    [Command]
    public void CmdCreateObject(Common.Spell s, Vector3 pos)
    {
        string path = s.prefabPath == "" ? "Prefabs/Stones/Big" : s.prefabPath;
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>(path));

        //meteorolite
        if (s.id == 25 || s.id == 29)
        {
            obj.transform.position = pos + new Vector3(UnityEngine.Random.Range(-50f, 50f), 60f, UnityEngine.Random.Range(-50f, 50f));
            obj.transform.forward = (pos - obj.transform.position).normalized;
            obj.GetComponent<MeteoriteController>().CarrySpell(s.id);
        }
        else
        {
            obj.transform.position = pos;
        }

        NetworkServer.Spawn(obj);
        Destroy(obj, 60f);
    }

    [Command]
    public void CmdCreateExplosion(string path, Vector3 pos, float lifeTime)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>(path));
        obj.transform.position = pos;
        Destroy(obj, lifeTime);
        NetworkServer.Spawn(obj);
    }

    [Command]
    public void CmdCreateBullet(Common.Spell s, Vector3 pos, Vector3 forward)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Spell&Buff/Bullet"));
        obj.transform.position = pos;
        obj.transform.forward = forward;
        obj.GetComponent<BulletController>().CarrySpell(s.id);
        Destroy(obj, 20);
        NetworkServer.Spawn(obj);
        RpcCarrySpellEffect(obj, s);
    }

    [Command]
    public void CmdCreateMud(Common.Spell s, Vector3 pos, Vector3 forward)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Spell&Buff/Mud"));
        obj.transform.position = pos + 2 * forward;
        obj.transform.forward = forward;
        obj.GetComponent<MudControl>().toward = forward.normalized * obj.GetComponent<MudControl>().speed;
        Destroy(obj, 20);
        NetworkServer.Spawn(obj);
    }

    [Command]
    public void CmdCreateFireEmission(Common.Spell s, Vector3 pos, Vector3 forward)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Spell&Buff/FireEmission"));
        obj.transform.position = pos;
        obj.transform.forward = forward;
        Destroy(obj, 3);
        NetworkServer.Spawn(obj);
    }

    [ClientRpc]
    public void RpcCarrySpellEffect(GameObject obj, Common.Spell s)
    {
        string path = s.prefabPath == "" ? "Spell&Buff/FireBall" : s.prefabPath;
        GameObject thisObj = Instantiate<GameObject>(Resources.Load<GameObject>(path), obj.transform);
        thisObj.transform.forward = obj.transform.forward;
    }

    [ClientRpc]
    public void RpcDestoryObject(GameObject obj)
    {
        Destroy(obj);
    }

    [ClientRpc]
    public void RpcSendMassage(GameObject target, string function)
    {
        target.SendMessage(function, SendMessageOptions.DontRequireReceiver);
    }

    [ClientRpc]
    public void RpcSendMassageWithData(GameObject target, string function, string para)
    {
        target.SendMessage(function, para, SendMessageOptions.DontRequireReceiver);
    }


    [ClientRpc]
    public void RpcRemoveBuff(GameObject obj, int id)
    {
        obj.GetComponent<UnitInfo>().RemoveBuff(id);
    }
    [Command]
    public void CmdCreateFireRing(GameObject target, string path, Common.Buff b)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>(path),target.transform);
        b.functionGameObject = obj;
        NetworkServer.Spawn(obj);
    }


    [Command]
    public void CmdBuffCreateObject(GameObject target, string path, Common.Buff b)
    {
        GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>(path),target.transform);

        b.functionGameObject = obj;
        NetworkServer.Spawn(obj);
    }

}
