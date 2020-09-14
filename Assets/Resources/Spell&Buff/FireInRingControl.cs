using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Obsolete]
public class FireInRingControl : MonoBehaviour
{
    private FireRingController parentController;
    private ParticleSystem ps;
    private Collider c;

    void Start() {
        parentController = transform.parent.GetComponent<FireRingController>();
        ps = GetComponent<ParticleSystem>();
        c = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider c)
    {

        parentController.ChildCollideWith(gameObject, c.gameObject);

    }

    public void TurnOff()
    {

        ps.Stop();
        c.enabled = false;
    }

    public void TurnOn()
    {
        ps.Play();
        c.enabled = true;
    }


}
