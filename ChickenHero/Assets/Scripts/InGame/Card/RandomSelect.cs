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

    //뽑을 PowerCardData를 담고 있는 리스트
    [SerializeField] private List<PowerCardData> PowerCardDataList = new List<PowerCardData>();

    private int powerCardDataWeightedTotal; //PowerCardData들의 총 가중치 합

    [SerializeField] private GameObject powerCard; //PowerCard Canvas Object

    //Instantiate한 PowerCard 오브젝트 담을 Dictionary
    private Dictionary<string, GameObject> PowerCardDictionary = new Dictionary<string, GameObject>();

    private PowerCardData powerCardData; //뽑은 PowerCardData 담을 변수


    private IEnumerator DisplayPickableIEnum;

    //Random Pick을 위해 필요한 비용
    private int pickCost = 0;

    private void OnEnable()
    {
        InitRandomSelect();
        InitObserver(true);
    }

    /// <summary>
    /// ShopPanel이 닫힐 때, 오픈했던 카드들 다 한번에 지워준다.
    /// GC 호출을 줄이고자 이런 방식을 채택
    /// </summary>
    private void OnDisable()
    {
        ResetRandomSelect();
        InitObserver(false);
    }

    /// <summary>
    /// ShopPanel열릴 때, OnEnable에서 실행되는 함수
    /// PowerCardData 리스트에서 각 card들이 갖고 있는 가중치들을 가져와 powerDeckTotalWeight에 저장해둔다
    /// Random Pick Button도 바인딩 중이다
    /// </summary>
    private void InitRandomSelect()
    {
        foreach (var card in PowerCardDataList)
        {
            //PowerCard GameObject가 Canvas하위에 있는 Object에 PowerCard.cs가 붙어있어서 InChildren으로 호출해야함
            powerCardDataWeightedTotal += card.cardWeighted;
        }

        pickCost = LocalData.GetInstance.PickCost;

        radomPickBtn.onClick.AddListener(RandomCardOpen);

        DisplayPickableIEnum = DisplaPickableCoroutine();
    }

    /// <summary>
    /// ShopPanel이 OnDisable 될 때 실행되는 함수
    /// 0 또는 null로 reset이 되어야 하는 값들을 초기화하고 있다.
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
    /// 가중치 랜덤 뽑기를 진행하는 핵심 부분
    /// selectNum에 total값의 Random.Range 값을 곱해두고 weights에 card들의 weight를 더해가면서
    /// selectNum보다 크거나 같으면 해당 PowerCardData를 반환한다.
    /// </summary>
    /// <returns> 선택된 PowerCardData 값</returns>
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
    /// ShopPanel에서 뽑기 버튼을 누르면 실제로 카드를 Open하고
    /// 해당 PowerCardData를 PowerCard UI에 데이터를 바인딩해준다.
    /// Dictionary에 저장해두는데, 없으면 저장 있으면 그 있는 값을 그대로 다시 보여준다.
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
            randomPickTxt.text = "파워 뽑기\n" + pickCost;
            radomPickBtn.interactable = true;

            DisplayPickableIEnum = DisplaPickableCoroutine();
        }
        else
        {
            randomPickTxt.text = "골드가 부족합니다";
            radomPickBtn.interactable = false;
        }
    }

    #region Observer Pattern - ISubject interface 구현

    [SerializeField] private LobbyCanvas LobbyCanvasObserver;
    [SerializeField] private ShopPanel ShopPanelObserver;
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
