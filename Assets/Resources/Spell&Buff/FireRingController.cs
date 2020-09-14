using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class FireRingController : NetworkBehaviour
{
    //public GameObject Root;

    public GameObject Q1;
    public GameObject Q2;
    public GameObject Q3;

    private FireInRingControl C1;
    private FireInRingControl C2;
    private FireInRingControl C3;
    public int spellID = 104;
    public float CD = 5f;
    private float counter = -Common.DistinctlyLargeNumber;

    public void Go()
    {
        counter = CD;
    }

    void Start()
    {

        C1 = Q1.GetComponent<FireInRingControl>();
        C2 = Q2.GetComponent<FireInRingControl>();
        C3 = Q3.GetComponent<FireInRingControl>();
    }

    // Update is called once per frame
    void Update()
    {

        //transform.position = Root.transform.position;

        if (counter > 0) { counter -= Time.deltaTime; }
        if (counter <= 0 && counter > -Common.DistinctlyLargeNumber) {
            C1.TurnOn();
            C2.TurnOn();
            C3.TurnOn();
            counter = -Common.DistinctlyLargeNumber;
        }
    }
    public void ChildCollideWith(GameObject child, GameObject target) {

        if (!isServer) return;

        if (!Common.IsInteractWithSpell(target.transform.tag)) return;

        target.gameObject.SendMessage("TakenSpell", spellID, SendMessageOptions.DontRequireReceiver);

        Go();

        child.GetComponent<FireInRingControl>().TurnOff();
    }

}
