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
    [SerializeField] private GameObject LoginCheckPanel; //���� ������ �ȵǾ����� ��, ��� ����â

    [SerializeField] private TMP_InputField NameInputField;
    private string nickName = string.Empty;

    private async void Awake()
    {
        LoginCheckPanel.SetActive(false);
        await GameServer.GetInstance.LoginToDevice();
    }

    public void OnLineStart()
    {
        if (CheckInputName)
        {
            if (GameServer.GetInstance.GetIsServerConnect())
            {
                UserInfoSetting();
                MatchManager.GetInstance.InitMatchManager();
                SceneController.GetInstance.GoToScene("Lobby").Forget();
            }
            else
            {

                LoginCheckPanelUpdate().Forget();
            }
        }
    }
    private async UniTaskVoid LoginCheckPanelUpdate()
    {
        LoginCheckPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        LoginCheckPanel.SetActive(false);
    }

    public void OffLineStart()
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
    }
}
