using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginCanvas : MonoBehaviour
{
    [SerializeField] private TMP_InputField NameInputField;
    private string nickName = string.Empty;


    public async void OnLineStart()
    {
        if (CheckInputName)
        {
            UserInfoSetting();

            GameManager.GetInstance.SaveUserInfo(LocalData.GetInstance.Level, nickName, LocalData.GetInstance.Gold);
            await GameServer.GetInstance.LoginToDevice();
            SceneController.GetInstance.GoToScene("Lobby");
        }
    }

    public void OffLineStart()
    {
        if (CheckInputName)
        {
            UserInfoSetting();
            SceneController.GetInstance.GoToScene("SinglePlay");
        }
    }

    /// <summary>
    /// InputField���� �Է��� ���� �̸��� �޴´�.
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
            LocalData.GetInstance.Level = 1;
            LocalData.GetInstance.Name = nickName;
            LocalData.GetInstance.Gold = 0;
        }
    }
}