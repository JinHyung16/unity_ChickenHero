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
    //UI����
    [SerializeField] private TMP_Text playerScoreTxt;
    [SerializeField] private TMP_Text playerHPTxt;

    [SerializeField] private GameObject bloodEffectPanel;

    private int playerHp = 0;
    private int playerScore = 0;

    //Camera Shake���� ���ε�
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
    /// �ʱ� SinglePlay Scene������ UI ����
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
        playerScoreTxt.text = "����: " + playerScore.ToString();
    }

    /// <summary>
    /// Exit Yes Button �� ������ �ٽ� �α��� ȭ������ ������.
    /// �̶�, �����ϸ鼭 ��Ҵ� ��峪 �̸��� ������� �ʴ´�.
    /// </summary>
    public void ExitSingleGame()
    {
        GameManager.GetInstance.GameExit();
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }

    #region Observer ���� ���� - GameObserver
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