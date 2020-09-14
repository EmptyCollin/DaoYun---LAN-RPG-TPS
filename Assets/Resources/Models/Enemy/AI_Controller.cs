using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AI_Controller : NetworkBehaviour
{
    public GameObject forWander;
    UnitInfo Unifo;

    // hardcode element table, should move to somewhere else
    Dictionary<string,string> HelpfulElements = new Dictionary<string, string>();
    Dictionary<string, string> HarmfulElements = new Dictionary<string, string>();

    GameObject QuestionMark;
    GameObject WowMark;

    float wanderGap = 10f;

    public float moveSpeed = 4f;
    CharacterController CC;

    Animator ani;


    public string MyElement;
    private string ElementApplied;

    float Attack_range = 30f;
    float cooldown = 5f;

    [HideInInspector] public bool Found;

    // target only if found
    public GameObject target = null;


    // network component
    [System.Obsolete]
    private NetWorkAPI net;
    private GameObject EDT_Projector;
    private GameObject Managers;
    private SpellManager spellManager;
    private BuffManager buffManager;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        ani = GetComponent<Animator>();
        CC = gameObject.GetComponent<CharacterController>();
        Unifo = GetComponent<UnitInfo>();

        QuestionMark = transform.Find("Question").gameObject;
        WowMark = transform.Find("WOW").gameObject;

        // add elements to dic
        HelpfulElements.Add("Jin", "Tu");
        HelpfulElements.Add("Tu", "Huo");
        HelpfulElements.Add("Huo", "Mu");
        HelpfulElements.Add("Mu", "Shui");
        HelpfulElements.Add("Shui", "Jin");

        HarmfulElements.Add("Mu", "Jin");
        HarmfulElements.Add("Jin", "Huo");
        HarmfulElements.Add("Huo", "Shui");
        HarmfulElements.Add("Shui", "Tu");
        HarmfulElements.Add("Tu", "Mu");


        EDT_Projector = transform.Find("EDT_Projector").gameObject;
        Managers = GameObject.Find("Managers").gameObject;
        spellManager = Managers.GetComponent<SpellManager>();
        buffManager = Managers.GetComponent<BuffManager>();
        net = GetComponent<NetWorkAPI>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!isServer) { return; }

        // been hit
        if (Unifo.HitTime > 0)
        {
            ani.SetBool("BeHit", true);
        }

        else
        {
            if (ani.GetBool("BeHit") == true) { ani.SetBool("BeHit", false); }
            
            // Found player?
            switch (Found)
            {
                case true:
                    ani.SetBool("FoundPlayer", true);
                    TryAttack();
                    break;
                case false:
                    ani.SetBool("FoundPlayer", false);
                    ani.SetBool("PlayerInRange", false);
                    break;
            }


            // Always look at target
            if (target)
            {
                gameObject.transform.LookAt(target.transform);

                // attack or move?
                switch (ani.GetBool("PlayerInRange"))
                {
                    case true:
                        //attack;
                        break;
                    case false:
                        MoveToTarget();
                        break;
                }

                // State sign
                // player
                if (target.tag == "Player")
                {
                    QuestionMark.SetActive(false);
                    WowMark.SetActive(true);
                    ani.SetBool("Warning", true);

                    // reset just in case
                    ani.SetBool("Wander", false);
                }
                // empty box
                else if (target.tag == "IsEnemyThere?")
                {
                    QuestionMark.SetActive(true);
                    WowMark.SetActive(false);
                    ani.SetBool("Warning", true);
                }
                // wander moving
                else
                {
                    ani.SetBool("Wander", true);
                    QuestionMark.SetActive(false);
                    WowMark.SetActive(false);
                    ani.SetBool("Warning", false);
                    wanderGap -= Time.deltaTime;
                    // its time for next position
                    if (wanderGap <= 0)
                    {
                        Wander();
                    }

                }
                

            }
            else
            {
                QuestionMark.SetActive(false);
                WowMark.SetActive(false);
                ani.SetBool("Wander", false);
                wanderGap -= Time.deltaTime;
                // its time for next position
                if (wanderGap <= 0)
                {
                    Wander();
                }
            }
        }
        

        // handel elements
        ShenAndKe();

        cooldown -= Time.deltaTime;
    }

    void TryAttack()
    {
        if (!target) { return; }
        // in range = attack
        if (Vector3.Distance(this.transform.position, target.transform.position) <= Attack_range && target.tag == "Player")
        {
            ani.SetBool("PlayerInRange", true);
        }
        else
        {
            ani.SetBool("PlayerInRange", false);
        }
    }

    void MoveToTarget()
    {
        CC.Move(transform.forward.normalized * moveSpeed * Time.deltaTime);
    }

    void ShenAndKe()
    {
        if (ElementApplied == MyElement || ElementApplied == HelpfulElements[MyElement])
        {
            GetComponent<UnitInfo>().healthRecoverSpeed = 1;
        }
        else if (ElementApplied == HarmfulElements[MyElement])
        {
            GetComponent<UnitInfo>().healthRecoverSpeed = -1;
        }
        else
        {
            GetComponent<UnitInfo>().healthRecoverSpeed = 0;
        }
    }
    

    public void ReleaseSpell(Common.Spell s)
        {
            Debug.Log("release spell: " + s.name + " ," + s.description);
            if (s.cursorType == Common.CursorType.Taichi)
            {
                Vector3 forward = transform.forward;
                Vector3 pos = transform.position + 5 * forward + new Vector3(0.0f, 2.0f, 0.0f);

                forward = target.transform.position - pos;

                net.CmdCreateBullet(s, pos, forward);
            }
            else if (s.cursorType == Common.CursorType.EDT)
            {
                Ray ray = new Ray(EDT_Projector.transform.position, EDT_Projector.transform.forward);
                RaycastHit[] h = Physics.RaycastAll(ray, 200f);
                Vector3 position = new Vector3(0, 0, 0);
                foreach (var target in h)
                {
                    if (target.transform.tag == "Terrain")
                    {
                        position = target.point;
                        break;
                    }

                }
                net.CmdCreateObject(s, position);
            }
            else
            {
                int buffId = 0;
                switch (s.id)
                {
                    case 1:
                        buffId = 1;
                        break;
                    case 2:
                        buffId = 2;
                        break;
                    case 3:
                        buffId = 3;
                        break;
                    case 4:
                        buffId = 4;
                        break;
                    case 5:
                        buffId = 5;
                        break;
                    case 6:
                        buffId = 10;
                        break;
                    case 7:
                        buffId = 12;
                        break;
                    case 9:
                        buffId = 11;
                        break;
                    case 11:
                        buffId = 11;
                        break;
                    case 15:
                        buffId = 12;
                        break;
                    case 18:
                        buffId = 6;
                        break;
                    case 19:
                        buffId = 12;
                        break;
                    case 20:
                        buffId = 11;
                        break;
                    case 21:
                        buffId = 11;
                        break;
                    case 23:
                        buffId = 12;
                        break;
                    case 27:
                        buffId = 11;
                        break;
                    case 28:
                        buffId = 12;
                        break;
                }
                Common.Buff b = buffManager.FindBuff(buffId);
                if (b != null)
                {
                    Common.Buff buff = new Common.Buff
                    {
                        name = b.name,
                        id = b.id,
                        iconPath = b.iconPath,
                        cd = b.cd,
                        intialDuration = b.intialDuration,
                        remainsTime = b.remainsTime,
                        description = b.description,
                        effectPrefabPath = b.effectPrefabPath,
                        functionPrefabPath = b.functionPrefabPath
                    };
                    net.CmdAddBuff(buff, this.gameObject);
                }

            }


        }


    //SendEmeraldDamage;
    public void EmeraldAttackEvent()
    {
        ReleaseSpell(spellManager.CheckSpellList(Common.Elements.Huo, Common.Elements.Mu));      
    }

    // return a random valid postion to move
    private void Wander()
    {
        float X = transform.position.x;
        float Z = transform.position.z;
        float Y_offset = Random.Range(-1f, 1f);
        float life = Random.Range(7f, 14f);
        Vector3 position = new Vector3(Random.Range(X-15.0f, X+15.0f), transform.position.y+ Y_offset, Random.Range(Z-15.0f, Z+15.0f));

        
        GameObject test = Instantiate(forWander);
        test.transform.position = position;
        target = test;
        wanderGap = life*2;
        Destroy(test, life);
        
    }
}
