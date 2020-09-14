using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialSelection : MonoBehaviour
{
    // Start is called before the first frame update
    private Common.Elements selectedEle;

    void Start()
    {
        selectedEle = Common.Elements.Undetermined;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectElement(Common.Elements e) {
        selectedEle = e;
    }

    public Common.Elements ReportSelection() {
        return selectedEle;
    }
    public void CleanSelection() {
        selectedEle = Common.Elements.Undetermined;
    }
}
