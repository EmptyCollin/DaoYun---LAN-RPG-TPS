using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{


    public enum Elements { Jin,Mu,Shui,Huo,Tu,Undetermined};

    public enum MovementState { Idel, Run, Jump, Cast, Hitted, Release};

    public enum ActionState { None, Spell, Strucken };

    public enum ShowingType { FadeIn, FadeOut, Normal };

    public enum CursorType { None, Taichi, EDT};

    public static string[] InreactionWithSpell = { "ElementArea", "NoInteractWithSpell" };

    static public float DistinctlyLargeNumber = 1000000f;
    // Start is called before the first frame update
    void Start()
    {
        //InreactionWithSpell.Add("ElementArea");
        //InreactionWithSpell.Add("NoInteractWithSpell");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public class Spell
    {
        public int id;
        public string name;
        public string description;
        public Common.Elements baseEle;
        public Common.Elements releaseEle;
        public string prefabPath;
        public CursorType cursorType;
        public float damage;
    }

    public class Buff {
        public int id;
        public string name;
        public float intialDuration;
        public float remainsTime;
        public string iconPath;
        public string description;
        public string effectPrefabPath;
        public string functionPrefabPath;
        public GameObject effectGameObject;
        public GameObject functionGameObject;
        public float cd;
        public float damage;
        public bool died;
    }

    static public bool IsInteractWithSpell(string tag) {
        foreach (var v in InreactionWithSpell) {
            if (tag.Equals(v.ToString())) return false;
        }
        return true;
    } 

}
