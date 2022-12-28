using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradaePowerTxt; //���� power �ܰ� �����ִ� text
    [SerializeField] private TMP_Text goldTxt; //���׷��̵�� �ʿ��� ��差 �����ִ� text

    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //���׷��̵�� �ʿ��� ���

    private int ownUpgradePower; //������ upgrade�� �ܰ� ������ �� ���
    private int ownGold; //������ �����ϰ� �ִ� �� ������ �� ���


    [SerializeField] private RandomSelect randomSelect;

    private void OnEnable()
    {
        InitShop();
    }

    private void InitShop()
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

    /// <summary>
    /// ���� �̱� ��ư�� ������ ��, �����ϴ� �Լ�
    /// </summary>
    public void RandomPick()
    {
        randomSelect.RandomCardOpen();
    }
}
