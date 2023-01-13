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

    /// <summary>
    /// 초기 MultiPlay Scene UI 설정
    /// </summary>
    private void InitMultiPlayCanvas()
    {
        GameManager.GetInstance.GameStart();

        localScore = 0;
        remoteScore = 0;
        localScoreText.text = localScore.ToString();
        remoteScoreText.text = remoteScore.ToString();
    }

    /// <summary>
    /// Score Update해서 UI로 화면에 보여준다
    /// </summary>
    private void UpdateScoreMultiPlay()
    {
        localScore = GameManager.GetInstance.PlayerScore;

        localScoreText.text = localScore.ToString();
        remoteScoreText.text = remoteScore.ToString();
    }
}
