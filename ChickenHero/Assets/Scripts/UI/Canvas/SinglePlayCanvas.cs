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
    }

    private void DisplayUpdate()
    {
        playerHPTxt.text = playerHp.ToString();
        playerScoreTxt.text = playerScore.ToString();
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