using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using UnityEngine.Networking;
using System;
using Image = UnityEngine.UI.Image;

public class StartSceneUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Button start;
    public Button join;
    public TMP_InputField input;
    public Button go;
    private Color inputColor;

    [System.Obsolete]
    void Start()
    {
        //inputColor = input.GetComponent<RawImage>().color;
        input.gameObject.SetActive(false);
        start.onClick.AddListener(LaunchGame);
        join.onClick.AddListener(ShowInputField);
        go.onClick.AddListener(JoinGame);
    }

    [Obsolete]
    private void ShowInputField()
    {
        input.gameObject.SetActive(true);
        join.GetComponent<Image>().color = Color.gray;
        join.onClick.RemoveAllListeners();
        join.onClick.AddListener(HideInputField);
    }

    [Obsolete]
    private void HideInputField()
    {
        input.gameObject.SetActive(false);
        join.GetComponent<Image>().color = Color.white;
        join.onClick.RemoveAllListeners();
        join.onClick.AddListener(ShowInputField);
    }

    [Obsolete]
    public void JoinGame()
    {
        NetworkManager.singleton.networkAddress = input.text;
        if (input.text.Equals("")) {
            NetworkManager.singleton.networkAddress = "127.0.0.1";
        }
        NetworkManager.singleton.StartClient();
        if (!NetworkClient.active)
        {
            LaunchGame();
        }
    }

    [System.Obsolete]
    private void LaunchGame() {
        NetworkManager.singleton.StartHost();
    }
}
