using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility.Observer;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class RandomSelect : MonoBehaviour, LobbySubject
{
    [SerializeField] private ShopPanel ShopPanelScript;

    [SerializeField] private Button radomPickBtn;
    [SerializeField] private TMP_Text randomPickTxt;

    //���� PowerCardData�� ��� �ִ� ����Ʈ
    [SerializeField] private List<PowerCardData> PowerCardDataList = new List<PowerCardData>();

    private int powerCardDataWeightedTotal; //PowerCardData���� �� ����ġ ��

    [SerializeField] private GameObject powerCard; //PowerCard Canvas Object

    //Instantiate�� PowerCard ������Ʈ ���� Dictionary
    private Dictionary<string, GameObject> PowerCardDictionary = new Dictionary<string, GameObject>();

    private PowerCardData powerCardData; //���� PowerCardData ���� ����

    //Random Pick�� ���� �ʿ��� ���
    private int pickCost = 0;

    private void OnEnable()
    {
        InitRandomSelect();
        InitObserver(true);
    }

    /// <summary>
    /// ShopPanel�� ���� ��, �����ߴ� ī��� �� �ѹ��� �����ش�.
    /// GC ȣ���� ���̰��� �̷� ����� ä��
    /// </summary>
    private void OnDisable()
    {
        ResetRandomSelect();
        InitObserver(false);
    }

    /// <summary>
    /// ShopPanel���� ��, OnEnable���� ����Ǵ� �Լ�
    /// PowerCardData ����Ʈ���� �� card���� ���� �ִ� ����ġ���� ������ powerDeckTotalWeight�� �����صд�
    /// Random Pick Button�� ���ε� ���̴�
    /// </summary>
    private void InitRandomSelect()
    {
        foreach (var card in PowerCardDataList)
        {
            //PowerCard GameObject�� Canvas������ �ִ� Object�� PowerCard.cs�� �پ��־ InChildren���� ȣ���ؾ���
            powerCardDataWeightedTotal += card.cardWeighted;
        }

        pickCost = DataManager.GetInstance.PickCost;

        radomPickBtn.onClick.AddListener(RandomCardOpen);
    }

    /// <summary>
    /// ShopPanel�� OnDisable �� �� ����Ǵ� �Լ�
    /// 0 �Ǵ� null�� reset�� �Ǿ�� �ϴ� ������ �ʱ�ȭ�ϰ� �ִ�.
    /// </summary>
    private void ResetRandomSelect()
    {
        foreach (var temp in PowerCardDictionary.Values)
        {
            Destroy(temp);
        }
        PowerCardDictionary.Clear();

        powerCardDataWeightedTotal = 0;
        radomPickBtn.onClick.RemoveListener(RandomCardOpen);
    }

    /// <summary>
    /// ����ġ ���� �̱⸦ �����ϴ� �ٽ� �κ�
    /// selectNum�� total���� Random.Range ���� ���صΰ� weights�� card���� weight�� ���ذ��鼭
    /// selectNum���� ũ�ų� ������ �ش� PowerCardData�� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns> ���õ� PowerCardData ��</returns>
    private PowerCardData RandomPowerCard()
    {
        int weighted = 0;
        int selectTotalWeight = 0;

        selectTotalWeight = Mathf.RoundToInt(powerCardDataWeightedTotal * UnityEngine.Random.Range(0.0f, 1.0f));

        foreach (var card in PowerCardDataList)
        {
            weighted += card.cardWeighted;
            if (selectTotalWeight <= weighted)
            {
                return card;
            }
        }

        return null;
    }

    /// <summary>
    /// ShopPanel���� �̱� ��ư�� ������ ������ ī�带 Open�ϰ�
    /// �ش� PowerCardData�� PowerCard UI�� �����͸� ���ε����ش�.
    /// Dictionary�� �����صδµ�, ������ ���� ������ �� �ִ� ���� �״�� �ٽ� �����ش�.
    /// </summary>
    public void RandomCardOpen()
    {
        if (pickCost <= DataManager.GetInstance.Gold)
        {
            powerCardData = RandomPowerCard();
            string name = powerCardData.powerCardName;

            if (!PowerCardDictionary.ContainsKey(name) || PowerCardDictionary.Count <= 0)
            {
                PowerCardDictionary.Add(name, Instantiate(powerCard, null));
            }

            if (PowerCardDictionary.TryGetValue(name, out GameObject obj))
            {
                obj.GetComponent<PowerCard>().SetPowerCard(powerCardData);
                obj.SetActive(true);
            }

            DataManager.GetInstance.Gold -= pickCost;
            ShopPanelScript.SetUpgradeLevel(name);
            NotifyObservers(LobbyNotifyType.OpenCard);
        }
        else
        {
            DisplayPowerCardPickable().Forget();
        }
    }

    private async UniTaskVoid DisplayPowerCardPickable()
    {
        DisplaPickable(false);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: this.GetCancellationTokenOnDestroy());
        DisplaPickable(true);
    }

    private void DisplaPickable(bool pickable)
    {
        if (pickable)
        {
            randomPickTxt.text = "�Ŀ� �̱�\n" + pickCost;
            radomPickBtn.interactable = true;
        }
        else
        {
            randomPickTxt.text = "��尡 �����մϴ�";
            radomPickBtn.interactable = false;
        }
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
        if (lobbyNotifyType == LobbyNotifyType.OpenCard)
        {
            foreach (var observer in ObserverList)
            {
                observer.UpdateOpenPowerCard(powerCardData);
            }
        }
    }
    #endregion
}
