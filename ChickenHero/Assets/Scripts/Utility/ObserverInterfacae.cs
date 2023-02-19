using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HughUtility.Observer
{
    #region LobbyObserver
    public interface LobbyObserver
    {
        //power card opoen Data 알려주기
        void UpdateOpenPowerCard(PowerCardData cardData);
        void UpdatePowerUp();
    }

    public interface LobbySubject
    {
        void RegisterObserver(LobbyObserver observer);
        void RemoveObserver(LobbyObserver observer);
        void NotifyObservers(LobbyNotifyType lobbyNotifyType);
    }

    public enum LobbyNotifyType
    {
        OpenCard = 0,
        UpgradePower,
    }
    #endregion

    #region Singleplay Observer
    public interface SingleplayObserver
    {
        void UpdateHPText(int playerHP);
        void GetDamaged();
        void UpdateScoreText(int score);
    }

    public interface SingleplaySubject
    {
        void RegisterObserver(SingleplayObserver observer);
        void RemoveObserver(SingleplayObserver observer);
        void NotifyObservers(SingleplayNotifyType notifyType);
    }

    public enum SingleplayNotifyType
    {
        None = 0,
        HP,
        Score,
    }
    #endregion

    #region Multiplay Observer
    public interface MultiplayObserver
    {
        void UpdateHPText(int playerHP);
        void GetDamaged();
        void UpdateLocalScoreText(int score);

        void UpdateRemoteScoreText(int score);
    }

    public interface MultiplaySubject
    {
        void RegisterObserver(MultiplayObserver observer);
        void RemoveObserver(MultiplayObserver observer);
        void NotifyObservers(MultiplayNotifyType notifyType);
    }

    public enum MultiplayNotifyType
    {
        None = 0,
        HP,
        LocalScore,
        RemoteScore,
    }
    #endregion
}
