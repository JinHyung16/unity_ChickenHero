using HughUtility.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MultiplayManager : MonoBehaviour, MultiplaySubject
{
    #region Singleton
    private static MultiplayManager instance;
    public static MultiplayManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    
    public int PlayerHP { get; set; }

    public int LocalScore { get; private set; } = 0;

    public int RemoteScore { get; private set; } = 0;

    private GameObject localPlayer;

    private void OnDisable()
    {
        RemoveAllObserver();
    }
    private void Start()
    {
        LocalScore = 0;
        RemoteScore = 0;
    }

    public void SetOnLocalPlayer(GameObject player)
    {
        this.localPlayer = player;
    }

    public void UpdateHPInMultiplay(int hp)
    {
        PlayerHP -= hp;
        NotifyObservers(MultiplayNotifyType.HP);

        if (PlayerHP <= 0)
        {
            localPlayer.GetComponentInChildren<PlayerDataController>().Died();
        }
    }

    public void UpdateLocalScoreInMultiplay()
    {
        LocalScore++;
        NotifyObservers(MultiplayNotifyType.LocalScore);
    }

    public void UpdateRemoteScoreInMultiplay(int score)
    {
        RemoteScore = score;
        NotifyObservers(MultiplayNotifyType.RemoteScore);
    }


    #region Observer pattern interface±¸Çö
    private List<MultiplayObserver> observerList = new List<MultiplayObserver>();
    public void RegisterObserver(MultiplayObserver observer)
    {
        observerList.Add(observer);
    }

    public void RemoveAllObserver()
    {
        observerList.Clear();
    }
    public void NotifyObservers(MultiplayNotifyType notifyType)
    {
        foreach (var observer in observerList)
        {

            switch (notifyType)
            {
                case MultiplayNotifyType.None:
                    observer.UpdateHPText(PlayerHP);
                    observer.UpdateLocalScoreText(LocalScore);
                    observer.UpdateRemoteScoreText(RemoteScore);
                    break;
                case MultiplayNotifyType.HP:
                    observer.UpdateHPText(PlayerHP);
                    observer.GetDamaged();
                    break;
                case MultiplayNotifyType.LocalScore:
                    observer.UpdateLocalScoreText(LocalScore);
                    break;
                case MultiplayNotifyType.RemoteScore:
                    observer.UpdateRemoteScoreText(RemoteScore);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}
