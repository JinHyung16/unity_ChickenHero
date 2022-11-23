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

    private int score = 0;

    private void Start()
    {
        score = 0;
        scoreTxt.text = score.ToString();
    }

    private void Update()
    {
        ScoreUpdate();
    }

    /// <summary>
    ///  점수를 계속 갱신한다
    /// </summary>
    private void ScoreUpdate()
    {
        if (GameManager.GetInstance.IsScoreUpdate)
        {
            score = GameManager.GetInstance.LocalUserScore;
            scoreTxt.text = score.ToString();
            GameManager.GetInstance.IsScoreUpdate = false;
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