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
    /// PowerCard를 RandomSelect에서 Instantiate시키면 OnEnable이 먼저 호출되는데
    /// 그 때, 미리 바인딩할 값을 초기화 해둔다.
    /// </summary>
    private void InitPowerCard()
    {
        rectTransfrom.sizeDelta = new Vector2(250f, 350f);

        fadeTime = 0.2f;
    }

    /// <summary>
    /// RandomSelect에서 뽑은 PowerCard 오브젝트의 PowerCardData값을 넣어주어
    /// 해당 값으로 이미지, power, 설명 등 값을 넣어준다.
    /// </summary>
    /// <param name="cardData"> RandomSelect에서 뽑은 PowerCardData</param>
    public void SetPowerCard(PowerCardData cardData)
    {
        descriptionTxt.text = "Power\n" + cardData.powerCardDescription;
        cardImg.overrideSprite = cardData.powerCardSprite;

        powerCardName = cardData.powerCardName;

        RotateCard();
    }

    /// <summary>
    /// Card Open되면 FadeIn 효과 주기
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
    /// PowerCard가 다 보여진 후, SetActive만 꺼준다.
    /// 왜냐면 RandomSelect.cs에서 Dictionary에 저장해두다가 ShopPanel꺼질 때 한번에 지울거기 때문
    /// </summary>
    private void CardDespawn()
    {
        this.gameObject.SetActive(false);
    }
}
