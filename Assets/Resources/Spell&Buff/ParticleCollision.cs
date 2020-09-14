using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete]
public class ParticleCollision : NetworkBehaviour
{
    // Start is called before the first frame update

    public int spellID = 24;

    void OnParticleCollision(GameObject other) {
        if (!isServer) return;
        other.SendMessage("TakenSpell", spellID, SendMessageOptions.DontRequireReceiver);
    }

}
