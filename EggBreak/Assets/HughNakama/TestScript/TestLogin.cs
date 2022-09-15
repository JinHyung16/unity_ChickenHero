using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour
{
    public Button loginTestButton;

    public void Awake()
    {
       // PlayerPrefs.DeleteAll();
    }

    public void Start()
    {
        loginTestButton.onClick.AddListener(Login);
    }

    private async void LoginTest()
    {
        await GameServer.GetInstance.DeviceLogin();
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> Login Test : SUCCESS");
    }

    public void Login()
    {
        LoginTest();
    }
}
