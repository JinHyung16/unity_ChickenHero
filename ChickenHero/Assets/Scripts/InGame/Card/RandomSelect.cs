using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility.Observer;
using System;
using UnityEngine.UI;
using TMPro;

public class RandomSelect : MonoBehaviour, LobbySubject
{
    [SerializeField] private Button radomPickBtn;
    [SerializeField] private TMP_Text randomPickTxt;

    //���� PowerCardData�� ��� �ִ� ����Ʈ
    [SerializeField] private List<PowerCardData> PowerCardDataList = new List<PowerCardData>();

    private int powerCardDataWeightedTotal; //PowerCardData���� �� ����ġ ��

    [SerializeField] private GameObject powerCard; //PowerCard Canvas Object

    //Instantiate�� PowerCard ������Ʈ ���� Dictionary
    private Dictionary<string, GameObject> PowerCardDictionary = new Dictionary<string, GameObject>();

    private PowerCardData powerCardData; //���� PowerCardData ���� ����


    private IEnumerator DisplayPickableIEnum;

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

        pickCost = LocalData.GetInstance.PickCost;

        radomPickBtn.onClick.AddListener(RandomCardOpen);

        DisplayPickableIEnum = DisplaPickableCoroutine();
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

        StopCoroutine(DisplayPickableIEnum);
        DisplayPickableIEnum = null;
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
        if (pickCost <= LocalData.GetInstance.Gold)
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

            LocalData.GetInstance.Power = powerCardData.cardPower;
            LocalData.GetInstance.Gold -= pickCost;
            NotifyObservers();
        }
        else
        {
            StartCoroutine(DisplayPickableIEnum);
        }
    }

    private IEnumerator DisplaPickableCoroutine()
    {
        DisplaPickable(false);
        yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(0.5f);
        DisplaPickable(true);
    }

    private void DisplaPickable(bool pickable)
    {
        if (pickable)
        {
            randomPickTxt.text = "�Ŀ� �̱�\n" + pickCost;
            radomPickBtn.interactable = true;

            DisplayPickableIEnum = DisplaPickableCoroutine();
        }
        else
        {
            randomPickTxt.text = "��尡 �����մϴ�";
            radomPickBtn.interactable = false;
        }
    }

    #region Observer Pattern - ISubject interface ����

    [SerializeField] private LobbyCanvas LobbyCanvasObserver;
    [SerializeField] private ShopPanel ShopPanelObserver;
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
            RegisterObserver(ShopPanelObserver);
        }
        else
        {
            RemoveObserver(LobbyCanvasObserver);
            RemoveObserver(ShopPanelObserver);
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

    public void NotifyObservers()
    {
        foreach (var observer in ObserverList)
        {
            observer.UpdateOpenPowerCard(powerCardData);
        }
    }
    #endregion
}
