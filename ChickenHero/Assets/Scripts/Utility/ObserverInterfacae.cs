using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HughUtility.Observer
{
    public interface IObserver
    {
        //power card opoen Data 알려주기
        void UpdateOpenPowerCard(string name, int power);
    }

    public interface ISubject
    {
        void RegisterObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers();
    }
}
