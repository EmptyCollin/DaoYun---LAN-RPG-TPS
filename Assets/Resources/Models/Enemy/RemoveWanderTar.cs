using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWanderTar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            Destroy(this.gameObject);
        }
    }
}
