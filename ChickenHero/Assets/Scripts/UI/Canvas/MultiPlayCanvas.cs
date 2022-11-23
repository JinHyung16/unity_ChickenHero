using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiPlayCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text localScoreText;
    [SerializeField] private TMP_Text remoteScoreText;

    private int localScore = 0;
    private int remoteScore = 0;

    private void Update()
    {
        ScoreUpdate();
    }

    private void ScoreUpdate()
    {
        if (GameManager.GetInstance.IsScoreUpdate)
        {
            localScore = GameManager.GetInstance.LocalUserScore;
            remoteScore = GameManager.GetInstance.RemoteUserScore;

            localScoreText.text = localScore.ToString();
            remoteScoreText.text = remoteScore.ToString();
            GameManager.GetInstance.IsScoreUpdate = false;
        }
    }
}
