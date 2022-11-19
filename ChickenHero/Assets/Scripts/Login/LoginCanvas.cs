using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{

    public async void OnLineStart()
    {
        await GameServer.GetInstance.LoginToDevice();
        SceneController.GetInstance.GoToScene("Lobby");
    }

    public void OffLineStart()
    {
        SceneController.GetInstance.GoToScene("SinglePlay");
    }
}
