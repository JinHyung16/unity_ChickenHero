using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeLevelTxt; //현재 power 단계 보여주는 text
    [SerializeField] private TMP_Text goldTxt; //업그레이드시 필요한 골드량 보여주는 text

    [SerializeField] private int upgradeLevel = 1;
    [SerializeField] private int upgradePower = 1;
    [SerializeField] private int upgradeGold = 100; //업그레이드시 필요한 골드

    private int ownUpgradeLevel; //본인이 upgrade한 단계 저장한 값 담기
    private int ownGold; //본인이 소지하고 있는 돈 저장한 값 담기
    
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
    /// 랜덤 뽑기 버튼을 눌렀을 때, 실행하는 함수
    /// </summary>
    public void RandomPick()
    {
        randomSelectObj.RandomCardOpen();
    }
}
