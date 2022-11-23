using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;

public class ScoreSubject : MonoBehaviour, Subject
{
    private List<Observer> observerList = new List<Observer>();

    private int Score;

    public void NotifyObservers()
    {
        for (int i = 0; i < observerList.Count; i++)
        {
            this.observerList[i].ObserverUpdateScore(Score);
        }
    }

    public void RegisterObserver(Observer _observer)
    {
        this.observerList.Add(_observer);
    }

    public void RemoveObserver(Observer _observer)
    {
        this.observerList.Remove(_observer);
    }

    public void UpdateScore(int _score)
    {
        this.Score = _score;
        this.NotifyObservers();
    }
}
