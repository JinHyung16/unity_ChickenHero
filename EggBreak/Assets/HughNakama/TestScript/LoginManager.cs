using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Button logginButton;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
    }
    private void Start()
    {
        logginButton.onClick.AddListener(LoginToGameServer);
    }

    private async void LoginToGameServer()
    {
        await GameServer.GetInstance.DeviceLogin();
        Debug.LogFormat("<color=green><b>[Game-Server]</b> Login : Connect </color>");
    }
}
