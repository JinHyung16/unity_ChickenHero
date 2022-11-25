using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HughUtility;


public class SinglePlayCanvas : MonoBehaviour
{
    //about UI
    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private Button exitBtn;

    [SerializeField] private TMP_Text timerTxt;

    private int score = 0;

    private void Start()
    {
        InitSinglePlayCanvas();
    }
    private void Update()
    {
        UpdateScoreSinglePlay();
        UpdateGameTime();
    }

    /// <summary>
    /// 초기 SinglePlay Scene에서의 UI 세팅
    /// </summary>
    private void InitSinglePlayCanvas()
    {
        score = 0;

        scoreTxt.text = score.ToString();
    }

    private void UpdateGameTime()
    {
        if (GameManager.GetInstance.IsGameStart)
        {
            timerTxt.text = GameManager.GetInstance.GameTime.ToString("F1");
        }
    }

    /// <summary>
    ///  Enemy를 잡았을 때, 점수를 계속 갱신한다
    ///  MltiPlay, SinglePlay 모두 UI로 보여줌
    /// </summary>
    private void UpdateScoreSinglePlay()
    {
        if (GameManager.GetInstance.IsEnemyDown)
        {
            score = GameManager.GetInstance.LocalUserScore;
            scoreTxt.text = score.ToString();
            GameManager.GetInstance.IsEnemyDown = false;
        }
    }

    /// <summary>
    /// Exit Yes Button 을 누르면 다시 로그인 화면으로 보낸다.
    /// 이때, 게임하면서 모았던 골드나 이름은 저장되지 않는다.
    /// </summary>
    public void ExitSingleGame()
    {
        SceneController.GetInstance.GoToScene("Lobby");
        GameManager.GetInstance.GameExit();
    }
}