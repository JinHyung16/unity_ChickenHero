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
    //UI관련
    [SerializeField] private TMP_Text playerScoreTxt;
    [SerializeField] private TMP_Text playerHPTxt;
    [SerializeField] private TMP_Text stageInfoTxt;

    //StageClear UI관련
    [SerializeField] private GameObject stageClearPanel;

    //타격 효과 UI 관련
    [SerializeField] private GameObject bloodEffectPanel;

    //ResultPanel UI 관련
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultStageTxt;
    [SerializeField] private TMP_Text resultScoreTxt;
    [SerializeField] private TMP_Text resultGoldTxt;

    private int playerHp = 0;
    private int playerScore = 0;
    private int curStage = 1;

    //Camera Shake관련 바인딩
    private CameraShake cameraShake;

    private void Start()
    {
        InitSinglePlayCanvas();
    }

    /// <summary>
    /// 초기 SinglePlay Scene에서의 UI 세팅
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
        playerScoreTxt.text = "점수: " + playerScore.ToString();
    }

    #region Exit Button - Sinplay Scene
    /// <summary>
    /// Exit Yes Button 을 누르면 다시 로그인 화면으로 보낸다.
    /// 이때, 게임하면서 모았던 골드나 이름은 저장되지 않는다.
    /// </summary>
    public void ExitSingleGame()
    {
        GameManager.GetInstance.GameEnd();
    }
    #endregion

    #region Observer 패턴 구현 - GameObserver
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
    /// Stage Clear시 Stage Up UI를 잠깐 보여주고 닫게 한다.
    /// </summary>
    /// <returns> UniTaskVoid를 콜백한다 </returns>
    private async UniTaskVoid OpenStageClearPanel()
    {
        stageClearPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
        stageInfoTxt.text = "Stage: " + curStage.ToString(); ;
        stageClearPanel.SetActive(false);
    }

    /// <summary>
    /// Game을 나가거나, Stage를 다 클리어 했을때 게임 결과를 보여준다.
    /// </summary>
    /// <param name="stage">최종 클리어한 stage</param>
    /// <param name="score">최종 획득함 점수</param>
    /// <param name="gold">최종 획득한 골드량</param>
    public void ResultGameText(int stage, int score, int gold)
    {
        resultStageTxt.text = "클리어 단계: " + stage.ToString();
        resultScoreTxt.text = "획득한 점수: " + score.ToString();
        resultGoldTxt.text = "획득한 골드: " + gold.ToString();
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
    /// GetDamaged()에서 호출하는 함수
    /// </summary>
    /// <returns> UniTaskVoid를 반환해서 콜백한다 </returns>
    private async UniTaskVoid BloodEffectTask()
    {
        bloodEffectPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: this.GetCancellationTokenOnDestroy());
        bloodEffectPanel.SetActive(false);
    }
    #endregion
}