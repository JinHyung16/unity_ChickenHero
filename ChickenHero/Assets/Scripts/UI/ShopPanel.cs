using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeLevelTxt; //���� power �ܰ� �����ִ� text
    [SerializeField] private TMP_Text goldTxt; //���׷��̵�� �ʿ��� ��差 �����ִ� text

    [SerializeField] private int upgradeLevel = 1;
    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //���׷��̵�� �ʿ��� ���

    private int ownUpgradeLevel; //������ upgrade�� �ܰ� ������ �� ���
    private int ownGold; //������ �����ϰ� �ִ� �� ������ �� ���
    
    public RandomSelect randomSelectObj;

    private void OnEnable()
    {
        InitShop();
    }

    private void InitShop()
    {
        ownUpgradeLevel = LocalData.GetInstance.UpgradeLevel;
        ownGold = LocalData.GetInstance.Gold;
        upgradeGold = 100;
    }

    public void PowerUp()
    {
        if (upgradeGold <= ownGold)
        {
            upgradeLevel++;
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
        upgradeLevelTxt.text = "Lv" + upgradeLevel.ToString();
        goldTxt.text = upgradeGold.ToString() + "G";
    }

    /// <summary>
    /// ���� �̱� ��ư�� ������ ��, �����ϴ� �Լ�
    /// </summary>
    public void RandomPick()
    {
        randomSelectObj.RandomCardOpen();
    }
}
