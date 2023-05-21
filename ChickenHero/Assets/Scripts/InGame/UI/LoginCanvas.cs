using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.EventSystems;
using System.Threading;

public class LoginCanvas : MonoBehaviour
{
    private async void Awake()
    {
        await GameServer.GetInstance.LoginToDevice();
    }

    public void LoginToStart()
    {
        UserInfoSetting();
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }

    /// <summary>
    /// Server 연결이 안되어 있을 경우를 가정하여 만든 함수
    /// Login성공시, OffLine Play시 PlayerPrefs에 데이터를 저장할지 판별해서 저장한다.
    /// </summary>
    private void UserInfoSetting()
    {
        DataManager.GetInstance.Name = "Player_Hero";
        DataManager.GetInstance.Gold = 5000;
        DataManager.GetInstance.Power = 10;
        DataManager.GetInstance.UpgradeLevel = 1;
    }
}
