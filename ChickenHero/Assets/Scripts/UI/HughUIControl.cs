using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HughUI
{

    public class HughUIControl : MonoBehaviour, IPointerDownHandler
    {

        public virtual void OnPointerDown(PointerEventData eventData) { }

        public virtual void InitaDictionary() { }

        public virtual void OpenPanel(UIType type) { }
    }

    /// <summary>
    /// 오로지 1개만 열려야 하는 Panel 모음
    /// 즉, 해당 Panel이 열리면 나머지 Panel은 자동으로 닫히는 서로 독립적인 Panel 모음
    /// </summary>
    public enum UIType
    {
        NonePanel = 0,

        InventoryPanel,
        PlayModePanel,
        OptionPanel,
        ShopPanel,

        PowerCardCanvas = 10,
    }
}
