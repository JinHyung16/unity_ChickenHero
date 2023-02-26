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

    //chicken data ���� ���ε��� ����
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
    /// OnEnable�� OnEnable���� ������ ������ �ʱ�ȭ �κ�
    /// </summary>
    private void InitChickenData()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = chickenData.playerSprite;

        playerHP = chickenData.chickenHP;
    }
}
