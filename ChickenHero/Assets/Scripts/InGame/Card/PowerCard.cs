using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerCard : MonoBehaviour
{
    [SerializeField] private PowerCardData powerCardData;

    [SerializeField] private TMP_Text nameTxt;
    private Sprite cardSprite;

    public int power;
    public int weight;

    private void Awake()
    {
        cardSprite = GetComponent<Sprite>();
        nameTxt.text = powerCardData.name;
        cardSprite = powerCardData.powerCardSprite;
        power = powerCardData.cardPower;
        weight = powerCardData.weight;
    }
}
