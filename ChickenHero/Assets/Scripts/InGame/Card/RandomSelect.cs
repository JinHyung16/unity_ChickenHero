using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility.Observer;
using System;

public class RandomSelect : MonoBehaviour
{
    //뽑을 PowerCardData를 담고 있는 리스트
    [SerializeField] private List<PowerCardData> PowerCardDataList = new List<PowerCardData>();

    private int powerCardDataWeightedTotal; //PowerCardData들의 총 가중치 합

    [SerializeField] private GameObject powerCard; //PowerCard Canvas Object

    private Dictionary<string, GameObject> PowerCardDictionary = new Dictionary<string, GameObject>();
    private PowerCardData powerCardData;

    private void OnEnable()
    {
        InitPowerCardData();
    }

    /// <summary>
    /// ShopPanel이 닫힐 때, 오픈했던 카드들 다 한번에 지워준다.
    /// GC 호출을 줄이고자 이런 방식을 채택
    /// </summary>
    private void OnDisable()
    {
        if (PowerCardDictionary.Count > 0)
        {
            foreach (var temp in PowerCardDictionary.Values)
            {
                Destroy(temp);
            }
        }
    }

    /// <summary>
    /// PowerCardData 리스트에서 각 card들이 갖고 있는 가중치들을 가져와 powerDeckTotalWeight에 저장해둔다
    /// </summary>
    private void InitPowerCardData()
    {
        foreach (var card in PowerCardDataList)
        {
            //PowerCard GameObject가 Canvas하위에 있는 Object에 PowerCard.cs가 붙어있어서 InChildren으로 호출해야함
            powerCardDataWeightedTotal += card.cardWeighted;
        }
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
        int referenceWeighted = 0;

        referenceWeighted = Mathf.RoundToInt(powerCardDataWeightedTotal * UnityEngine.Random.Range(0.0f, 1.0f));

        foreach (var card in PowerCardDataList)
        {
            weighted += card.cardWeighted;
            if (referenceWeighted <= weighted)
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
        powerCardData = RandomPowerCard();
        string name = powerCardData.name;

        if (!PowerCardDictionary.ContainsKey(name) || PowerCardDictionary.Count <= 0)
        {
            PowerCardDictionary.Add(name, Instantiate(powerCard, null));
        }

        if (PowerCardDictionary.TryGetValue(name, out GameObject obj))
        {
            obj.GetComponent<PowerCard>().SetPowerCard(powerCardData);
            obj.SetActive(true);
        }
    }
}
