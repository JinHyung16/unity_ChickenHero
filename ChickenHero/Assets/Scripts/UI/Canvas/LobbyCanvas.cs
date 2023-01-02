using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Packet.GameServer;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEngine.EventSystems;
using HughUI; //UIType ����� ����
using HughUtility.Observer;

public class LobbyCanvas : MonoBehaviour, IPointerDownHandler, LobbyObserver
{
    [Tooltip("TopPanel�� �ٴ� UI��")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text goldTxt;

    //Ư�� Panel���� ���ε� -> Ư�� �г��� ������ 1���� ������ �ϴ� �г��̴� (�ش� Ư������ �����°� �������̴�)
    [SerializeField] private List<GameObject> PanelList; //�г��� ���� list

    private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>(); //�г��� key�� �ο��� ����
    private Queue<GameObject> PanelActiveQueue = new Queue<GameObject>(); //SetActive�� Queue�� �ְ� �� ���� ����� ������ 1���� ������ ����

    private void Start()
    {
        InitaDictionary();
        LoadUserInfo();
    }

    /// <summary>
    /// Online, Offline ������� Login �Ǿ��� ��, User�� Info�� �����´�
    /// Login�� PlayerPrefs�� Server�� ���������� ���� ���� ������
    /// PlayerPrefs���� user�� ������ �����ͼ� �ٿ��ش�.
    /// </summary>
    private void LoadUserInfo()
    {
        nameTxt.text = LocalData.GetInstance.Name.ToString();
        goldTxt.text = LocalData.GetInstance.Gold.ToString();
    }

    #region Observer Pattern - IObserver interface ����
    public void UpdateOpenPowerCard(PowerCardData cardData)
    {
        LoadUserInfo();
    }
    #endregion

    #region Panel Open�ϴ� Button���� �Լ���
    /// <summary>
    /// LobbyCanvase�� Bottom Panel�ؿ� Shop Button�� ���� ������
    /// Shop Button�� ������ Shop�� ����.
    /// </summary>
    public void ShopPanelOpen()
    {
        OpenPanel(UIType.ShopPanel);
    }

    /// <summary>
    /// LobbyCanvas�� Bottom Panel�ؿ� Inventory Button�� ���� ������
    /// Inventory Button�� ������ Inventory�� ����.
    /// </summary>
    public void InventoryPanelOpen()
    {
        OpenPanel(UIType.InventoryPanel);
    }

    /// <summary>
    /// LobbyCanvas�� Bottom Panel�ؿ� PlayMode Button�� ���� ������
    /// PlayMode Button�� ������ ȣ��ȴ�
    /// </summary>
    public void PlayModePanelOpen()
    {
        OpenPanel(UIType.PlayModePanel);
    }

    /// <summary>
    /// LobbyCanvas�� Top Panel�ؿ� Option Button�� ���� ������
    /// Option Button�� ������ ȣ��ȴ�
    /// </summary>
    public void OptionPanelOpen()
    {
        OpenPanel(UIType.OptionPanel);
    }
    #endregion

    #region Panel�� Button ��� �Լ���
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
    public void SaveUserInfo()
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
    /// </summary>
    public void ClearUserInfo()
    {
        LocalData.GetInstance.ClearAllUserInfo();
        SceneController.GetInstance.GoToScene("Login");
    }
    #endregion

    #region Always Panel��Ʈ�� �Լ���
    /// <summary>
    /// �ٸ� Panel�� �������¿��� AlwaysPanel tag�� �����ִ� ���� ��ġ�ϸ� Ȱ��ȭ�� ��� �г��� ������
    /// pointerEnter�������� �޾ƶ� ����Ͽ��� ��ġ �Է� �޴´�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter.gameObject.CompareTag("AlwaysPanel"))
        {
            OpenPanel(UIType.NonePanel);
        }
    }

    /// <summary>
    /// Lobby Scene ���Խ�, �г� ����
    /// </summary>
    public void InitaDictionary()
    {
        foreach (GameObject panel in PanelList)
        {
            switch (panel.gameObject.name)
            {
                case "Inventory Panel":
                    PanelDictionary.Add(UIType.InventoryPanel, panel);
                    break;
                case "PlayMode Panel":
                    PanelDictionary.Add(UIType.PlayModePanel, panel);
                    break;
                case "Option Panel":
                    PanelDictionary.Add(UIType.OptionPanel, panel);
                    break;
                case "Shop Panel":
                    PanelDictionary.Add(UIType.ShopPanel, panel);
                    break;
            }

            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// LobbyScene���� LobbyCanvas���� �Է��� �޾Ƽ� �ش� ���� �г��� ó�����ش�
    /// </summary>
    /// <param name="type"> Open�� Panel�� Type�� �޴´� </param>
    /// <param name="layer"> Canvas�� ��ġ�� �޾� �ڽ����� �ִ´� </param>
    public void OpenPanel(UIType type)
    {
        if (type == UIType.NonePanel)
        {
            if (PanelActiveQueue.Count > 0)
            {
                foreach (var panel in PanelActiveQueue)
                {
                    panel.gameObject.SetActive(false);
                }
                PanelActiveQueue.Clear();
            }
        }

        if (PanelDictionary.TryGetValue(type, out GameObject obj) && type != UIType.NonePanel)
        {
            if (PanelActiveQueue.Contains(obj))
            {
                obj.SetActive(false);
                PanelActiveQueue.Clear();
            }
            else
            {
                if (PanelActiveQueue.Count > 0)
                {
                    var removeObj = PanelActiveQueue.Peek();
                    removeObj.SetActive(false);
                    PanelActiveQueue.Dequeue();
                }

                PanelActiveQueue.Enqueue(obj);
                obj.SetActive(true);
            }
        }
    }

    #endregion
}
