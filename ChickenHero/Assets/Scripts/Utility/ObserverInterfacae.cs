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
}
