using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    public GameObject LoginCanvas;

    public Button loginBtn;
    public Button offlineBtn;

    private void Start()
    {
        if (LoginCanvas == null)
        {
            LoginCanvas = Resources.Load("Canvas/Login Canvas") as GameObject;
        }
        loginBtn.onClick.AddListener(GameStart);
        offlineBtn.onClick.AddListener(OffLineStart);
    }

    private async void GameStart()
    {
        await GameServer.GetInstance.LoginToDevice();
        SceneController.GetInstance.GoToScene("Lobby");
    }

    private void OffLineStart()
    {
        SceneController.GetInstance.GoToScene("SinglePlay");
    }



}
