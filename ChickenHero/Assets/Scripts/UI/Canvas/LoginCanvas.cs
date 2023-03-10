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
    /// Name Input Field���� �Է��� ���� �̸��� �޴´�.
    /// </summary>
    public void NickNameInput()
    {
        nickName = NameInputField.text;
    }

    /// <summary>
    /// ������ NickName�� �Է��ߴ��� Ȯ���Ѵ�.
    /// </summary>
    /// <returns>�̸� �Է����� �ʾ����� �Է��϶�� �˸� �� false ����, �Է������� true ����</returns>
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
    /// Server ������ �ȵǾ� ���� ��츦 �����Ͽ� ���� �Լ�
    /// Login������, OffLine Play�� PlayerPrefs�� �����͸� �������� �Ǻ��ؼ� �����Ѵ�.
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
            Debug.Log("UserInfoSetting - User�� �̹� �ֽ��ϴ�.");
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
