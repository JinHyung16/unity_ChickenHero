using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelect : MonoBehaviour
{
    public List<GameObject> PowerDeck = new List<GameObject>();
    public int powerDeckTotalWeight;

    private GameObject pickCard;

    private void Start()
    {
        foreach(var card in PowerDeck)
        {
            powerDeckTotalWeight += card.GetComponent<PowerCard>().weight;
        }
    }

    private GameObject RandomPowerCard()
    {
        int weights = 0;
        int selectNum = 0;

        selectNum = Mathf.RoundToInt(powerDeckTotalWeight * Random.Range(0.0f, 1.0f));

        foreach (var card in PowerDeck)
        {
            weights += card.GetComponent<PowerCard>().weight;
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
