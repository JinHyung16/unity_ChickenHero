using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    public GameObject LoginCanvas;

    public Button loginButton;

    private void Start()
    {
        if (LoginCanvas == null)
        {
            LoginCanvas = Resources.Load("Canvas/Login Canvas") as GameObject;
        }
        loginButton.onClick.AddListener(GameStart);
    }

    private async void GameStart()
    {
        await GameServer.GetInstance.DeviceLogin();
        SceneController.GetInstance.GoToScene("Lobby");
    }
    

}
