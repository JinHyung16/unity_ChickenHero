using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HughGeneric;
using System;

sealed class LobbySceneManager : Singleton<LobbySceneManager>
{
    [SerializeField] private List<GameObject> PanelList; //������ 1���� ������ �� �г��� ���� list

    private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>(); //�г��� key�� �ο��� ����
    private Queue<GameObject> PanelActiveQueue = new Queue<GameObject>(); //SetActive�� Queue�� �ְ� �� ���� ����� ������ 1���� ������ ����

    private void Start()
    {
        InitaDictionary();
    }

    /// <summary>
    /// Lobby Scene ���Խ�, �г� ����
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
    /// LobbyScene���� LobbyCanvas���� �Է��� �޾Ƽ� �ش� ���� �г��� ó�����ش�
    /// </summary>
    /// <param name="type"> Open�� Panel�� Type�� �޴´� </param>
    /// <param name="layer"> Canvas�� ��ġ�� �޾� �ڽ����� �ִ´� </param>
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
    /// ������ 1���� ������ �ϴ� Panel ����
    /// ��, �ش� Panel�� ������ ������ Panel�� �ڵ����� ������ ���� �������� Panel ����
    /// </summary>
    public enum UIType
    { 
        InventoryPanel,
        PlayModePanel,
        OptionPanel,
    }
}
