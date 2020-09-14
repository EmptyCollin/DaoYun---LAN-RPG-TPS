using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class BuffManager : MonoBehaviour
{
    private string dataPath = @".\Assets\Resources\Buffs.csv";

    public List<Common.Buff> buffList;
    //private PlayerList playerManager;

    // Start is called before the first frame update
    void Start()
    {
        buffList = new List<Common.Buff>();
        LoadBuffs();

    }

    private void LoadBuffs() {
        CSV csv = new CSV();
        csv.LoadFile(dataPath);
        for (int i = 1; i < csv.m_ArrayData.Count; i++)
        {
            Common.Buff b = new Common.Buff();
            b.id = int.Parse(csv.GetString(i, 0));
            b.name = csv.GetString(i, 1);
            b.description = csv.GetString(i, 2);
            b.intialDuration = float.Parse(csv.GetString(i, 3));
            b.cd = float.Parse(csv.GetString(i, 4));
            b.iconPath = csv.GetString(i, 5);
            b.effectPrefabPath = csv.GetString(i, 6);
            b.functionPrefabPath = csv.GetString(i, 7);
            b.damage = float.Parse(csv.GetString(i, 8));
            b.remainsTime = b.intialDuration;
            buffList.Add(b);
        }
    }


    public Common.Buff FindBuff(int id) {
        foreach (var buff in buffList) {
            if (buff.id == id) {
                return buff;
            }
        }
        return null;
    }

    
}
