using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Packet.GameServer;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using UnityEngine.EventSystems;
using HughUI; //UIType 사용을 위해
using HughUtility.Observer;
using Cysharp.Threading.Tasks;

public class LobbyCanvas : MonoBehaviour, IPointerDownHandler, LobbyObserver
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text goldTxt;
    [SerializeField] private TMP_Text powerTxt;

    //특수 Panel관련 바인딩 -> 특수 패널은 오로지 1개만 열려야 하는 패널이다 (해당 특수끼린 열리는게 독립적이다)
    [SerializeField] private List<GameObject> PanelList; //패널을 담은 list

    private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>(); //패널의 key를 부여해 저장
    private Queue<GameObject> PanelActiveQueue = new Queue<GameObject>(); //SetActive시 Queue에 넣고 맨 앞은 지우고 오로지 1개만 열리게 저장

    //PlayMode Button UI
    [SerializeField] private Button singlePlayBtn;
    [SerializeField] private Button multiPlayBtn;

    [SerializeField] private GameObject ServerConnectCheckPanel; //서버 연결이 안되어있을 때, 띄울 공지창

    private void Awake()
    {
        if (LocalData.GetInstance.Name == null)
        {
            LocalData.GetInstance.Name = GameManager.GetInstance.curUserName;
        }
    }
    private void Start()
    {
        InitaDictionary();
        LoadUserInfoDisplay();

        singlePlayBtn.onClick.AddListener(GoToSinglePlay);
        multiPlayBtn.onClick.AddListener(GoToMultiPlay);
    }

    /// <summary>
    /// Online, Offline 상관없이 Login 되었을 때, User의 Info를 가져온다
    /// Login시 PlayerPrefs와 Server에 저장했으니 둘이 같은 정보로
    /// PlayerPrefs에서 user의 정보를 꺼내와서 붙여준다.
    /// </summary>
    private void LoadUserInfoDisplay()
    {
        nameTxt.text = LocalData.GetInstance.Name.ToString();
        goldTxt.text = "G: " + LocalData.GetInstance.Gold.ToString();
        powerTxt.text = "P: " + LocalData.GetInstance.Power.ToString();
    }

    #region Panel Open하는 Button관련 함수들
    /// <summary>
    /// LobbyCanvase의 Bottom Panel밑에 Shop Button에 직접 연결중
    /// Shop Button을 누르면 Shop을 연다.
    /// </summary>
    public void ShopPanelOpen()
    {
        OpenPanel(UIType.ShopPanel);
    }

    /// <summary>
    /// LobbyCanvas의 Bottom Panel밑에 Inventory Button에 직접 연결중
    /// Inventory Button을 누르면 Inventory를 연다.
    /// </summary>
    public void InventoryPanelOpen()
    {
        OpenPanel(UIType.InventoryPanel);
    }

    /// <summary>
    /// LobbyCanvas의 Bottom Panel밑에 PlayMode Button에 직접 연결중
    /// PlayMode Button을 누르면 호출된다
    /// </summary>
    public void PlayModePanelOpen()
    {
        OpenPanel(UIType.PlayModePanel);
    }

    /// <summary>
    /// LobbyCanvas의 Top Panel밑에 Option Button에 직접 연결중
    /// Option Button을 누르면 호출된다
    /// </summary>
    public void OptionPanelOpen()
    {
        OpenPanel(UIType.OptionPanel);
    }
    #endregion

    #region Panel내 Button 기능 함수들
    /// <summary>
    /// PlayMode Panel 밑 MultiPlay Button에 직접 연결중
    /// MultiPlay Button을 누르면 매칭을 진행한다.
    /// </summary>
    public void GoToMultiPlay()
    {
        if (GameServer.GetInstance.GetIsServerConnect())
        {
            GameManager.GetInstance.isSingleplay = false;
            SceneController.GetInstance.GoToScene("MultiPlay").Forget();
        }
        else
        {
            LoginCheckPanelUpdate().Forget();
        }
    }
    private async UniTaskVoid LoginCheckPanelUpdate()
    {
        ServerConnectCheckPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        ServerConnectCheckPanel.SetActive(false);
    }

    /// <summary>
    /// PlayMode Panel 밑 SinglePlay Button에 직접 연결중
    /// SinglePlay Button을 누르면 싱글 플레이를 진행한다.
    /// </summary>
    public void GoToSinglePlay()
    {
        GameManager.GetInstance.isSingleplay = true;
        SceneController.GetInstance.GoToScene("SinglePlay").Forget();
    }

    /// <summary>
    /// Option Panel 밑 Save Button에 직접 연결중
    /// Save Button을 누르면 유저 정보를 저장한다.
    /// 서버가 연결되어 있으면 DB서버에 유저 정보 갱신한다.
    /// </summary>
    public void SaveUserInfo()
    {
        string name = nameTxt.text;
        int gold = int.Parse(goldTxt.text);

        LocalData.GetInstance.Name = name;
        LocalData.GetInstance.Gold = gold;

        if (GameServer.GetInstance.GetIsServerConnect())
        {
            GameServer.GetInstance.SaveUserInfoServer(name, gold);
        }
    }

    /// <summary>
    /// Option Panel 밑 Clear Button에 직접 연결중
    /// Clear Button을 누르면 유저 정보를 삭제한다.
    /// </summary>
    public void ClearUserInfo()
    {
        LocalData.GetInstance.ClearAllUserInfo();
        SceneController.GetInstance.GoToScene("Login").Forget();
    }
    #endregion

    #region Always Panel컨트롤 함수들
    /// <summary>
    /// 다른 Panel이 열린상태에서 AlwaysPanel tag를 갖고있는 곳을 터치하면 활성화된 모든 패널이 닫힌다
    /// pointerEnter형식으로 받아라 모바일에서 터치 입력 받는다
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
    /// Lobby Scene 진입시, 패널 세팅
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
    /// LobbyScene에서 LobbyCanvas에서 입력을 받아서 해당 씬의 패널을 처리해준다
    /// </summary>
    /// <param name="type"> Open할 Panel의 Type을 받는다 </param>
    /// <param name="layer"> Canvas의 위치를 받아 자식으로 넣는다 </param>
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

    #region Observer Pattern - PowerCardObserver interface 구현
    public void UpdateOpenPowerCard(PowerCardData cardData)
    {
        switch (cardData.powerCardName)
        {
            case "F":
                LocalData.GetInstance.Power = cardData.cardPower;
                break;
            default:
                LocalData.GetInstance.Power += cardData.cardPower;
                break;
        }
        LoadUserInfoDisplay();
    }

    public void UpdatePowerUp()
    {
        LoadUserInfoDisplay();
    }
    #endregion
}
