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
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� ���������� �˸���
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
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� �������� �˸���
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
    /// �߰��� Game�� ������ �ʰ� �� �÷��� ���� ��� ����Ǵ� �Լ�
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
    /// offlinePlayer �ı��� �̸� �Ҵ� �����ϰ� �ı��ϱ�
    /// </summary>
    public void Dispose()
    {
        offlinePlayer.SetActive(false);
        GC.SuppressFinalize(offlinePlayer);
    }

    #region Observer ���� ���� - GameSubject
    private List<GameObserver> GameObserverList = new List<GameObserver>();

    public void RegisterObserver(GameObserver observer)
    {
        GameObserverList.Add(observer);
        Debug.Log("GameManager Observer ����" + GameObserverList.Count);
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