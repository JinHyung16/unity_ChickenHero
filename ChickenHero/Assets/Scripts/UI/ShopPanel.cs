using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour, LobbyObserver
{
    [SerializeField] private TMP_Text upgradaePowerTxt; //���� power �ܰ� �����ִ� text
    [SerializeField] private TMP_Text goldTxt; //���׷��̵�� �ʿ��� ��差 �����ִ� text

    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 0; //���׷��̵�� �ʿ��� ���

    private int ownUpgradePower; //������ upgrade�� �ܰ� ������ �� ���

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
            LocalData.GetInstance.UpgradeLevel += 1;

            LocalData.GetInstance.Power += upgradePower;
            upgradeGold = LocalData.GetInstance.GetUpgradeCost(LocalData.GetInstance.UpgradeLevel.ToString());
            DisplayUpdate();
        }
    }

    private void DisplayUpdate()
    {
        upgradaePowerTxt.text = upgradePower.ToString();
        goldTxt.text = upgradeGold.ToString() + "G";
    }

    #region Observer Pattern - IObserver interface ����

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
