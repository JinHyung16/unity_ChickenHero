using HughUtility.Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MultiplayPresenter : MonoBehaviour
{
    #region Static
    public static MultiplayPresenter GetInstance;

    private void Awake()
    {
        GetInstance = this;
    }
    #endregion

    [SerializeField] private MultiplayViewer multiplayViewer;
    public int PlayerHP { get; set; }

    public int LocalScore { get; private set; } = 0;

    public int RemoteScore { get; private set; } = 0;

    private GameObject localPlayer;

    private void OnDisable()
    {
    }
    private void Start()
    {
        LocalScore = 0;
        RemoteScore = 0;
    }

    public void InitSinglePlayStart()
    {
        multiplayViewer.UpdateHPText(PlayerHP);
        multiplayViewer.UpdateLocalScoreText(LocalScore);
        multiplayViewer.UpdateRemoteScoreText(RemoteScore);
    }

    public void SetOnLocalPlayer(GameObject player)
    {
        this.localPlayer = player;
    }

    public void UpdateHPInMultiplay(int hp)
    {
        PlayerHP -= hp;
        multiplayViewer.UpdateHPText(PlayerHP);
        multiplayViewer.GetDamaged();

        if (PlayerHP <= 0)
        {
            localPlayer.GetComponentInChildren<PlayerDataController>().Died();
        }
    }

    public void UpdateLocalScoreInMultiplay()
    {
        LocalScore++;
        multiplayViewer.UpdateLocalScoreText(LocalScore);
    }

    public void UpdateRemoteScoreInMultiplay(int score)
    {
        RemoteScore = score;
        multiplayViewer.UpdateRemoteScoreText(RemoteScore);
    }
}
