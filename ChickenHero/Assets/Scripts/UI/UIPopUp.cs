using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopUp : MonoBehaviour
{
    private List<GameObject> PopupList = new List<GameObject>();


    public void PushPopup(GameObject obj)
    {
        PopupList.Add(obj);
        foreach (var ui in PopupList)
        {
            if (obj == ui)
            {
                ui.SetActive(true);
            }
            else
            {
                ui.SetActive(false);
                PopupList.Remove(ui);
            }
        }
    }
}
