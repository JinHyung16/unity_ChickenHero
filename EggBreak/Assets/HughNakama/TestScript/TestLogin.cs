using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour
{
    public Button loginTestButton;


    public void Start()
    {
        PlayerPrefs.DeleteAll();
        loginTestButton.onClick.AddListener(LoginTest);
    }

    private async void LoginTest()
    {
        await LoginServer.GetInstance.DeviceLogin();
        Debug.LogFormat("<color=orange><b>[Login-Server]</b> Login Test : SUCCESS");
    }
}
