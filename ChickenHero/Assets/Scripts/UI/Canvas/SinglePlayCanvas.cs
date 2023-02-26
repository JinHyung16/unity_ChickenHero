using TMPro;
using UnityEngine;
using HughUtility;
using HughUtility.Observer;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor.SceneManagement;

public class SingleplayCanvas : MonoBehaviour, SingleplayObserver
{
    //UI����
    [SerializeField] private TMP_Text playerScoreTxt;
    [SerializeField] private TMP_Text playerHPTxt;
    [SerializeField] private TMP_Text stageInfoTxt;

    //StageClear UI����
    [SerializeField] private GameObject stageClearPanel;

    //Ÿ�� ȿ�� UI ����
    [SerializeField] private GameObject bloodEffectPanel;

    //ResultPanel UI ����
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultStageTxt;
    [SerializeField] private TMP_Text resultScoreTxt;
    [SerializeField] private TMP_Text resultGoldTxt;

    private int playerHp = 0;
    private int playerScore = 0;
    private int curStage = 1;

    //Camera Shake���� ���ε�
    private CameraShake cameraShake;

    private void Start()
    {
        InitSinglePlayCanvas();
    }

    /// <summary>
    /// �ʱ� SinglePlay Scene������ UI ����
    /// </summary>
    private void InitSinglePlayCanvas()
    {
        SingleplayManager.GetInstance.RegisterObserver(this);

        GameManager.GetInstance.GameStart();
        playerScore = 0;
        DisplayUpdate();

        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();

        bloodEffectPanel.SetActive(false);
        stageClearPanel.SetActive(false);
        resultPanel.SetActive(false);

        stageInfoTxt.text = "Stage: " + 1.ToString();
    }

    private void DisplayUpdate()
    {
        playerHPTxt.text = "HP: " + playerHp.ToString();
        playerScoreTxt.text = "����: " + playerScore.ToString();
    }

    #region Exit Button - Sinplay Scene
    /// <summary>
    /// Exit Yes Button �� ������ �ٽ� �α��� ȭ������ ������.
    /// �̶�, �����ϸ鼭 ��Ҵ� ��峪 �̸��� ������� �ʴ´�.
    /// </summary>
    public void ExitSingleGame()
    {
        GameManager.GetInstance.GameEnd();
    }
    #endregion

    #region Observer ���� ���� - GameObserver
    public void UpdateHPText(int playerHP)
    {
        this.playerHp = playerHP;
        DisplayUpdate();
    }

    public void UpdateScoreText(int score)
    {
        this.playerScore = score;
        DisplayUpdate();
    }

    public void UpdateEnemyDownCountAndStageText(bool isStageClear, int stage)
    {
        if (isStageClear)
        {
            curStage = stage;
            OpenStageClearPanel().Forget();
        }
        else
        {
            stageInfoTxt.text = "Stage: " + stage.ToString();
            stageClearPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Stage Clear�� Stage Up UI�� ��� �����ְ� �ݰ� �Ѵ�.
    /// </summary>
    /// <returns> UniTaskVoid�� �ݹ��Ѵ� </returns>
    private async UniTaskVoid OpenStageClearPanel()
    {
        stageClearPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        stageInfoTxt.text = "Stage: " + curStage.ToString(); ;
        stageClearPanel.SetActive(false);
    }

    /// <summary>
    /// Game�� �����ų�, Stage�� �� Ŭ���� ������ ���� ����� �����ش�.
    /// </summary>
    /// <param name="stage">���� Ŭ������ stage</param>
    /// <param name="score">���� ȹ���� ����</param>
    /// <param name="gold">���� ȹ���� ��差</param>
    public void ResultGameText(int stage, int score, int gold)
    {
        resultStageTxt.text = "Ŭ���� �ܰ�: " + stage.ToString();
        resultScoreTxt.text = "ȹ���� ����: " + score.ToString();
        resultGoldTxt.text = "ȹ���� ���: " + gold.ToString();
        ResultPanelAutoActiveMode().Forget();

    }

    private async UniTaskVoid ResultPanelAutoActiveMode()
    {
        resultPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        resultPanel.SetActive(false);
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }

    public void GetDamaged()
    {
        BloodEffectTask().Forget();
        if (cameraShake != null)
        {
            cameraShake.CameraShaking();
        }
    }

    /// <summary>
    /// GetDamaged()���� ȣ���ϴ� �Լ�
    /// </summary>
    /// <returns> UniTaskVoid�� ��ȯ�ؼ� �ݹ��Ѵ� </returns>
    private async UniTaskVoid BloodEffectTask()
    {
        bloodEffectPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: this.GetCancellationTokenOnDestroy());
        bloodEffectPanel.SetActive(false);
    }
    #endregion
}