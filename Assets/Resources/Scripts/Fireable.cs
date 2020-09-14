using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireable : MonoBehaviour
{
    public GameObject Thefire;

    public void LetsFire()
    {
        Thefire.SetActive(true);
        try
        {
            transform.Find("Element").GetComponent<ElementArea>().EnvironmentEle = Common.Elements.Huo;
        }
        catch { }

    }

}
