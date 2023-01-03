using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using HughUtility.Observer;
using UnityEngine.SocialPlatforms.Impl;

sealed class GameManager : Singleton<GameManager>, IDisposable, GameSubject
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

        if (GameServer.GetInstance.IsLogin == false)
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

        if (GameServer.GetInstance.IsLogin == false)
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
    }
    
    public void UpdateHPInGame(int damange)
    {
        this.playerHP -= damange;
        NotifyObservers();
        if (this.playerHP <= 0)
        {
            GameClear();
        }
    }

    public void UpdateScoreInGame()
    {
        this.playerScore += 1;
        NotifyObservers();
    }

    /// <summary>
    /// offlinePlayer 파괴전 미리 할당 해제하고 파괴하기
    /// </summary>
    public void Dispose()
    {
        offlinePlayer.SetActive(false);
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

    public void NotifyObservers()
    {
        foreach (var observer in GameObserverList)
        {
            observer.UpdateHPText(playerHP);
            observer.UpdateScoreText(playerScore);
        }
    }
    #endregion
}