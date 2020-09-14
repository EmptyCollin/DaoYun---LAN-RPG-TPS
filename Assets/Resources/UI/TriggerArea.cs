using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerArea : MonoBehaviour
{
    public Common.Elements element;
    private GameObject particle;
    private GameObject SpellDial;

    private Color transparency;
    private Color original;

    // Start is called before the first frame update
    void Start()
    {
        particle = transform.parent.Find("Particle").gameObject;
        SpellDial = transform.parent.parent.parent.gameObject;
        transparency = new Color(1, 1, 1, 0);
        original = GetComponent<RawImage>().color;
        GetComponent<RawImage>().color = transparency;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnOn()
    {
        particle.GetComponent<ParticleSystem>().Play();
        SpellDial.GetComponent<DialSelection>().SelectElement(element);
        GetComponent<RawImage>().color = original;

    }

    public void TurnOff()
    {
        particle.GetComponent<ParticleSystem>().Stop();
        SpellDial.GetComponent<DialSelection>().SelectElement(Common.Elements.Undetermined);
        GetComponent<RawImage>().color = transparency;
    }
}
