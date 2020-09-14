using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Cursor = UnityEngine.Cursor;
using System;
using UnityEngine.Animations;

[System.Obsolete]
public class PlayerController : NetworkBehaviour
{

    private float HitStunTime = 0f;

    private Animator ani;
    private float QianHou;
    private float ZuoYou;

    // attributes
    public int ID;

    public float EnvironmentMulti;

    public  Common.Elements CurEnvironmentEle; 
    public  Common.Elements PreEnvironmentEle;
    public float chargeOfEnvEle; 
    //public float chargeOfPreEnvEle;
    public bool isSelectingElement;
    private Vector3 mouseRecord = new Vector3(Screen.width/2, Screen.height/2,0);
    private bool canReleaseSpell;

    // if the spell need to aim
    public Common.Spell holdenSpell;

    // the max distance of edt away from player
    private float EDTDistance;
    private Vector3 EDTLookAtSky = new Vector3(0, 1, 0);

    // objects recording
    private GameObject UI;
    private GameObject UICamera;
    [HideInInspector]public GameObject UIFliter;
    private GameObject SpellDial;
    private CharacterController playerController;
    private GameObject FPCamera;
    private GameObject TPCamera;
    private GameObject healthBar;
    private GameObject healthBarInScreen;
    private GameObject manaBars;
    private GameObject environmentBar;
    private GameObject Taichi;

    private GameObject Taichi1;
    private GameObject Taichi2;
    public Transform _UI_Ele_Jin;
    public Transform _UI_Ele_Mu;
    public Transform _UI_Ele_Shui;
    public Transform _UI_Ele_Huo;
    public Transform _UI_Ele_Tu;

    public GameObject EDT_Projector;
    private GameObject Managers;
    private SpellManager spellManager;
    private BuffManager  buffManager;
    private GameObject buffSlots;

    private UnitInfo info;
    private GameObject preEnvEle_Icon;
    private GameObject curEnvEle_Icon;

    // animation
    private Animator animator;
    private Common.MovementState mState;
    private Common.ActionState aState;

    // view control
    public float m_sensitivityX = 10f;
    public float m_sensitivityY = 10f;
    public float m_minimumX = -360f;
    public float m_maximumX = 360f;
    public float m_minimumY = -45f;
    public float m_maximumY = 45f;
    float m_rotationY = 0f;


    // icons' path
    private string iconPath = "UI/Texture/";
    private string fireIcon = "FireElement";
    private string waterIcon = "WaterElement";
    private string soilIcon = "SoilElement";
    private string metalIcon = "MetalElement";
    private string treeIcon = "TreeElement";
    private string transparent = "transparent";


    // network component
    private NetWorkAPI net;


    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {

        ID = NetworkManager.singleton.numPlayers;

        isSelectingElement = false;
        holdenSpell = null;

        UI = transform.Find("UI").gameObject;
        UICamera = transform.Find("UICamera").gameObject;
        UIFliter = transform.Find("UIFliter").gameObject;
        UIFliter.SetActive(true);
        UI.SetActive(true);
        UICamera.SetActive(true);
        healthBar = UI.transform.Find("CharacterInfo").Find("HealthBar").gameObject;
        manaBars = UI.transform.Find("CharacterInfo").Find("ManaBars").gameObject;
        healthBarInScreen = transform.Find("HealthBar").gameObject;
        healthBarInScreen.SetActive(false);
        environmentBar = UI.transform.Find("CharacterInfo").Find("Environment").Find("EnvironmentBar").gameObject;
        buffSlots = UI.transform.Find("CharacterInfo").Find("BuffSlots").gameObject;
        preEnvEle_Icon = environmentBar.transform.Find("Icon_Pre").gameObject;
        curEnvEle_Icon = environmentBar.transform.Find("Icon_Cur").gameObject;
        CurEnvironmentEle = Common.Elements.Undetermined;
        PreEnvironmentEle = Common.Elements.Undetermined;
        chargeOfEnvEle = 0;

        /*
        _UI_Ele_Jin = SpellDial.transform.Find("Buttons").Find("Element_Jin").Find("TriggerArea");
        _UI_Ele_Mu = SpellDial.transform.Find("Buttons").Find("Element_Mu").Find("TriggerArea");
        _UI_Ele_Shui = SpellDial.transform.Find("Buttons").Find("Element_Shui").Find("TriggerArea");
        _UI_Ele_Huo = SpellDial.transform.Find("Buttons").Find("Element_Huo").Find("TriggerArea");
        _UI_Ele_Tu = SpellDial.transform.Find("Buttons").Find("Element_Tu").Find("TriggerArea");
        */

        Taichi = UI.transform.Find("Taichi").gameObject;
        Taichi1 = Taichi.transform.Find("Taichi1").gameObject;
        Taichi2 = Taichi.transform.Find("Taichi2").gameObject;
        Taichi.SetActive(false);

        animator = transform.Find("Model").GetComponent<Animator>();
        mState = Common.MovementState.Idel;
        aState = Common.ActionState.None;

        Managers = GameObject.Find("Managers").gameObject;
        spellManager = Managers.GetComponent<SpellManager>();
        buffManager =Managers.GetComponent<BuffManager>();

        SpellDial = UI.transform.Find("SpellDial").gameObject;
        SpellDial.SetActive(false);

        playerController = GetComponent<CharacterController>();

        FPCamera = transform.Find("FPCamera").gameObject;
        TPCamera = transform.Find("TPCamera").gameObject;
        TPCamera.SetActive(true);

        info = GetComponent<UnitInfo>();
        info.InitialManaSlots(2);

        EDT_Projector = transform.Find("EDT_Projector").gameObject;
        EDTDistance = 50f;

        Cursor.visible = false;
        UIFliter.GetComponent<UIFliterControl>().ChangeAnchor(TPCamera.transform);

        net = GetComponent<NetWorkAPI>();
        info.state = Common.MovementState.Idel;
        ani = transform.GetComponent<Animator>();

    }


    public void RemoveBuffSlot(int id)
    {
        for (int i = buffSlots.transform.childCount-1; i >=0; i--) {
            if (buffSlots.transform.GetChild(i).GetComponent<BuffSlot>().buff.id == id) {
                Destroy(buffSlots.transform.GetChild(i).gameObject);
                break;
            }
        }

        RelocateBuffSlots();
    }

    public void AddBuffSlot(Common.Buff buff) {
        GameObject obj = Instantiate<GameObject>(Resources.Load<GameObject>("UI/Prefabs/Buff"), buffSlots.transform);
        obj.GetComponent<BuffSlot>().Record(buff);

        RelocateBuffSlots();
    }

    public void RelocateBuffSlots()
    {
        int maxBuffInColumn = 5;
        float width = 75f;
        float height = 75f;
        int buffNum = buffSlots.transform.childCount;

        for (int i = 0, j = 0; i < buffNum; i++) {
            buffSlots.transform.GetChild(i).GetComponent<BuffSlot>().moveTo = new Vector3(width * (i - j * maxBuffInColumn), height * j, 0);
            if (i - j * maxBuffInColumn >= maxBuffInColumn) j++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        

        // update character info.

        RenewUI();
        ShowCrosshair();
        EnvironmentEleCharging();

        // been hit
        if (info.HitTime > 0f || !info.controllable)
        {
        }

        else
        {

            // default set to idel
            info.state = Common.MovementState.Idel;


            // movement control
            Vector3 toward = Vector3.zero;
            Vector3 fall = new Vector3(0, -info.gravity, 0);
            Vector3 planeNormal = new Vector3(0, 1, 0);


            QianHou = 0;
            ZuoYou = 0;
            if (Input.GetKey(KeyCode.W))
            {
                // forward
                info.state = Common.MovementState.Run;
                QianHou = 1;
                toward += Vector3.ProjectOnPlane(transform.forward, planeNormal);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                // backward
                info.state = Common.MovementState.Run;
                QianHou = -1;
                toward += Vector3.ProjectOnPlane(-transform.forward, planeNormal);
            }

            if (Input.GetKey(KeyCode.A))
            {
                // left
                info.state = Common.MovementState.Run;
                ZuoYou = -1;
                toward += Vector3.ProjectOnPlane(-transform.right, planeNormal);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                // right
                info.state = Common.MovementState.Run;
                ZuoYou = 1;
                toward += Vector3.ProjectOnPlane(transform.right, planeNormal);
            }

            if (playerController.isGrounded)
            {
                if (isSelectingElement)
                {
                    playerController.Move(toward.normalized * info.moveMentSpeed * Time.deltaTime * 0.3f);
                }
                else
                {
                    playerController.Move(toward.normalized * info.moveMentSpeed * Time.deltaTime);
                }
            }
            else
            {
                playerController.Move(fall * Time.deltaTime + toward.normalized * info.moveMentSpeed * Time.deltaTime);
            }

            ani.SetFloat("QianHou", QianHou);
            ani.SetFloat("ZuoYou", ZuoYou);

            if (isSelectingElement && info.state != Common.MovementState.Run)
            {
                ani.SetFloat("RunBlend", 1.0f);
                info.state = Common.MovementState.Cast;
            }
            else if (isSelectingElement && info.state == Common.MovementState.Run)
            {
                ani.SetFloat("RunBlend", 0.6f);
                info.state = Common.MovementState.Cast;
            }
            else
            {
                ani.SetFloat("RunBlend", 0.0f);
            }
        }


        // environment element is reach to be filled
        if (chargeOfEnvEle >= 1 && holdenSpell == null)
            {
                SelectElement(CurEnvironmentEle);
                chargeOfEnvEle = 0;
            }
            // selecting elements
            else if (chargeOfEnvEle < 1 && holdenSpell == null)
            {
            if (Input.GetMouseButtonDown(1) && info.CheckManaConsume())
            {
                SetCursorPos((int)(Screen.width / 2), (int)(Screen.height / 2));
                isSelectingElement = true;
                // Cursor.visible = true;
                SpellDial.SetActive(true);

            }
            else if (isSelectingElement && Input.GetMouseButtonUp(1))
            {
                SelectElement(SpellDial.GetComponent<DialSelection>().ReportSelection());
                if (SpellDial.GetComponent<DialSelection>().ReportSelection() != Common.Elements.Undetermined)
                {
                    info.ManaConsume();
                }
                isSelectingElement = false;
                //Cursor.visible = false;
                ClearSpellDial();

                SpellDial.SetActive(false);
            }
            else if(isSelectingElement){
                Vector3 y_axis = new Vector3(0, -1, 0);
                Vector3 z_Axis = new Vector3(0, 0, 1);
                float angle = Vector3.Angle(mouseRecord - Input.mousePosition, y_axis);
                Vector3 normal = Vector3.Cross(mouseRecord - Input.mousePosition, y_axis);
                angle *= Mathf.Sign(Vector3.Dot(normal, z_Axis));
                //Debug.Log(angle);


                
                _UI_Ele_Jin.GetComponent<TriggerArea>().TurnOff();
                _UI_Ele_Mu.GetComponent<TriggerArea>().TurnOff();
                _UI_Ele_Shui.GetComponent<TriggerArea>().TurnOff();
                _UI_Ele_Huo.GetComponent<TriggerArea>().TurnOff();
                _UI_Ele_Tu.GetComponent<TriggerArea>().TurnOff();
                
                if ((mouseRecord - Input.mousePosition).magnitude >= 20f) {
                    Transform uiEle = null;
                    if (-180 <= angle && angle <= -108)
                    {
                        uiEle = _UI_Ele_Huo;
                    }
                    if (-108 < angle && angle <= -36)
                    {
                        uiEle = _UI_Ele_Tu;
                    }
                    else if (-36 < angle && angle <= 36)
                    {
                        uiEle = _UI_Ele_Jin;
                    }
                    else if (36 < angle && angle <= 108)
                    {
                        uiEle = _UI_Ele_Shui;
                    }
                    else if (108 < angle && angle <= 180)
                    {
                        uiEle = _UI_Ele_Mu;
                    }

                    uiEle.GetComponent<TriggerArea>().TurnOn();
                }
                
            }
        }

            // release spell which is needed to aim
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ReleaseSpell(holdenSpell);
                    holdenSpell = null;
                    EDT_Projector.transform.LookAt(EDT_Projector.transform.position + EDTLookAtSky);
                    Taichi.SetActive(false);
                    info.RemoveElements(false, true);
                }
            }


            // F/T camera switch
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                FPCamera.SetActive(false);
                TPCamera.SetActive(true);
                UIFliter.GetComponent<UIFliterControl>().ChangeAnchor(TPCamera.transform);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                TPCamera.SetActive(false);
                FPCamera.SetActive(true);
                UIFliter.GetComponent<UIFliterControl>().ChangeAnchor(FPCamera.transform);

            }

            // view control, disabled when seleting elements
            // need to be updated after buying models
            // when hold Ctrl key, set cursor to be visible and disable view control to make sure player can check game information by moving mouse
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Cursor.visible = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                SetCursorPos((int)(Screen.width / 2), (int)(Screen.height / 2));
                Cursor.visible = false;
            }
            else if (!isSelectingElement)
            {
                float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
                m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
                m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);
                Camera.main.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
                transform.localEulerAngles = new Vector3(0, m_rotationX, 0);

            }

        // pass state to animator
        switch (info.state)
        {
            case Common.MovementState.Idel:
                ani.SetInteger("State", 1);
                break;
            case Common.MovementState.Run:
                ani.SetInteger("State", 2);
                break;
            case Common.MovementState.Release:
                ani.SetInteger("State", 3);
                break;
            case Common.MovementState.Cast:
                ani.SetInteger("State", 4);
                break;
            case Common.MovementState.Hitted:
                ani.SetInteger("State", 5);
                break;
            default:
                break;
        }
        
    }




    private void EnvironmentEleCharging()
    {
        if (PreEnvironmentEle != Common.Elements.Undetermined)
        {
            chargeOfEnvEle -= Time.deltaTime * EnvironmentMulti;
            if (chargeOfEnvEle <= 0)
            {
                PreEnvironmentEle = Common.Elements.Undetermined;
                chargeOfEnvEle = 0;
            }
        }
        else
        {
            if (CurEnvironmentEle != Common.Elements.Undetermined)
            {
                chargeOfEnvEle = chargeOfEnvEle + Time.deltaTime * EnvironmentMulti > 1 ? 1 : chargeOfEnvEle + Time.deltaTime * EnvironmentMulti;
            }
        }
    }

    private void ClearSpellDial()
    {
        Transform buttons = SpellDial.transform.Find("Buttons");
        foreach (Transform e in buttons)
        {
            e.Find("TriggerArea").GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
            SpellDial.GetComponent<DialSelection>().CleanSelection();
        }
    }

    [DllImport("user32.dll")]
    private static extern int SetCursorPos(int x, int y);


    // Start is called before the first frame update

    private void ShowCrosshair()
    {

        if (holdenSpell != null) {
            if (holdenSpell.cursorType == Common.CursorType.EDT)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit[] h = Physics.RaycastAll(ray, EDTDistance);
                foreach (var target in h)
                {
                    if (target.transform.tag == "Terrain")
                    {
                        EDT_Projector.transform.LookAt(target.point);
                        break;
                    }
                }
            }
            else {
                EDT_Projector.transform.LookAt(EDT_Projector.transform.position + EDTLookAtSky);
            }
        }
    }

    private void RenewUI()
    {
        // health bar
        healthBar.GetComponent<Bar_FillControl>().Fill = info.hp / info.maxHp;

        // mana bars
        for (int i = 0; i < Mathf.Min(manaBars.transform.childCount,info.manas.Count); i++) {
            manaBars.transform.GetChild(i).GetComponent<Bar_FillControl>().Fill = info.manas[i] / 1;
        }

        // Taichi effect
        switch (info.baseEle) {
            case Common.Elements.Jin:
                Taichi1.GetComponent<RawImage>().color = Color.yellow;
                break;
            case Common.Elements.Mu:
                Taichi1.GetComponent<RawImage>().color = Color.green;
                break;
            case Common.Elements.Shui:
                Taichi1.GetComponent<RawImage>().color = Color.blue;
                break;
            case Common.Elements.Huo:
                Taichi1.GetComponent<RawImage>().color = Color.red;
                break;
            case Common.Elements.Tu:
                Taichi1.GetComponent<RawImage>().color = new Color(0.635f,0.462f,0.09f);
                break;
            default:
                Taichi1.GetComponent<RawImage>().color = new Color(0.76f, 0.76f, 0.76f);
                break;
        }
        switch (info.releaseEle)
        {
            case Common.Elements.Jin:
                Taichi2.GetComponent<RawImage>().color = Color.yellow;
                break;
            case Common.Elements.Mu:
                Taichi2.GetComponent<RawImage>().color = Color.green;
                break;
            case Common.Elements.Shui:
                Taichi2.GetComponent<RawImage>().color = Color.blue;
                break;
            case Common.Elements.Huo:
                Taichi2.GetComponent<RawImage>().color = Color.red;
                break;
            case Common.Elements.Tu:
                Taichi2.GetComponent<RawImage>().color = new Color(0.635f, 0.462f, 0.09f);
                break;
            default:
                Taichi2.GetComponent<RawImage>().color = new Color(0.76f, 0.76f, 0.76f);
                break;
        }

        if (info.baseEle != Common.Elements.Undetermined && info.releaseEle != Common.Elements.Undetermined) {
            Taichi.transform.RotateAround(Taichi.transform.position, UICamera.transform.forward,2f);
        }

        // environment bar
        if (chargeOfEnvEle > 0)
        {
            environmentBar.SetActive(true);
        }
        else {
            environmentBar.SetActive(false);
        }
        environmentBar.GetComponent<Bar_FillControl>().Fill = chargeOfEnvEle;

    }


    public void SelectElement(Common.Elements e) {
        if (e == Common.Elements.Undetermined) {
            return;
        }
        if (info.baseEle == Common.Elements.Undetermined)
        {
            info.baseEle = e;
        }
        else {
            info.releaseEle = e;
        }
        Common.Spell s = spellManager.CheckSpellList(info.baseEle, info.releaseEle);
        if (s != null) {
            if (s.cursorType == Common.CursorType.EDT) {
                holdenSpell = s;
            }else if(s.cursorType == Common.CursorType.Taichi)
            {
                holdenSpell = s;
                Taichi.SetActive(true);
            }
            else
            {
                ReleaseSpell(s);
                info.RemoveElements(false, true);
            }
        }
    }


    public void ChangeMovmentState(Common.MovementState m) {
        mState = m;
        animator.SetInteger("movementState", (int)m);
    }
    public void ChangeActionState(Common.ActionState a)
    {
        aState = a;
        animator.SetInteger("actionState", (int)a);
    }

    public void ChangeEnvEle(Common.Elements e) {
        PreEnvironmentEle = CurEnvironmentEle;
        CurEnvironmentEle = e;
        ResetIcon();
    }
    private void ResetIcon() {
        preEnvEle_Icon.GetComponent<RawImage>().texture = curEnvEle_Icon.GetComponent<RawImage>().texture;
        preEnvEle_Icon.GetComponent<Fade>().FadeOut(curEnvEle_Icon.GetComponent<RawImage>().canvasRenderer.GetAlpha());

        string path = "";
        switch (CurEnvironmentEle)
        {
            case Common.Elements.Undetermined:
                path = iconPath + transparent;
                break;
            case Common.Elements.Jin:
                path = iconPath + metalIcon;
                break;
            case Common.Elements.Mu:
                path = iconPath + treeIcon;
                break;
            case Common.Elements.Shui:
                path = iconPath + waterIcon;
                break;
            case Common.Elements.Huo:
                path = iconPath + fireIcon;
                break;
            case Common.Elements.Tu:
                path = iconPath + soilIcon;
                break;
        }
        curEnvEle_Icon.GetComponent<Fade>().SetTexture(path);
        curEnvEle_Icon.GetComponent<Fade>().FadeIn();
    }

    public void ReleaseSpell(Common.Spell s)
    {
        //Debug.Log("release spell: " + s.name + " ," + s.description);
        info.state = Common.MovementState.Release;

        if (s.cursorType == Common.CursorType.Taichi)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 pos = transform.position + 5 * forward.normalized;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit[] h = Physics.RaycastAll(ray, 200f);
            foreach (var target in h)
            {
                if (Common.IsInteractWithSpell(target.transform.tag))
                {
                    forward = target.point - pos;
                    break;
                }
            }
            if (s.id == 24)
            {
                net.CmdCreateFireEmission(s, pos, forward);
            }
            else if (s.id == 10 || s.id == 26) {
                net.CmdCreateMud(s, pos, forward.normalized);
            }
            else
            {
                net.CmdCreateBullet(s, pos, forward);
            }

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
            net.CmdCreateObject(s,position);
        }
        else
        {
            int buffId = 0;
            switch (s.id){
                case 1: 
                    buffId = 101;
                    break;
                case 2: 
                    buffId = 102;
                    break;
                case 3:
                    buffId = 103;
                    break;
                case 4:
                    buffId = 104;
                    break;
                case 5:
                    buffId = 105;
                    break;
                case 7:
                    buffId = 112;
                    break;
                case 9:
                    buffId = 111;
                    break;
                case 11:
                    buffId = 111;
                    break;
                case 15:
                    buffId = 112;
                    break;
                case 18:
                    buffId = 106;
                    break;
                case 19:
                    buffId = 112;
                    break;
                case 20:
                    buffId = 111;
                    break;
                case 21:
                    buffId = 112;
                    break;
                case 23:
                    buffId = 111;
                    break;
                case 27:
                    buffId = 111;
                    break;
                case 28:
                    buffId = 112;
                    break;
                case 30:
                    buffId = 110;
                    break;
            }
            Common.Buff b = buffManager.FindBuff(buffId);
            if (b != null) {
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
                    functionPrefabPath = b.functionPrefabPath,
                    damage = b.damage,
                    died = false,
                    effectGameObject = null,
                    functionGameObject = null

            };
                net.CmdAddBuff(buff, this.gameObject);
            }

        }

    }
    
}
