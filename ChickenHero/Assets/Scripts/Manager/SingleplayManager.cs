using HughUtility.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayManager : MonoBehaviour, SingleplaySubject
{
    #region Singleton
    private static SingleplayManager instance;
    public static SingleplayManager GetInstance
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

    public int Score { get; private set; } = 0;

    private void Start()
    {
        Score = 0;
    }
    public void UpdateHPInSingleplay(int hp)
    {
        PlayerHP -= hp;
        NotifyObservers(SingleplayNotifyType.HP);
    }

    public void UpdateScoreInSingleplay()
    {
        Score++;
        NotifyObservers(SingleplayNotifyType.Score);
    }

    #region Observer pattern interface±¸Çö
    private List<SingleplayObserver> observerList = new List<SingleplayObserver>();

    public void RegisterObserver(SingleplayObserver observer)
    {
        observerList.Add(observer);
    }

    public void RemoveObserver(SingleplayObserver observer)
    {
        observerList.Remove(observer);
    }

    public void NotifyObservers(SingleplayNotifyType notifyType)
    {
        foreach (var observer in observerList)
        {

            switch (notifyType)
            {
                case SingleplayNotifyType.None:
                    observer.UpdateHPText(PlayerHP);
                    observer.UpdateScoreText(Score);
                    break;
                case SingleplayNotifyType.HP:
                    observer.UpdateHPText(PlayerHP);
                    observer.GetDamaged();
                    break;
                case SingleplayNotifyType.Score:
                    observer.UpdateScoreText(Score);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}
