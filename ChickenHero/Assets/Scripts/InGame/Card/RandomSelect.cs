using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelect : MonoBehaviour
{
    public List<GameObject> PowerDeck = new List<GameObject>();
    [SerializeField] private int powerDeckTotalWeight;

    private GameObject pickCard;


    private void OnEnable()
    {
        foreach (var card in PowerDeck)
        {
            //PowerCard GameObject가 Canvas하위에 있는 Object에 PowerCard.cs가 붙어있어서 InChildren으로 호출해야함
            powerDeckTotalWeight += card.GetComponentInChildren<PowerCard>().weight;
        }
    }
    private GameObject RandomPowerCard()
    {
        int weights = 0;
        int selectNum = 0;

        selectNum = Mathf.RoundToInt(powerDeckTotalWeight * Random.Range(0.0f, 1.0f));

        foreach (var card in PowerDeck)
        {
            weights += card.GetComponentInChildren<PowerCard>().weight;
            if (selectNum <= weights)
            {
                return card;
            }
        }

        return null;
    }

    public void RandomCardOpen()
    {
        pickCard = RandomPowerCard();
        Instantiate(pickCard);
        pickCard.SetActive(true);
    }
}
