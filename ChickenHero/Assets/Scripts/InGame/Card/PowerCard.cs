using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HughUtility.Observer;
using System;
using Cysharp.Threading.Tasks.Triggers;

public class PowerCard : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvaseGroup;
    [SerializeField] private RectTransform rectTransfrom;
    private float fadeTime = 0.2f;

    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private Image cardImg;
    private string powerCardName;

    private void OnEnable()
    {
        InitPowerCard();
    }

    private void OnDisable()
    {
        CancelInvoke("CardDespawn");
    }

    /// <summary>
    /// PowerCard�� RandomSelect���� Instantiate��Ű�� OnEnable�� ���� ȣ��Ǵµ�
    /// �� ��, �̸� ���ε��� ���� �ʱ�ȭ �صд�.
    /// </summary>
    private void InitPowerCard()
    {
        rectTransfrom.sizeDelta = new Vector2(250f, 350f);

        fadeTime = 0.2f;
    }

    /// <summary>
    /// RandomSelect���� ���� PowerCard ������Ʈ�� PowerCardData���� �־��־�
    /// �ش� ������ �̹���, power, ���� �� ���� �־��ش�.
    /// </summary>
    /// <param name="cardData"> RandomSelect���� ���� PowerCardData</param>
    public void SetPowerCard(PowerCardData cardData)
    {
        descriptionTxt.text = "Power\n" + cardData.powerCardDescription;
        cardImg.overrideSprite = cardData.powerCardSprite;

        powerCardName = cardData.powerCardName;

        RotateCard();
    }

    /// <summary>
    /// Card Open�Ǹ� FadeIn ȿ�� �ֱ�
    /// </summary>
    private void RotateCard()
    {
        canvaseGroup.alpha = 0.0f;
        rectTransfrom.transform.localPosition = new Vector3(0, -100.0f, 0);
        rectTransfrom.DOAnchorPos(new Vector2(0.0f, 0.0f), fadeTime, false).SetEase(Ease.OutElastic);
        canvaseGroup.DOFade(1, fadeTime);

        Invoke("CardDespawn", 0.5f);
    }

    /// <summary>
    /// PowerCard�� �� ������ ��, SetActive�� ���ش�.
    /// �ֳĸ� RandomSelect.cs���� Dictionary�� �����صδٰ� ShopPanel���� �� �ѹ��� ����ű� ����
    /// </summary>
    private void CardDespawn()
    {
        this.gameObject.SetActive(false);
    }
}
