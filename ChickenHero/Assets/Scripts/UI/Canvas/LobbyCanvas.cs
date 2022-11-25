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
    [Tooltip("TopPanel�� �ٴ� UI��")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text goldTxt;

    [Tooltip("Bottom Panel�� �ٴ� UI��")]
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
    /// �ʱ� Lobby Scene������ UI ����
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
    /// Login �Ǿ��� ��, User�� Info�� �����´�
    /// Login�� PlayerPrefs�� Server�� ���������� ���� ���� ������
    /// PlayerPrefs���� user�� ������ �����ͼ� �ٿ��ش�.
    /// </summary>
    private void LoadUserInfo()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            nameTxt.text = LocalData.GetInstance.Name;
            goldTxt.text = LocalData.GetInstance.Gold.ToString();
        }
    }

    #region Panel ��Ʈ��
    /// <summary>
    /// PlayMode Toggle Button�� ���� �� ȣ��ȴ�
    /// </summary>
    /// <param name="active"> toggle ���� ���¸� �޴´� </param>
    private void PlayModeSelectToggle(bool active)
    {
        playModePanel.SetActive(active);
    }

    /// <summary>
    /// Option Toggle Button�� ���� �� ȣ��ȴ�
    /// </summary>
    /// <param name="active"> toggle ���� ���¸� �޴´� </param>
    private void OptionPanelToggle(bool active)
    {
        optionPanel.SetActive(active);
    }
    #endregion

    #region Button�� ���� ������ Function
    /// <summary>
    /// Inventory Toggle Button�� ������ Inventory�� ����.
    /// </summary>
    /// <param name="active"> toggle ���� ���¸� �޴´�</param>
    private void InventoryPanelToggle(bool active)
    {
        inventoryPanel.SetActive(active);
    }

    /// <summary>
    /// PlayMode Panel �� MultiPlay Button�� ���� ������
    /// MultiPlay Button�� ������ ��Ī�� �����Ѵ�.
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
    /// PlayMode Panel �� SinglePlay Button�� ���� ������
    /// SinglePlay Button�� ������ �̱� �÷��̸� �����Ѵ�.
    /// </summary>
    public void GoToSinglePlay()
    {
        SceneController.GetInstance.GoToScene("SinglePlay");
        GameManager.GetInstance.GameStart();
    }

    /// <summary>
    /// Option Panel �� Save Button�� ���� ������
    /// Save Button�� ������ ���� ������ �����Ѵ�.
    /// ������ ����Ǿ� ������ DB������ ���� ���� �����Ѵ�.
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
    /// Option Panel �� Clear Button�� ���� ������
    /// Clear Button�� ������ ���� ������ �����Ѵ�.
    /// ���� �����ÿ� DB�������� ����� ���� ������ �����Ѵ�
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
