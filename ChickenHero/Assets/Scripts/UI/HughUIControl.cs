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
    /// ������ 1���� ������ �ϴ� Panel ����
    /// ��, �ش� Panel�� ������ ������ Panel�� �ڵ����� ������ ���� �������� Panel ����
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
