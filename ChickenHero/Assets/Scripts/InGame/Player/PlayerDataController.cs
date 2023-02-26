using HughUtility.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] private ChickenData chickenData;

    //player died event when use multiplay
    public PlayerDieEvent playerDieEvent;

    //chicken data 관련 바인딩할 변수
    private SpriteRenderer spriteRenderer;
    private int playerHP;

    private void OnEnable()
    {
        InitChickenData();
    }

    private void Start()
    {
        if (GameManager.GetInstance.isSingleplay)
        {
            SingleplayManager.GetInstance.playerHP = this.playerHP;
            SingleplayManager.GetInstance.NotifyObservers(SingleplayNotifyType.None);
        }
        else
        {
            MultiplayManager.GetInstance.PlayerHP = this.playerHP;
            MultiplayManager.GetInstance.NotifyObservers(MultiplayNotifyType.None);
        }

        if (playerDieEvent == null)
        {
            playerDieEvent = new PlayerDieEvent();
        }
    }

    public void Died()
    {
        playerDieEvent.Invoke(gameObject);
    }


    /// <summary>
    /// OnEnable시 OnEnable에서 수행할 데이터 초기화 부분
    /// </summary>
    private void InitChickenData()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = chickenData.playerSprite;

        playerHP = chickenData.chickenHP;
    }
}
