using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using HughUtility.Observer;

sealed class GameManager : Singleton<GameManager>, GameSubject, IDisposable
{
    public bool IsGameStart { get; set; }

    private int playerScore;
    public int PlayerScore
    {
        get { return playerScore; }
    }

    private int playerHP;
    public int PlayerHP { set { this.playerHP = value; } }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private GameObject offlinePlayer = null;

    private void Start()
    {
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
    }

    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 시작했음을 알린다
    /// </summary>
    public void GameStart()
    {
        playerScore = 0;
        IsGameStart = true;
        enemySpawner.StartEnemySpawnerPooling();

        if (!GameServer.GetInstance.GetIsServerConnect())
        {
            offlinePlayer = Instantiate(playerPrefab);
            offlinePlayer.transform.SetParent(this.gameObject.transform);
            offlinePlayer.SetActive(true);
            offlinePlayer.transform.position = playerSpawnPoint.position;
        }
    }

    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 끝났음을 알린다
    /// </summary>
    public void GameExit()
    {
        IsGameStart = false;
        enemySpawner.StopEnemySpawnerPooling();

        if (!GameServer.GetInstance.GetIsServerConnect())
        {
            Dispose();
        }
    }

    /// <summary>
    /// 중간에 Game을 나가지 않고 다 플레이 했을 경우 실행되는 함수
    /// </summary>
    public void GameClear()
    {
        IsGameStart = false;
        enemySpawner.StopEnemySpawnerPooling();

        playerScore /= 5;
        int gold = playerScore;
        LocalData.GetInstance.Gold += gold;
        SceneController.GetInstance.GoToScene("Lobby").Forget();

        if (!GameServer.GetInstance.GetIsServerConnect())
        {
            Dispose();
        }
    }
    
    public void UpdateHPInGame(int damange)
    {
        this.playerHP -= damange;
        NotifyObservers(GameNotifyType.HPDown);
        if (this.playerHP <= 0)
        {
            GameClear();
        }
    }

    public void UpdateScoreInGame()
    {
        this.playerScore += 1;
        NotifyObservers(GameNotifyType.ScoreUp);
    }

    /// <summary>
    /// offlinePlayer 파괴전 미리 할당 해제하고 파괴하기
    /// </summary>
    public void Dispose()
    {
        Destroy(offlinePlayer);
        GC.SuppressFinalize(offlinePlayer);
    }

    #region Observer 패턴 구현 - GameSubject
    private List<GameObserver> GameObserverList = new List<GameObserver>();

    public void RegisterObserver(GameObserver observer)
    {
        GameObserverList.Add(observer);
    }

    public void RemoveObserver(GameObserver observer)
    {
        GameObserverList.Remove(observer);
    }

    public void NotifyObservers(GameNotifyType notifyType)
    {
        foreach (var observer in GameObserverList)
        {
            switch (notifyType)
            {
                case GameNotifyType.HPDown:
                    observer.UpdateHPText(playerHP);
                    observer.UpdateAttackDamage();
                    break;
                case GameNotifyType.ScoreUp:
                    observer.UpdateScoreText(playerScore);
                    break;
                default:
                    observer.UpdateScoreText(playerScore);
                    observer.UpdateHPText(playerHP);
                    break;
            }
        }
    }
    #endregion
}