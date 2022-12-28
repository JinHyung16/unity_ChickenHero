using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HughUtility.Observer;
using System;

public class PowerCard : MonoBehaviour, IDisposable, ISubject
{
    [SerializeField] private CanvasGroup canvaseGroup;
    [SerializeField] private RectTransform rectTransfrom;
    private float fadeTime = 0.5f;

    [SerializeField] private TMP_Text descriptionTxt;
    private Image cardImg;
    private string powerCardName;

    public int power;

    private Tween tweenRect;
    private Tween tweenCanvas;

    private void OnEnable()
    {
        InitPowerCard();
    }

    private void OnDisable()
    {
        CancelInvoke("RotateCard");
        CancelInvoke("Dispose");
    }

    /// <summary>
    /// PowerCard�� RandomSelect���� Instantiate��Ű�� OnEnable�� ���� ȣ��Ǵµ�
    /// �� ��, �̸� ���ε��� ���� �ʱ�ȭ �صд�.
    /// </summary>
    private void InitPowerCard()
    {
        rectTransfrom.sizeDelta = new Vector2(250f, 350f);
        cardImg = GetComponentInChildren<Image>();

        fadeTime = 0.2f;
    }

    /// <summary>
    /// RandomSelect���� ���� PowerCard ������Ʈ�� PowerCardData���� �־��־�
    /// �ش� ������ �̹���, power, ���� �� ���� �־��ش�.
    /// </summary>
    /// <param name="cardData"> RandomSelect���� ���� PowerCardData</param>
    public void SetPowerCard(PowerCardData cardData)
    {
        RotateCard();

        descriptionTxt.text = "Power\n" + cardData.powerCardDescription.ToString();
        cardImg.sprite = cardData.powerCardSprite;

        powerCardName = cardData.powerCardName;
        power = cardData.cardPower;
    }

    /// <summary>
    /// Card Open�Ǹ� FadeIn ȿ�� �ֱ�
    /// </summary>
    private void RotateCard()
    {
        canvaseGroup.alpha = 0.0f;
        rectTransfrom.transform.localPosition = new Vector3(0, -100.0f, 0);
        tweenRect = rectTransfrom.DOAnchorPos(new Vector2(0.0f, 0.0f), fadeTime, false).SetEase(Ease.OutElastic);
        tweenCanvas = canvaseGroup.DOFade(1, fadeTime);

        Invoke("Dispose", 0.5f);
    }

    /// <summary>
    /// ī�� ���Ƕ� ������Ʈ �ı��ϱ�
    /// </summary>
    public void Dispose()
    {
        tweenRect.Kill();
        tweenCanvas.Kill();

        GC.SuppressFinalize(this.gameObject);
        Destroy(this.gameObject);
    }

    #region Objserver Pattern ���� �Լ�
    public void RegisterObserver(IObserver observer)
    {
    }

    public void RemoveObserver(IObserver observer)
    {
    }

    public void NotifyObservers()
    {
    }
    #endregion
}
