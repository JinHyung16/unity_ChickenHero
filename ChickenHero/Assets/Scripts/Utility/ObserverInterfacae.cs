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
    }

    public interface LobbySubject
    {
        void RegisterObserver(LobbyObserver observer);
        void RemoveObserver(LobbyObserver observer);
        void NotifyObservers();
    }
    #endregion

    #region GamePlayer Observer - Single Play or Multi Play
    public interface GameObserver
    {
        void UpdateHPText(int playerHP);
        void UpdateScoreText(int score);

        void UpdateAttackDamage();
    }

    public interface GameSubject
    {
        void RegisterObserver(GameObserver observer);
        void RemoveObserver(GameObserver observer);
        void NotifyObservers(GameNotifyType notifyType);
    }

    public enum GameNotifyType
    {
        None = 0,
        HPDown,
        ScoreUp,
    }
    #endregion

}
