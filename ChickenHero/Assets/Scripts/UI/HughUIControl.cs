using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HughUI
{
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
    }
}
