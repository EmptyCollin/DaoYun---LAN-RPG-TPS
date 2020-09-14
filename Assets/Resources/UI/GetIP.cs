using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;

public class GetIP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_Text>().text = "IP: " + GetAddressIP();
    }


    private string GetAddressIP()
    {

        string AddressIP = string.Empty;
        foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (_IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                AddressIP = _IPAddress.ToString();
            }
        }
        return AddressIP;
    }
}
