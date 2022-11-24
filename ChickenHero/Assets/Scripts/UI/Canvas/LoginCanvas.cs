using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LoginCanvas : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_InputField NameInputField;
    private string nickName = string.Empty;

    public async void OnLineStart()
    {
        if (CheckInputName)
        {
            UserInfoSetting();

            MatchManager.GetInstance.SaveUserInfoServer(nickName, LocalData.GetInstance.Gold);
            await GameServer.GetInstance.LoginToDevice();
            SceneController.GetInstance.GoToScene("Lobby");
        }
    }

    public void OffLineStart()
    {
        if (CheckInputName)
        {
            UserInfoSetting();
            SceneController.GetInstance.GoToScene("Lobby");
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
        if (LocalData.GetInstance.CheckForUserInfo(nickName))
        {
            return;
        }
        else
        {
            LocalData.GetInstance.Name = nickName;
            LocalData.GetInstance.Gold = 500;
        }
    }

    /// <summary>
    /// Unity에서 제공하는 IPointerDownHandler interface를 상속받았다
    /// InputField가 눌렸을 경우, Mobile에서 KeyPard를 열기위해 필요한 함수
    /// </summary>
    /// <param name="eventData"> 터치된 정보를 받는다 </param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerClick.name == "Name InputField")
        {
            MobileManager.GetInstance.ActiveMobileKeyBoard(true);
        }
        else
        {
            MobileManager.GetInstance.ActiveMobileKeyBoard(false);
        }
    }
}
