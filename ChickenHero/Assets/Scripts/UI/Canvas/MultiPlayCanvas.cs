using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiPlayCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text localScoreText;
    [SerializeField] private TMP_Text remoteScoreText;
    [SerializeField] private TMP_Text timerText;

    private int localScore = 0;
    private int remoteScore = 0;

    private void Start()
    {
        InitMultiPlayCanvas();
    }
    private void Update()
    {
        UpdateGameTime();
        UpdateScoreMultiPlay();
    }

    /// <summary>
    /// �ʱ� MultiPlay Scene UI ����
    /// </summary>
    private void InitMultiPlayCanvas()
    {
        localScore = 0;
        remoteScore = 0;
        localScoreText.text = localScore.ToString();
        remoteScoreText.text = remoteScore.ToString();
    }


    private void UpdateGameTime()
    {
        if (GameManager.GetInstance.IsGameStart)
        {
            timerText.text = GameManager.GetInstance.GameTime.ToString("F1");
        }
    }

    /// <summary>
    /// Score Update�ؼ� UI�� ȭ�鿡 �����ش�
    /// </summary>
    private void UpdateScoreMultiPlay()
    {
        if (GameManager.GetInstance.IsEnemyDown)
        {
            localScore = GameManager.GetInstance.LocalUserScore;
            remoteScore = GameManager.GetInstance.RemoteUserScore;

            localScoreText.text = localScore.ToString();
            remoteScoreText.text = remoteScore.ToString();
            GameManager.GetInstance.IsEnemyDown = false;
        }
    }
}
