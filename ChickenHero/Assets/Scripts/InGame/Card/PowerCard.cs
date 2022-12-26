using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerCard : MonoBehaviour
{
    [SerializeField] private PowerCardData powerCardData;

    [SerializeField] private TMP_Text descriptionTxt;

    private RectTransform rectTransfrom;
    private Image cardImg;
    private string powerCardName;

    public int power;
    public int weight;

    private void OnEnable()
    {
        rectTransfrom = GetComponent<RectTransform>();
        rectTransfrom.sizeDelta = new Vector2(250f, 350f);
        cardImg = GetComponent<Image>();

        descriptionTxt.text = powerCardData.powerCardDescription;
        cardImg.sprite = powerCardData.powerCardSprite;
        powerCardName = powerCardData.powerCardName;
        power = powerCardData.cardPower;
        weight = powerCardData.weight;
    }
}
