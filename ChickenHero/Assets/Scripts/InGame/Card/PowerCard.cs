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
    /// PowerCard를 RandomSelect에서 Instantiate시키면 OnEnable이 먼저 호출되는데
    /// 그 때, 미리 바인딩할 값을 초기화 해둔다.
    /// </summary>
    private void InitPowerCard()
    {
        rectTransfrom.sizeDelta = new Vector2(250f, 350f);
        cardImg = GetComponentInChildren<Image>();

        fadeTime = 0.2f;
    }

    /// <summary>
    /// RandomSelect에서 뽑은 PowerCard 오브젝트의 PowerCardData값을 넣어주어
    /// 해당 값으로 이미지, power, 설명 등 값을 넣어준다.
    /// </summary>
    /// <param name="cardData"> RandomSelect에서 뽑은 PowerCardData</param>
    public void SetPowerCard(PowerCardData cardData)
    {
        RotateCard();

        descriptionTxt.text = "Power\n" + cardData.powerCardDescription.ToString();
        cardImg.sprite = cardData.powerCardSprite;

        powerCardName = cardData.powerCardName;
        power = cardData.cardPower;
    }

    /// <summary>
    /// Card Open되면 FadeIn 효과 주기
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
    /// 카드 사라실때 오브젝트 파괴하기
    /// </summary>
    public void Dispose()
    {
        tweenRect.Kill();
        tweenCanvas.Kill();

        GC.SuppressFinalize(this.gameObject);
        Destroy(this.gameObject);
    }

    #region Objserver Pattern 관련 함수
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
