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
    private CancellationTokenSource tokenSource;

    [SerializeField] private TMP_InputField NameInputField;
    private string nickName = string.Empty;

    private void Awake()
    {
        LoginCheckPanel.SetActive(false);
        if (tokenSource != null)
        {
            tokenSource.Dispose();
        }
        tokenSource = new CancellationTokenSource();

        LoginToServer();
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }
    private void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    private async void LoginToServer()
    {
        await GameServer.GetInstance.LoginToDevice();
    }

    //public async void OnLineStart()
    public void OnLineStart()
    {
        if (CheckInputName)
        {
            if (GameServer.GetInstance.GetIsServerConnect())
            {
                UserInfoSetting();
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
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: tokenSource.Token);
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
            return;
        }
        else
        {
            LocalData.GetInstance.Name = nickName;
            LocalData.GetInstance.Gold = 99999;
            LocalData.GetInstance.Power = 1;
            LocalData.GetInstance.UpgradeLevel = 1;
        }
    }
}
