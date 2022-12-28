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
    private PowerCardData powerCardData;

    private void OnEnable()
    {
        InitPowerCardData();
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

    public void RandomCardOpen()
    {
        this.powerCardData = RandomPowerCard();

        var card = Instantiate(powerCard);
        card.SetActive(false);
        card.GetComponent<PowerCard>().SetPowerCard(this.powerCardData);
        card.SetActive(true);
    }
}
