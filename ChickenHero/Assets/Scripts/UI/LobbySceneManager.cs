using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HughGeneric;
using System;

public class LobbySceneManager : Singleton<LobbySceneManager>, IDisposable
{
    [SerializeField] private Dictionary<UIType, GameObject> PanelDictionary = new Dictionary<UIType, GameObject>();

    public void Dispose()
    {
        if (GameManager.GetInstance.noticeType != NoticeType.Lobby)
        {
            GC.SuppressFinalize(this);
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
            foreach (var panel in PanelDictionary)
            {
                panel.Value.SetActive(false);
                if (obj == panel.Value)
                {
                    panel.Value.transform.SetParent(layer);
                    panel.Value.SetActive(true);
                }
            }
        }
    }

    public enum UIType
    {
        NoneUI = 0,
        
        InventoryUI,
        PlayModeUI,
        OptionUI,
    }
}
