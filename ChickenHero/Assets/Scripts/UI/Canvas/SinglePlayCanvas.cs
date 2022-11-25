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
    /// �ʱ� SinglePlay Scene������ UI ����
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
    ///  Enemy�� ����� ��, ������ ��� �����Ѵ�
    ///  MltiPlay, SinglePlay ��� UI�� ������
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
    /// Exit Yes Button �� ������ �ٽ� �α��� ȭ������ ������.
    /// �̶�, �����ϸ鼭 ��Ҵ� ��峪 �̸��� ������� �ʴ´�.
    /// </summary>
    public void ExitSingleGame()
    {
        SceneController.GetInstance.GoToScene("Lobby");
        GameManager.GetInstance.GameExit();
    }
}