using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[System.Obsolete]
public class ElementArea : MonoBehaviour
{
    public Common.Elements EnvironmentEle;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "Player")
        {
            c.gameObject.GetComponent<PlayerController>().ChangeEnvEle(EnvironmentEle);
        }
    }
    
    void OnTriggerExit(Collider c) {
        if (c.gameObject.tag == "Player")
        {
            c.gameObject.GetComponent<PlayerController>().ChangeEnvEle(Common.Elements.Undetermined);
        }
    }
}
