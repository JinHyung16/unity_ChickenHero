using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HughUtility;
using HughUtility.Observer;

public class SinglePlayCanvas : MonoBehaviour, GameObserver
{
    //about UI
    [SerializeField] private TMP_Text playerScoreTxt;

    [SerializeField] private TMP_Text playerHPTxt;

    private int playerHp = 0;
    private int playerScore = 0;

    private void Start()
    {
        InitSinglePlayCanvas();
        GameManager.GetInstance.RegisterObserver(this);
    }
    private void OnDestroy()
    {
        GameManager.GetInstance.RemoveObserver(this);
    }

    /// <summary>
    /// 초기 SinglePlay Scene에서의 UI 세팅
    /// </summary>
    private void InitSinglePlayCanvas()
    {
        playerScore = 0;
        DisplayUpdate();
    }

    private void DisplayUpdate()
    {
        playerHPTxt.text = playerHp.ToString();
        playerScoreTxt.text = playerScore.ToString();
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
    #endregion
}