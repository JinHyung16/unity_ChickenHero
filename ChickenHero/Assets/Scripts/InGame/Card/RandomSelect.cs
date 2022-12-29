using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility.Observer;
using System;

public class RandomSelect : MonoBehaviour
{
    //���� PowerCardData�� ��� �ִ� ����Ʈ
    [SerializeField] private List<PowerCardData> PowerCardDataList = new List<PowerCardData>();

    private int powerCardDataWeightedTotal; //PowerCardData���� �� ����ġ ��

    [SerializeField] private GameObject powerCard; //PowerCard Canvas Object

    private Dictionary<string, GameObject> PowerCardDictionary = new Dictionary<string, GameObject>();
    private PowerCardData powerCardData;

    private void OnEnable()
    {
        InitPowerCardData();
    }

    /// <summary>
    /// ShopPanel�� ���� ��, �����ߴ� ī��� �� �ѹ��� �����ش�.
    /// GC ȣ���� ���̰��� �̷� ����� ä��
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
    /// PowerCardData ����Ʈ���� �� card���� ���� �ִ� ����ġ���� ������ powerDeckTotalWeight�� �����صд�
    /// </summary>
    private void InitPowerCardData()
    {
        foreach (var card in PowerCardDataList)
        {
            //PowerCard GameObject�� Canvas������ �ִ� Object�� PowerCard.cs�� �پ��־ InChildren���� ȣ���ؾ���
            powerCardDataWeightedTotal += card.cardWeighted;
        }
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
    /// ShopPanel���� �̱� ��ư�� ������ ������ ī�带 Open�ϰ�
    /// �ش� PowerCardData�� PowerCard UI�� �����͸� ���ε����ش�.
    /// Dictionary�� �����صδµ�, ������ ���� ������ �� �ִ� ���� �״�� �ٽ� �����ش�.
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
