using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Packet.GameServer;
using UnityEngine.UI;
using TreeEditor;
using System.Threading.Tasks;
using System;

public class LobbyCanvas : MonoBehaviour
{
    [Tooltip("TopPanel에 붙는 UI들")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text goldTxt;

    [Tooltip("Bottom Panel에 붙는 UI들")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject playModePanel;
    [SerializeField] private GameObject optionPanel;

    [SerializeField] private Toggle inventoryToggle;
    [SerializeField] private Toggle playModeSelectToggle;
    [SerializeField] private Toggle optionToggle;

    private void Start()
    {
        InitLobbyCanvas();
        LoadUserInfo();
    }

    /// <summary>
    /// 초기 Lobby Scene에서의 UI 세팅
    /// </summary>
    private void InitLobbyCanvas()
    {
        inventoryPanel.SetActive(false);
        playModePanel.SetActive(false);
        optionPanel.SetActive(false);

        inventoryToggle.onValueChanged.AddListener(InventoryPanelToggle);
        playModeSelectToggle.onValueChanged.AddListener(PlayModeSelectToggle);
        optionToggle.onValueChanged.AddListener(OptionPanelToggle);
    }
    
    /// <summary>
    /// Login 되었을 때, User의 Info를 가져온다
    /// Login시 PlayerPrefs와 Server에 저장했으니 둘이 같은 정보로
    /// PlayerPrefs에서 user의 정보를 꺼내와서 붙여준다.
    /// </summary>
    private void LoadUserInfo()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            nameTxt.text = LocalData.GetInstance.Name;
            goldTxt.text = LocalData.GetInstance.Gold.ToString();
        }
    }

    #region Panel 컨트롤
    /// <summary>
    /// PlayMode Toggle Button을 누를 때 호출된다
    /// </summary>
    /// <param name="active"> toggle 눌린 상태를 받는다 </param>
    private void PlayModeSelectToggle(bool active)
    {
        playModePanel.SetActive(active);
    }

    /// <summary>
    /// Option Toggle Button을 누를 때 호출된다
    /// </summary>
    /// <param name="active"> toggle 눌린 상태를 받는다 </param>
    private void OptionPanelToggle(bool active)
    {
        optionPanel.SetActive(active);
    }
    #endregion

    #region Button에 직접 연결한 Function
    /// <summary>
    /// Inventory Toggle Button을 누르면 Inventory를 연다.
    /// </summary>
    /// <param name="active"> toggle 눌린 상태를 받는다</param>
    private void InventoryPanelToggle(bool active)
    {
        inventoryPanel.SetActive(active);
    }

    /// <summary>
    /// PlayMode Panel 밑 MultiPlay Button에 직접 연결중
    /// MultiPlay Button을 누르면 매칭을 진행한다.
    /// </summary>
    public async void GoToMultiPlay()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            SceneController.GetInstance.GoToScene("MultiPlay");
            await MatchManager.GetInstance.MatchSetup();
            GameManager.GetInstance.GameStart();
        }
    }

    /// <summary>
    /// PlayMode Panel 밑 SinglePlay Button에 직접 연결중
    /// SinglePlay Button을 누르면 싱글 플레이를 진행한다.
    /// </summary>
    public void GoToSinglePlay()
    {
        SceneController.GetInstance.GoToScene("SinglePlay");
        GameManager.GetInstance.GameStart();
    }

    /// <summary>
    /// Option Panel 밑 Save Button에 직접 연결중
    /// Save Button을 누르면 유저 정보를 저장한다.
    /// 서버가 연결되어 있으면 DB서버에 유저 정보 갱신한다.
    /// </summary>
    public void SaveInfoToSever()
    {
        string name = nameTxt.text;
        int gold = int.Parse(goldTxt.text);

        LocalData.GetInstance.Name = name;
        LocalData.GetInstance.Gold = gold;

        if (GameServer.GetInstance.IsLogin)
        {
            MatchManager.GetInstance.SaveUserInfoServer(name, gold);
        }
    }

    /// <summary>
    /// Option Panel 밑 Clear Button에 직접 연결중
    /// Clear Button을 누르면 유저 정보를 삭제한다.
    /// 서버 연동시엔 DB서버에서 저장된 유저 정보를 삭제한다
    /// </summary>
    public void ClearInfo()
    {
        PlayerPrefs.DeleteAll();
        if (GameServer.GetInstance.IsLogin)
        {
            MatchManager.GetInstance.RemoveUserInfoServer(GameServer.GetInstance.userid);
        }

        SceneController.GetInstance.GoToScene("Login");
    }
    #endregion
}
