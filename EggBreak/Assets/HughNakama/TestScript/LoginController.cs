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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape the Game");
            Application.Quit();
        }
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
        string host = "34.82.70.174";
        int port = 7450;

        await GameServer.GetInstance.Disconnect();
        Debug.LogFormat("<color=red><b>[GameServer-Server]</b> Login : Disconnect </color>");

        await GameServer.GetInstance.ConnectToGameServer(loginServer.userid, host, port);
        Debug.LogFormat("<color=red><b>[Game-Server]</b> Login : Connect </color>");
    }
}
