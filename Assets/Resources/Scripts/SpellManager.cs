using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class SpellManager : MonoBehaviour
{
    private string dataPath = @".\Assets\Resources\Spells.csv";

    public List<Common.Spell> spellList;
    private BuffManager buffManager;
    //private PlayerList playerManager;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        spellList = new List<Common.Spell>();
        LoadSpells();

        buffManager = GetComponent<BuffManager>();

    }

    private void LoadSpells() {
        CSV csv = new CSV();
        csv.LoadFile(dataPath);
        for (int i = 1; i < csv.m_ArrayData.Count; i++)
        {
            Common.Spell s = new Common.Spell();
            s.id = int.Parse(csv.GetString(i, 0));
            s.name = csv.GetString(i, 1);
            s.description = csv.GetString(i, 2);
            s.baseEle = (Common.Elements)Enum.Parse(typeof(Common.Elements), csv.GetString(i, 3),true);

            string re = csv.GetString(i, 4);
            if (re == "") s.releaseEle = Common.Elements.Undetermined;
            else s.releaseEle = (Common.Elements)Enum.Parse(typeof(Common.Elements), csv.GetString(i, 4), true);
            s.prefabPath = csv.GetString(i, 5);

            if (csv.GetString(i, 6).ToUpper().Equals("EDT")) {
                s.cursorType = Common.CursorType.EDT;
            }
            else if (csv.GetString(i, 6).ToUpper().Equals("TAICHI"))
            {
                s.cursorType = Common.CursorType.Taichi;
            }
            else {
                s.cursorType = Common.CursorType.None;
            }
            string damage = csv.GetString(i, 7);
            if (damage != "")
            {
                s.damage = float.Parse(damage);
            }
            else {
                s.damage = 0;
            }
            spellList.Add(s);
        }
    }


    public Common.Spell CheckSpellList(Common.Elements baseEle, Common.Elements releaseEle) {
        foreach (var s in spellList) {
            if (s.baseEle == baseEle && s.releaseEle == releaseEle) {
                return s;
            }
        }
        return null;
    }

    public Common.Spell CheckSpellList(int ID)
    {
        foreach (var s in spellList)
        {
            if (s.id == ID)
            {
                return s;
            }
        }
        return null;
    }

}
