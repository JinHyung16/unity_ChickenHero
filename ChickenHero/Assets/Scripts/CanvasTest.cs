using HughUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasTest : UISubject
{
    [Tooltip("Bottom Panel�� �ٴ� UI��")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject playModePanel;
    [SerializeField] private GameObject optionPanel;

    [SerializeField] private Toggle inventoryToggle;
    [SerializeField] private Toggle playModeSelectToggle;
    [SerializeField] private Toggle optionToggle;


    public override void RegisterObserver(Observer observer)
    {

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerClick.gameObject.name == "InventoryToggleButton")
        {
        }
        else
        {
        }
    }
}
