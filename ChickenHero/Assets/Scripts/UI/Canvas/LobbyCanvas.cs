using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Packet.GameServer;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEngine.EventSystems;

public class LobbyCanvas : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("TopPanel�� �ٴ� UI��")]
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text goldTxt;

    //Ư�� Panel���� ���ε� -> Ư�� �г��� ������ 1���� ������ �ϴ� �г��̴� (�ش� Ư������ �����°� �������̴�)
    [SerializeField] private List<GameObject> PanelList; //Ư�� �г��� ���� list

    private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>(); //�г��� key�� �ο��� ����
    private Queue<GameObject> PanelActiveQueue = new Queue<GameObject>(); //SetActive�� Queue�� �ְ� �� ���� ����� ������ 1���� ������ ����

    private void Start()
    {
        InitaDictionary();
        LoadUserInfo();
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

    /// <summary>
    /// middle panel �߰��Ͽ� ��ġ�� �޴� �Լ�
    /// �ٸ� Panel�� �������¿��� �� ������ ��ġ�ϸ� ��� �� �������� ����
    /// pointerEnter�� �ƴ϶� pointerClick���� �ϸ� ��ġ �޴� �ý����� �޶� �׷��� Null���´�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter.gameObject.CompareTag("ImportantPanel"))
        {
            OpenPanel(UIType.NonePanel);
        }
    }
    
    #region Ư�� Panel ��Ʈ���ϴ� Button �Լ�
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
    /// LobbyCanvas�� Bottom Panel�ؿ� Option Button�� ���� ������
    /// Option Button�� ������ ȣ��ȴ�
    /// </summary>
    public void OptionPanelOpen()
    {
        OpenPanel(UIType.OptionPanel);
    }
    #endregion

    #region Button�� ���� ������ Function
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

    #region Ư�� Panel���� �Լ���
    /// <summary>
    /// Lobby Scene ���Խ�, �г� ����
    /// </summary>
    private void InitaDictionary()
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
            }
        }
    }

    /// <summary>
    /// LobbyScene���� LobbyCanvas���� �Է��� �޾Ƽ� �ش� ���� �г��� ó�����ش�
    /// </summary>
    /// <param name="type"> Open�� Panel�� Type�� �޴´� </param>
    /// <param name="layer"> Canvas�� ��ġ�� �޾� �ڽ����� �ִ´� </param>
    private void OpenPanel(UIType type)
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


    /// <summary>
    /// ������ 1���� ������ �ϴ� Panel ����
    /// ��, �ش� Panel�� ������ ������ Panel�� �ڵ����� ������ ���� �������� Panel ����
    /// </summary>
    private enum UIType
    {
        NonePanel = 0,

        InventoryPanel,
        PlayModePanel,
        OptionPanel,
    }
    #endregion
}
