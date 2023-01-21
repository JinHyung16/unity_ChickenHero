using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HughUtility;
using HughUtility.Observer;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class SinglePlayCanvas : GameObserver
{
    //UI관련
    [SerializeField] private TMP_Text playerScoreTxt;
    [SerializeField] private TMP_Text playerHPTxt;

    [SerializeField] private GameObject bloodEffectPanel;

    private int playerHp = 0;
    private int playerScore = 0;

    //Camera Shake관련 바인딩
    private CameraShake cameraShake;

    private void Awake()
    {
        GameManager.GetInstance.RegisterObserver(this);
    }
    private void Start()
    {
        InitSinglePlayCanvas();
    }

    private void OnDisable()
    {
        GameManager.GetInstance.RemoveObserver(this);
    }

    /// <summary>
    /// 초기 SinglePlay Scene에서의 UI 세팅
    /// </summary>
    private void InitSinglePlayCanvas()
    {
        GameManager.GetInstance.GameStart();
        playerScore = 0;
        DisplayUpdate();
        bloodEffectPanel.SetActive(false);

        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void DisplayUpdate()
    {
        playerHPTxt.text = "HP: " + playerHp.ToString();
        playerScoreTxt.text = "점수: " + playerScore.ToString();
    }

    /// <summary>
    /// Exit Yes Button 을 누르면 다시 로그인 화면으로 보낸다.
    /// 이때, 게임하면서 모았던 골드나 이름은 저장되지 않는다.
    /// </summary>
    public void ExitSingleGame()
    {
        GameManager.GetInstance.GameExit();
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }

    #region Observer 패턴 구현 - GameObserver
    public override void UpdateHPText(int playerHP)
    {
        this.playerHp = playerHP;
        DisplayUpdate();
    }

    public override void UpdateScoreText(int score)
    {
        this.playerScore = score;
        DisplayUpdate();
    }

    public override void UpdateAttackDamage()
    {
        BloodEffectTask().Forget();
        if (cameraShake != null)
        {
            cameraShake.CameraShaking();
        }
    }

    private async UniTaskVoid BloodEffectTask()
    {
        bloodEffectPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: this.GetCancellationTokenOnDestroy());
        bloodEffectPanel.SetActive(false);
    }
    #endregion
}