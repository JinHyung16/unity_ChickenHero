using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public Button loginBt;
    public Button startBt;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        loginBt.onClick.AddListener(LoginToGameServer);
        startBt.onClick.AddListener(StartToGame);
    }

    private void Start()
    {
        startBt.gameObject.SetActive(false);
    }

    private async void LoginToGameServer()
    {
        var result = await LoginServer.GetInstance.DeviceLogin();

        if (result == null)
        {
            // if login is success
            // startBt visible
            startBt.gameObject.SetActive(true);
        }
    }

    private async void StartToGame()
    {
        LoginServer loginServer = LoginServer.GetInstance;

        await GameServer.GetInstance.Disconnect();
        Debug.LogFormat("<color=green><b>[Game-Server]</b> Login : Disconnect </color>");

        await GameServer.GetInstance.ConnectToGameServer(loginServer.userid);
        Debug.LogFormat("<color=green><b>[Game-Server]</b> Login : Connect </color>");
    }
}
