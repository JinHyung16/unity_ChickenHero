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
    public bool IsSinglePlay { get; set; } = false; //false�� default value �ʱ�ȭ

    private void Start()
    {
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
    }

    /// <summary>
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� ���������� �˸���
    /// </summary>
    public void GameStart()
    {
        playerScore = 0;
        IsGameStart = true;
        enemySpawner.StartEnemySpawnerPooling();

        if (IsSinglePlay)
        {
            offlinePlayer = Instantiate(playerPrefab);
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
        enemySpawner.StopEnemySpawnerPooling();

        if (IsSinglePlay)
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
        enemySpawner.StopEnemySpawnerPooling();

        playerScore /= 5;
        int gold = playerScore;
        LocalData.GetInstance.Gold += gold;
        SceneController.GetInstance.GoToScene("Lobby").Forget();

        if (IsSinglePlay)
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
            if (!IsSinglePlay)
            {
                MatchManager.GetInstance.SendMatchState(OpCodes.IsDie, MatchDataJson.Died(true));
            }
            GameClear();
        }
    }

    public void UpdateScoreInGame()
    {
        this.playerScore += 1;
        NotifyObservers(GameNotifyType.ScoreUp);

        if (!IsSinglePlay)
        {
            MatchManager.GetInstance.SendMatchState(OpCodes.Score, MatchDataJson.Score(PlayerScore));
        }
    }

    /// <summary>
    /// offlinePlayer �ı��� �̸� �Ҵ� �����ϰ� �ı��ϱ�
    /// </summary>
    public void Dispose()
    {
        Destroy(offlinePlayer);
        GC.SuppressFinalize(offlinePlayer);
    }

    #region Observer ���� ���� - GameSubject
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