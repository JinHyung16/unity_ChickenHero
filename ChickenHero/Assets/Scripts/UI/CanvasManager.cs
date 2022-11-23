using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour
{
    [HideInInspector]
    public bool isActive = false;

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        isActive = true;
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
        isActive = false;

    }

    public void TogglePanel()
    {
        if (isActive)
        {
            this.HidePanel();
        }
        else
        {
            this.ShowPanel();
        }
    }

    public void ToggleInGamePanel()
    {
        if (isActive)
        {
            this.HidePanel();
        }
        else
        {
            this.ShowPanel();
        }
    }
}
