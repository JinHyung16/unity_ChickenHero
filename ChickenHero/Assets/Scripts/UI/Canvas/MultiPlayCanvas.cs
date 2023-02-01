using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HughUtility.Observer;
using Cysharp.Threading.Tasks;
using System;

public class MultiPlayCanvas : GameObserver
{
    [SerializeField] private TMP_Text localHPText;
    [SerializeField] private TMP_Text localScoreText;
    [SerializeField] private TMP_Text remoteScoreText;

    [SerializeField] private GameObject bloodEffectPanel;

    private int localHP = 0;
    private int localScore = 0;

    //Camera Shake관련 바인딩
    private CameraShake cameraShake;

    private void Awake()
    {
        GameManager.GetInstance.RegisterObserver(this);
    }
    private void Start()
    {
        InitMultiPlayCanvas();
    }
    private void OnDisable()
    {
        GameManager.GetInstance.RemoveObserver(this);
    }


    /// <summary>
    /// 초기 MultiPlay Scene UI 설정
    /// </summary>
    private void InitMultiPlayCanvas()
    {
        GameManager.GetInstance.GameStart();

        localScore = 0;

        DisplayUpdate();

        localScoreText.text = localScore.ToString();
        remoteScoreText.text = 0.ToString();

        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }

    /// <summary>
    /// Score Update해서 UI로 화면에 보여준다
    /// </summary>
    private void DisplayUpdate()
    {
        localHPText.text = "HP: " + localHP.ToString();
        localScoreText.text = "나: " + localScore.ToString();
    }

    #region Exit Button - Multiplay Scene
    /// <summary>
    /// Exit Yes Button 을 누르면 다시 로그인 화면으로 보낸다.
    /// 이때, 게임하면서 모았던 골드나 이름은 저장되지 않는다.
    /// </summary>
    public async void ExitMultiplayGame()
    {
        await MatchManager.GetInstance.QuickMatch();
        GameManager.GetInstance.GameExit();
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }
    #endregion

    #region Observer 패턴 구현 - GameObserver
    public override void UpdateHPText(int playerHP)
    {
        this.localHP = playerHP;
        DisplayUpdate();
    }

    public override void UpdateScoreText(int score)
    {
        this.localScore = score;
        DisplayUpdate();
    }

    public override void UpdateRetmoeScoreText(int score)
    {
        remoteScoreText.text = "상대: " + score.ToString();
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
