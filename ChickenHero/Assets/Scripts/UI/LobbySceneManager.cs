using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HughGeneric;
using System;

sealed class LobbySceneManager : Singleton<LobbySceneManager>
{
    private Dictionary<UIType, GameObject> PanelDictionary;

    [SerializeField] private List<GameObject> PanelList;

    private void Start()
    {
        InitaDictionary();
    }

    private void InitaDictionary()
    {
        Debug.Log("Init");
        foreach (GameObject panel in PanelList)
        {
            switch (panel.name)
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

    public void PushPopup(UIType type, Transform layer)
    {
        if (PanelDictionary.ContainsKey(type) == false)
        {
#if UNITY_EDITOR
            Debug.Log("<color=red><br> have not exist panel in PanelDictionary </br></color>");
#endif
            return;
        }

        if (PanelDictionary.TryGetValue(type, out GameObject obj))
        {
            Debug.Log("불러오기");
            foreach (var panel in PanelDictionary)
            {
                if (obj == panel.Value)
                {
                    obj.transform.SetParent(layer);
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(null);
                }
            }
        }
    }

    public enum UIType
    {
        NoneUI = 0,
        
        InventoryPanel,
        PlayModePanel,
        OptionPanel,
    }
}
