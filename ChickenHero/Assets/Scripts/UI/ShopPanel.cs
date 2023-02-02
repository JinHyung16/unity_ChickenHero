using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradePowerLevelTxt; //현재 power 단계 보여주는 text
    [SerializeField] private TMP_Text goldTxt; //업그레이드시 필요한 골드량 보여주는 text

    [SerializeField] private int upgradePowerLevel = 1;
    [SerializeField] private int upgradeGold = 0; //업그레이드시 필요한 골드

    private int ownUpgradePower; //본인이 upgrade한 단계 저장한 값 담기

    private void OnEnable()
    {
        InitShopPanel();
        InitObserver(true);
    }

    private void OnDisable()
    {
        InitObserver(false);
    }

    private void InitShopPanel()
    {
        ownUpgradePower = LocalData.GetInstance.UpgradeLevel;
        upgradeGold = LocalData.GetInstance.GetUpgradeCost(LocalData.GetInstance.UpgradeLevel.ToString());
    }

    public void PowerUp()
    {
        if (upgradeGold <= LocalData.GetInstance.Gold)
        {
            upgradePowerLevel++;
            LocalData.GetInstance.UpgradeLevel += 1;

            LocalData.GetInstance.Power += 1;
            upgradeGold = LocalData.GetInstance.GetUpgradeCost(LocalData.GetInstance.UpgradeLevel.ToString());

            DisplayUpdate();
            NotifyObservers(LobbyNotifyType.UpgradePower);
        }
    }

    private void DisplayUpdate()
    {
        upgradePowerLevelTxt.text = upgradePowerLevel.ToString();
        goldTxt.text = upgradeGold.ToString() + "G";
    }

    public void SetUpgradeLevel(string cardName)
    {
        switch (cardName)
        {
            case "F":
                upgradePowerLevel = 1;
                break;
            default:
                break;
        }
        DisplayUpdate();
    }

    #region Observer Pattern - ISubject interface 구현

    [SerializeField] private LobbyCanvas LobbyCanvasObserver;
    private List<LobbyObserver> ObserverList = new List<LobbyObserver>(); //Objserver들 저장할 List

    /// <summary>
    /// Observer들을 Observer List에 저장해두는 함수
    /// parameter에 따라 OnEable / OnDisable에 사용하는게 다르다
    /// </summary>
    /// <param name="register"> 등록 또는 해제여부를 판한다는 parameter </param>
    private void InitObserver(bool register)
    {
        if (register)
        {
            RegisterObserver(LobbyCanvasObserver);
        }
        else
        {
            RemoveObserver(LobbyCanvasObserver);
        }
    }


    public void RegisterObserver(LobbyObserver observer)
    {
        this.ObserverList.Add(observer);
    }

    public void RemoveObserver(LobbyObserver observer)
    {
        this.ObserverList.Remove(observer);
    }

    public void NotifyObservers(LobbyNotifyType lobbyNotifyType)
    {
        if (lobbyNotifyType == LobbyNotifyType.UpgradePower)
        {
            foreach (var observer in ObserverList)
            {
                observer.UpdatePowerUp();
            }
        }
    }
    #endregion
}
