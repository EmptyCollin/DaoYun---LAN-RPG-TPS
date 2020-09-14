using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

[Obsolete]
public class UnitInfo : NetworkBehaviour
{
    [SyncVar]
    public float hp = 100;
    [SyncVar]
    public float maxHp = 100;

    public List<float> manas;
    public List<Common.Buff> buffs;
    public float manaRecoverSpeed = 0;
    public float healthRecoverSpeed = 0;
    public float moveMentSpeed = 10f;
    public float damageTakenMulti = 1.0f;
    public float gravity = 9.8f;
    public bool controllable = true;
    public bool hurtable = true;
    private Bar_FillControl healthBar;
    private UIFliterControl uifiliter;
    private NetWorkAPI net;
    private System.Object[] defaultData;
    public Common.Elements baseEle = Common.Elements.Undetermined;
    public Common.Elements releaseEle = Common.Elements.Undetermined;

    [HideInInspector] public Common.MovementState state;
    [HideInInspector] public float HitTime = 0;
    public float HitTimeDuration = 1f;


    private GameObject Managers;
    private SpellManager spellManager;
    private BuffManager buffManager;

    // Start is called before the first frame update
    void Awake()
    {
        manas = new List<float>();
        buffs = new List<Common.Buff>();
        try
        {
            healthBar = transform.Find("HealthBar").GetComponent<Bar_FillControl>();
        }
        catch { }

        try {
            uifiliter = transform.Find("UIFliter").GetComponent<UIFliterControl>();
        }
        catch { }

        net = GetComponent<NetWorkAPI>();
        Managers = GameObject.Find("Managers").gameObject;
        spellManager = Managers.GetComponent<SpellManager>();
        buffManager = Managers.GetComponent<BuffManager>();
        defaultData = new object[10];
        RecordDefaultData();

    }

    private void RecordDefaultData()
    {
        defaultData[0] = maxHp;
        defaultData[1] = healthRecoverSpeed;
        defaultData[2] = manaRecoverSpeed;
        defaultData[3] = moveMentSpeed;
        defaultData[4] = controllable;
        defaultData[5] = hurtable;
        defaultData[6] = damageTakenMulti;
    }

    private void RetrievedDefaultData()
    {
        maxHp = (float)defaultData[0];
        healthRecoverSpeed = (float)defaultData[1];
        manaRecoverSpeed = (float)defaultData[2];
        moveMentSpeed = (float)defaultData[3];
        controllable = (bool)defaultData[4];
        hurtable = (bool)defaultData[5];
        damageTakenMulti = (float)defaultData[6];
    }

    // Update is called once per frame
    void Update()
    {
        RetrievedDefaultData();
        // update buffs
        for (int i = buffs.Count-1;i>=0;i--) {
            ApplyBuff(buffs[i]);
            buffs[i].remainsTime -= Time.deltaTime;
            if (buffs[i].remainsTime <= 0 && !buffs[i].died && buffs[i].intialDuration > 0 && isServer) {
                buffs[i].died = true;
                net.RpcRemoveBuff(this.gameObject, buffs[i].id);
            }
        }

        // recover
        if (HitTime > 0)
        {
            HitTime -= Time.deltaTime;
        }

        if (hp <= 0 && isServer && healthBar != null)
        {
            // respawn player
            if (gameObject.tag == "Player") {
                gameObject.SetActive(false);
                hp = 100;
                RetrievedDefaultData();
                GameObject Neww;
                Neww = GameObject.FindGameObjectsWithTag("startPos")[(int)(UnityEngine.Random.Range(0f,2.9f))];
                transform.position = Neww.transform.position;
                gameObject.SetActive(true);
            }
            else
            {
                net.RpcDestoryObject(this.gameObject);
            }
        }

        RecoverHealth();
        RecoverMana();

        // update UI
        try
        {
            healthBar.Fill = hp / maxHp;
        }
        catch { }
    }

    private void ApplyBuff(Common.Buff buff)
    {
        switch (buff.id)
        {
            case 101:

                break;
            case 102:
                manaRecoverSpeed *= 2;
                break;
            case 103:
                moveMentSpeed *= 1.25f;
                break;
            case 104:

                break;
            case 105:
                damageTakenMulti *= 0.9f;
                break;
            case 106:
                healthRecoverSpeed += buff.damage;
                break;
            case 107:
                moveMentSpeed *= 0.7f;
                break;
            case 108:
                moveMentSpeed *= 0.5f;
                break;
            case 109:
                controllable = false;
                break;
            case 110:
                hurtable = false;
                moveMentSpeed *= 0.3f;
                break;
        }
    }

    private void RecoverHealth()
    {
        hp = hp + healthRecoverSpeed * Time.deltaTime >= maxHp ? maxHp : hp + healthRecoverSpeed * Time.deltaTime;
    }

    private void RecoverMana()
    {
        for (int i = 0; i < manas.Count; i++)
        {
            if (manas[i] < 1)
            {
                manas[i] = manas[i] + manaRecoverSpeed * Time.deltaTime >= 1 ? 1 : manas[i] + manaRecoverSpeed * Time.deltaTime;
            }
        }
    }

    public void InitialManaSlots(int num) {
        for (int i = 0; i < num; i++) {
            manas.Add(0.0f);
        }
    }

    public bool CheckManaConsume()
    {
        for (int i = manas.Count - 1; i >= 0; i--)
        {
            if (manas[i] == 1)
            {
                return true;
            }
        }
        return false;
    }

    public void ManaConsume()
    {
        for (int i = manas.Count - 1; i >= 0; i--)
        {
            if (manas[i] == 1)
            {
                manas[i] = 0;
                return;
            }
        }
    }

    // objs[0] - who release this spell
    // objs[1] - what spell it is
    public void TakenSpell(int spellID) {
        Debug.Log(gameObject.name + " is taken " + spellID);
        if (!isServer) return;

        // fire type
        int[] fireType = { 4, 14, 22, 24, 25, 29, 104 };
        if (fireType.Contains(spellID))
        {
            net.RpcSendMassage(this.gameObject, "LetsFire");
        }


        Common.Spell s = spellManager.CheckSpellList(spellID);
        Common.Buff b = buffManager.FindBuff(spellID);
        float damage = s == null ? b.damage : s.damage;
        if (healthBar != null && hurtable)
        {
            // On hit
            hp -= damage;
            state = Common.MovementState.Hitted;
            HitTime = HitTimeDuration;

            if (uifiliter != null) uifiliter.Blink(Color.red, 0.3f, 0.7f);

            // add buff after been damaged
            int id = 0;
            switch (spellID) {
                case 8:
                    id = 107;
                    break;
                case 10:
                    id = 108;
                    break;
                case 13:
                    id = 109;
                    break;
                case 16:
                    id = 107;
                    break;
                case 17:
                    id = 109;
                    break;
                case 25:
                    id = 109;
                    break;
                case 26:
                    id = 108;
                    break;
                case 29:
                    id = 109;
                    break;
            }
            Common.Buff buff = buffManager.FindBuff(id);
            if (buff != null) net.CmdAddBuff(buff, gameObject);
        }
      
    }

    public void AddBuff(Common.Buff b) {

        // common buffs
        if (b.id != 111 && b.id != 112)
        {
            if (b.id <= 105 && isServer) {
                foreach (var buff in buffs)
                {
                    if (buff.id <= 105)
                    {
                        net.RpcRemoveBuff(gameObject,buff.id);
                        break;
                    }
                }
            }
            // re-new buff
            foreach (var buff in buffs)
            {
                if (buff.id == b.id)
                {
                    buff.remainsTime = buff.intialDuration;
                    return;
                }
            }

            // create instance
            GameObject obj;
            string path = b.effectPrefabPath;
            if (path != "") {
                switch (b.id) {
                    case 107:
                        if (transform.Find("Model") == null)
                        {
                            break;
                        }
                        obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), transform);
                        obj.GetComponent<AddTempMat>().Intial(transform.Find("Model").Find("Skin").gameObject, "Spell&Buff/FrozenSkin", 0.5f, 0.5f);
                        b.effectGameObject = obj;
                        break;
                    case 110:
                        obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), transform);
                        obj.GetComponent<AddTempMat>().Intial(transform.Find("Model").Find("Skin").gameObject, "Spell&Buff/MudSkin");
                        b.effectGameObject = obj;
                        break;
                    default:
                        obj = Instantiate<GameObject>(Resources.Load<GameObject>(path), transform);
                        b.effectGameObject = obj;
                        break;
                }

            }
            path = b.functionPrefabPath;
            if (path != "" && isServer) {

                switch (b.id)
                {
                    case 104:
                        net.CmdCreateFireRing(gameObject, path, b);
                        break;

                    default:
                        net.CmdBuffCreateObject(gameObject, path, b);
                        break;
                }
            }



            // add new buff
            buffs.Add(b);

            // if the buff is added to player, update UI
            try
            {
                GetComponent<PlayerController>().AddBuffSlot(b);
            }
            catch { }
        }

        // special buff, to remove base element and all buffs
        else {
            Color c = new Color();
            switch (baseEle)
            {
                case Common.Elements.Jin:
                    c = Color.yellow;
                    break;
                case Common.Elements.Mu:
                    c = Color.green;
                    break;
                case Common.Elements.Shui:
                    c = new Color(0.02f, 0.52f, 0.72f, 1f);
                    break;
                case Common.Elements.Huo:
                    c = new Color(1, 0.1f, 0, 1); ;
                    break;
                case Common.Elements.Tu:
                    c = new Color(0.75f, 0.4f, 0, 1f);
                    break;
                default:
                    c = new Color(1, 1, 1, 0.5f);
                    break;
            }
            GameObject obj = NetworkBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>(b.effectPrefabPath));
            obj.transform.position = transform.position;
            obj.transform.parent = transform;
            obj.GetComponent<ChangeParticleColor>().Doit(c);
            Destroy(obj, 3f);

            RemoveElements(true, true);

            if (isServer) {
                net.RpcSendMassage(this.gameObject, "RemoveAllBuffs");
                if (b.id == 112)
                {
                    // taken damage
                    TakenSpell(b.id);

                }
            }

        
        }


        
    }

    // only for remove the buffs which are directily created by single element
    public void RemoveAllBuffs() {

        for (int i = buffs.Count - 1; i >= 0; i--) {
            RemoveBuff(buffs[i].id);
        }
    }

    public void RemoveBuff(int buffID) {
        try
        {
            GetComponent<PlayerController>().RemoveBuffSlot(buffID);
        }
        catch
        { }

        foreach (var buff in buffs)
        {
            if (buff.id == buffID)
            {
                //remove the effect
                Destroy(buff.effectGameObject);
                Destroy(buff.functionGameObject);
                // remove the record
                buffs.Remove(buff);
                return;
            }
        }

        
    }
    public void RemoveElements(bool b, bool r)
    {
        if (b)
        {
            baseEle = Common.Elements.Undetermined;
        }
        if (r)
        {
            releaseEle = Common.Elements.Undetermined;
        }
    }
}
