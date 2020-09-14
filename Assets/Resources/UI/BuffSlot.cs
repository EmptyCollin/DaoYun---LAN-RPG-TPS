using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

[Obsolete]
public class BuffSlot : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Common.Buff buff;    // the id of unit who do this operation
    private GameObject bubble;
    private GameObject text;
    private GameObject time;
    //private bool needToAdjustLocation;
    public Vector3 moveTo;
    private float movementSpeed; 
    // Start is called before the first frame update
    void Start()
    {
        bubble = transform.Find("Bubble").gameObject;
        text = bubble.transform.Find("Text").gameObject;
        time = transform.Find("Time").gameObject;
        bubble.SetActive(false);
        movementSpeed = 10.0f;

    }

    // Update is called once per frame
    void Update()
    {
        // re-location of the slot
        if ((transform.localPosition - moveTo).magnitude > 0.1f) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTo, movementSpeed);
        }
        if (buff != null && buff.intialDuration >0) {
            time.GetComponent<TMP_Text>().text = "(" + ((int)buff.remainsTime).ToString() + ")";
        }


    }

    public void Record(Common.Buff b)
    {
        bubble = transform.Find("Bubble").transform.gameObject;
        text = bubble.transform.Find("Text").gameObject;
        buff = b;

        GetComponent<RawImage>().texture = Resources.Load<Texture2D>(buff.iconPath);
        text.GetComponent<TMP_Text>().text = buff.name + "," + buff.description;

    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        bubble.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        bubble.SetActive(false);
    }
}
