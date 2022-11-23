using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreObserver : MonoBehaviour, Observer
{
    private Subject subject;

    private int score;
    public void InitSubject(Subject _subject)
    {
        this.subject = _subject;
    }

    public void ObserverUpdateScore(int _score)
    {
        score = _score;
    }
}
