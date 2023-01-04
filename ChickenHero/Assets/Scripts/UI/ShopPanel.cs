using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour, LobbyObserver
{
    [SerializeField] private TMP_Text upgradaePowerTxt; //현재 power 단계 보여주는 text
    [SerializeField] private TMP_Text goldTxt; //업그레이드시 필요한 골드량 보여주는 text

    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //업그레이드시 필요한 골드

    private int ownUpgradePower; //본인이 upgrade한 단계 저장한 값 담기

    private void OnEnable()
    {
        InitShopPanel();
    }

    private void InitShopPanel()
    {
        ownUpgradePower = LocalData.GetInstance.Power;
        upgradeGold = LocalData.GetInstance.GetUpgradeCost(LocalData.GetInstance.UpgradeLevel.ToString());
    }

    public void PowerUp()
    {
        if (upgradeGold <= LocalData.GetInstance.Gold)
        {
            upgradePower++;
            LocalData.GetInstance.UpgradeLevel++;

            LocalData.GetInstance.Power += upgradePower;
            DisplayUpdate();
        }
        else
        {
            return;
        }
        upgradeGold = LocalData.GetInstance.GetUpgradeCost(LocalData.GetInstance.UpgradeLevel.ToString());
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
