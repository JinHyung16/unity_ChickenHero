using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using HughUtility.Observer;
using Nakama;
using Nakama.TinyJson;
using System.Text;
using Cysharp.Threading.Tasks;

sealed class GameManager : Singleton<GameManager>, GameSubject, IDisposable
{
    public bool IsOfflinePlay { get; set; } = false; //false�� default value �ʱ�ȭ
    public bool IsSinglePlay { get; set; } = false; //false�� default value �ʱ�ȭ
    public int Score{ get; private set;}
    public int RemoteScore { get; private set; }
    public bool canSendScoreToServer { get; set; } = false;
    public int PlayerHP { private get; set; }

    [SerializeField] private GameObject offLinePlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private GameObject offlinePlayer = null;

    private void Start()
    {
        if (offLinePlayerPrefab == null)
        {
            offLinePlayerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
    }

    #region IDisposable Interface ����
    /// <summary>
    /// offlinePlayer �ı��� �̸� �Ҵ� �����ϰ� �ı��ϱ�
    /// </summary>
    public void Dispose()
    {
        Destroy(offlinePlayer);
        GC.SuppressFinalize(offlinePlayer);
    }
    #endregion

    /// <summary>
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� ���������� �˸���
    /// </summary>
    public void GameStart()
    {
        Score = 0;
        if (IsSinglePlay)
        {
            offlinePlayer = Instantiate(offLinePlayerPrefab);
            offlinePlayer.transform.SetParent(this.gameObject.transform);
            offlinePlayer.SetActive(true);
            offlinePlayer.transform.position = playerSpawnPoint.position;

            EnemySpawnManager.GetInstance.StartEnemySpawnerPooling();
        }
    }

    /// <summary>
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� �������� �˸���
    /// </summary>
    public void GameExit()
    {
        EnemySpawnManager.GetInstance.StopEnemySpawnerPooling();

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
        EnemySpawnManager.GetInstance.StopEnemySpawnerPooling();

        Score /= 5;
        int gold = Score;
        LocalData.GetInstance.Gold += gold;
        SceneController.GetInstance.GoToScene("Lobby").Forget();

        if (IsSinglePlay)
        {
            Dispose();
        }
    }
    
    public void UpdateHPInGame(int damange)
    {
        this.PlayerHP -= damange;
        NotifyObservers(GameNotifyType.HPDown);
        if (this.PlayerHP <= 0)
        {
            GameClear();
        }
    }

    public async void UpdateScoreInGame()
    {
        this.Score += 1;
        NotifyObservers(GameNotifyType.ScoreUp);

        if (!IsSinglePlay)
        {
            string jsonData = MatchDataJson.Score(this.Score);
            await MatchManager.GetInstance.SendMatchStateAsync(OpCodes.Score, jsonData);
        }
    }

    public void UpdateRemoteScore(int score)
    {
        this.RemoteScore = score;
        NotifyObservers(GameNotifyType.RemoteUp);
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
                case GameNotifyType.None:
                    observer.UpdateScoreText(Score);
                    observer.UpdateHPText(PlayerHP);
                    break;
                case GameNotifyType.HPDown:
                    observer.UpdateHPText(PlayerHP);
                    observer.UpdateAttackDamage();
                    break;
                case GameNotifyType.ScoreUp:
                    observer.UpdateScoreText(Score);
                    break;
                case GameNotifyType.RemoteUp:
                    observer.UpdateRetmoeScoreText(RemoteScore);
                    break;
                default: //GameNotifyTpye.None�� �ǹ�
                    break;
            }
        }
    }
    #endregion
}