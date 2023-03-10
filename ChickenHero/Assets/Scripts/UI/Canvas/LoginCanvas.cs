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
    [SerializeField] private TMP_InputField NameInputField;
    private string nickName = string.Empty;

    private async void Awake()
    {
        await GameServer.GetInstance.LoginToDevice();
    }

    public void LoginToStart()
    {
        if (CheckInputName)
        {
            UserInfoSetting();
            SceneController.GetInstance.GoToScene("Lobby").Forget();
        }
    }

    /// <summary>
    /// Name Input Field에서 입력한 유저 이름을 받는다.
    /// </summary>
    public void NickNameInput()
    {
        nickName = NameInputField.text;
    }

    /// <summary>
    /// 유저가 NickName을 입력했는지 확인한다.
    /// </summary>
    /// <returns>이름 입력하지 않았으면 입력하라고 알린 뒤 false 리턴, 입력했으면 true 리턴</returns>
    private bool CheckInputName
    {
        get
        {
            if (string.IsNullOrEmpty(nickName))
            {
                NameInputField.GetComponent<TMP_InputField>().placeholder.GetComponent<TMP_Text>().text = "Please Input NickName";
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Server 연결이 안되어 있을 경우를 가정하여 만든 함수
    /// Login성공시, OffLine Play시 PlayerPrefs에 데이터를 저장할지 판별해서 저장한다.
    /// </summary>
    private void UserInfoSetting()
    {
        LocalData.GetInstance.Name = nickName;
        LocalData.GetInstance.Gold = 10000;
        LocalData.GetInstance.Power = 100;
        LocalData.GetInstance.UpgradeLevel = 1;

        GameManager.GetInstance.curUserName = nickName;

        /*
        if (LocalData.GetInstance.CheckForUserInfo(nickName))
        {
#if UNITY_EDITOR
            Debug.Log("UserInfoSetting - User가 이미 있습니다.");
#endif
            return;
        }
        else
        {
            LocalData.GetInstance.Name = nickName;
            LocalData.GetInstance.Gold = 999;
            LocalData.GetInstance.Power = 100;
            LocalData.GetInstance.UpgradeLevel = 1;
        }
        */
    }
}
