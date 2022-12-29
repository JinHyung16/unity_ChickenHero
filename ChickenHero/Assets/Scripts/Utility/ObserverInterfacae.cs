using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HughUtility.Observer
{
    public interface IObserver
    {
        //power card opoen Data �˷��ֱ�
        void UpdateOpenPowerCard(PowerCardData cardData);
    }

    public interface ISubject
    {
        void RegisterObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers();
    }
}
