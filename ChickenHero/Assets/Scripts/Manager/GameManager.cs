using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using UnityEngine.UI;
using HughLibrary;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        var main = new UnityMainThreadDispatcher();
    }

    private async void MatchStart()
    {
    }
}
