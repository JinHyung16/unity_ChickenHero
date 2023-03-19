using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradePowerLevelTxt; //���� power �ܰ� �����ִ� text
    [SerializeField] private TMP_Text goldTxt; //���׷��̵�� �ʿ��� ��差 �����ִ� text

    [SerializeField] private int upgradePowerLevel = 1;
    [SerializeField] private int upgradeGold = 0; //���׷��̵�� �ʿ��� ���

    private int ownUpgradePower; //������ upgrade�� �ܰ� ������ �� ���

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
        ownUpgradePower = LocalDataManager.GetInstance.UpgradeLevel;
        upgradeGold = LocalDataManager.GetInstance.GetUpgradeCost(LocalDataManager.GetInstance.UpgradeLevel.ToString());
    }

    public void PowerUp()
    {
        if (upgradeGold <= LocalDataManager.GetInstance.Gold)
        {
            upgradePowerLevel++;
            LocalDataManager.GetInstance.UpgradeLevel += 1;

            LocalDataManager.GetInstance.Power += 1;
            upgradeGold = LocalDataManager.GetInstance.GetUpgradeCost(LocalDataManager.GetInstance.UpgradeLevel.ToString());

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

    #region Observer Pattern - ISubject interface ����

    [SerializeField] private LobbyCanvas LobbyCanvasObserver;
    private List<LobbyObserver> ObserverList = new List<LobbyObserver>(); //Objserver�� ������ List

    /// <summary>
    /// Observer���� Observer List�� �����صδ� �Լ�
    /// parameter�� ���� OnEable / OnDisable�� ����ϴ°� �ٸ���
    /// </summary>
    /// <param name="register"> ��� �Ǵ� �������θ� ���Ѵٴ� parameter </param>
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
