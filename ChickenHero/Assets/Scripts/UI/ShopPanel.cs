using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour, IObserver
{
    [SerializeField] private TMP_Text upgradaePowerTxt; //���� power �ܰ� �����ִ� text
    [SerializeField] private TMP_Text goldTxt; //���׷��̵�� �ʿ��� ��差 �����ִ� text

    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //���׷��̵�� �ʿ��� ���

    private int ownUpgradePower; //������ upgrade�� �ܰ� ������ �� ���

    private void OnEnable()
    {
        InitShopPanel();
    }

    private void InitShopPanel()
    {
        ownUpgradePower = LocalData.GetInstance.Power;
        upgradeGold = 100;
    }

    public void PowerUp()
    {
        if (upgradeGold <= LocalData.GetInstance.Gold)
        {
            upgradePower++;
            upgradeGold++;

            DisplayUpdate();
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

        LocalData.GetInstance.Power = upgradePower;
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
