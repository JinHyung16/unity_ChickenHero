using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("PlayerNetworkLocalSync »ý¼ºµÊ");
    }

    private void LateUpdate()
    {
        if (GameManager.GetInstance.canSendScoreToServer)
        {
            SendLocalScore();
        }
    }

    public void SendLocalScore()
    {
        string jsonData = MatchDataJson.Score(GameManager.GetInstance.Score);
        MatchManager.GetInstance.SendMatchState(OpCodes.Score, jsonData);

        GameManager.GetInstance.canSendScoreToServer = false;
    }
}
