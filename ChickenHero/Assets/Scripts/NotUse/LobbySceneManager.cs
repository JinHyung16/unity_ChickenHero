using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HughGeneric;
using System;

sealed class LobbySceneManager : Singleton<LobbySceneManager>
{
    [SerializeField] private List<GameObject> PanelList; //오로지 1개만 열려야 할 패널을 담을 list

    private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>(); //패널의 key를 부여해 저장
    private Queue<GameObject> PanelActiveQueue = new Queue<GameObject>(); //SetActive시 Queue에 넣고 맨 앞은 지우고 오로지 1개만 열리게 저장

    private void Start()
    {
        InitaDictionary();
    }

    /// <summary>
    /// Lobby Scene 진입시, 패널 세팅
    /// </summary>
    private void InitaDictionary()
    {
        Debug.Log("Init");

        foreach (GameObject panel in PanelList)
        {
            var obj = Instantiate(panel);
            switch (panel.gameObject.name)
            {
                case "Inventory Panel":
                    Debug.Log("Inven" + obj.name);
                    PanelDictionary.Add(UIType.InventoryPanel, obj);
                    break;
                case "PlayMode Panel":
                    Debug.Log("PlayMode" + obj.name);
                    PanelDictionary.Add(UIType.PlayModePanel, obj);
                    break;
                case "Option Panel":
                    Debug.Log("Option" + obj.name);
                    PanelDictionary.Add(UIType.OptionPanel, obj);
                    break;
            }
        }
    }

    /// <summary>
    /// LobbyScene에서 LobbyCanvas에서 입력을 받아서 해당 씬의 패널을 처리해준다
    /// </summary>
    /// <param name="type"> Open할 Panel의 Type을 받는다 </param>
    /// <param name="layer"> Canvas의 위치를 받아 자식으로 넣는다 </param>
    public void PopupPanel(UIType type, Transform layer)
    {
        if (PanelDictionary.TryGetValue(type, out GameObject obj))
        {
            if (PanelActiveQueue.Count > 0)
            {
                var removeObj = PanelActiveQueue.Peek();
                removeObj.transform.SetParent(null);
                removeObj.SetActive(false);
                PanelActiveQueue.Dequeue();
            }

            PanelActiveQueue.Enqueue(obj);
            obj.transform.SetParent(layer);
            obj.SetActive(true);
        }
    }


    /// <summary>
    /// 오로지 1개만 열려야 하는 Panel 모음
    /// 즉, 해당 Panel이 열리면 나머지 Panel은 자동으로 닫히는 서로 독립적인 Panel 모음
    /// </summary>
    public enum UIType
    { 
        InventoryPanel,
        PlayModePanel,
        OptionPanel,
    }
}
