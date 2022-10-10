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
    private void StartToGame()
    {
        Debug.Log("Game Start");
    }
}
