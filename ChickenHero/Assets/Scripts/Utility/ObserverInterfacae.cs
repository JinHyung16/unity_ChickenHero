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

    #region GamePlayer Observer - Single Play or Multi Play
    public class GameObserver : MonoBehaviour
    {
        public virtual void UpdateHPText(int playerHP)
        {
        }
        public virtual void UpdateScoreText(int score)
        {
        }
        public virtual void UpdateAttackDamage()
        {
        }

        public virtual void UpdateRetmoeScoreText(int score)
        {
        }
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
        RemoteUp,
    }
    #endregion

}
