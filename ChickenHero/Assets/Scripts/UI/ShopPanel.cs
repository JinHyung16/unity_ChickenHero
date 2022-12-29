using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour, IObserver
{
    [SerializeField] private TMP_Text upgradaePowerTxt; //현재 power 단계 보여주는 text
    [SerializeField] private TMP_Text goldTxt; //업그레이드시 필요한 골드량 보여주는 text

    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //업그레이드시 필요한 골드

    private int ownUpgradePower; //본인이 upgrade한 단계 저장한 값 담기
    private int ownGold; //본인이 소지하고 있는 돈 저장한 값 담기

    private void OnEnable()
    {
        InitShopPanel();
    }

    private void InitShopPanel()
    {
        ownUpgradePower = LocalData.GetInstance.Power;
        ownGold = LocalData.GetInstance.Gold;
        upgradeGold = 100;
    }

    public void PowerUp()
    {
        if (upgradeGold <= ownGold)
        {
            upgradePower++;
            upgradeGold++;

            DisplayUpdate();
            LocalData.GetInstance.Power += upgradePower;
        }
        else
        {
            return;
        }
    }

    private void DisplayUpdate()
    {
        upgradaePowerTxt.text = upgradePower.ToString();
        goldTxt.text = upgradeGold.ToString() + "G";
    }

    #region Observer Pattern - IObserver interface 구현
    public void UpdateOpenPowerCard(PowerCardData cardData)
    {
        switch (cardData.powerCardName)
        {
            case "F":
                upgradePower = 1;
                break;
            default:
                upgradePower += cardData.cardPower;
                break;
        }

        DisplayUpdate();
    }
    #endregion
}
