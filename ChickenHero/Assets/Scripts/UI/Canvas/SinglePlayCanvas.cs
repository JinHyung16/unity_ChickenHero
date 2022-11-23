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

    [HideInInspector] public int score = 0;

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
    ///  ������ ��� �����Ѵ�
    /// </summary>
    private void ScoreUpdate()
    {
        if (GameManager.GetInstance.IsUpdateScore)
        {
            score = GameManager.GetInstance.LocalUserScore;
            scoreTxt.text = score.ToString();
            GameManager.GetInstance.IsUpdateScore = false;
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