using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using HughUtility.Observer;

sealed class GameManager : Singleton<GameManager>, IDisposable, GameSubject
{
    public int userGold { get; set; }
    public bool IsGameStart { get; set; }

    private int playerScore;
    public int PlayerScore
    {
        get { return playerScore; }
    }
    private int playerHP;

    [SerializeField] private GameObject OfflinePlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private GameObject offlinePlayer = null;

    private void Start()
    {
        if (OfflinePlayerPrefab == null)
        {
            OfflinePlayerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
    }

    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 시작했음을 알린다
    /// </summary>
    public void GameStart()
    {
        IsGameStart = true;

        enemySpawner.InitEnemySpawnerPooling();

        if (GameServer.GetInstance.IsLogin == false)
        {
            offlinePlayer = Instantiate(OfflinePlayerPrefab);
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
        enemySpawner.EnemySpanwStop();

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
        enemySpawner.EnemySpanwStop();

        LocalData.GetInstance.Gold = userGold;
        SceneController.GetInstance.GoToScene("Lobby");
    }

    public void SetGameplayInfo(int hp, int score)
    {
        this.playerHP = hp;
        this.playerScore = score;
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
        Debug.Log("GameManager Observer 갯수" + GameObserverList.Count);
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