using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{
    public PlayerDieEvent playerDieEvent;

    private void Start()
    {
        if (playerDieEvent == null)
        {
            playerDieEvent = new PlayerDieEvent();
        }
    }

    public void Died()
    {
        playerDieEvent.Invoke(gameObject);
    }
}