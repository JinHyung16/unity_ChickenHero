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
    public bool IsOfflinePlay { get; set; } = false; //false로 default value 초기화
    public bool IsSinglePlay { get; set; } = false; //false로 default value 초기화
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

    #region IDisposable Interface 구현
    /// <summary>
    /// offlinePlayer 파괴전 미리 할당 해제하고 파괴하기
    /// </summary>
    public void Dispose()
    {
        Destroy(offlinePlayer);
        GC.SuppressFinalize(offlinePlayer);
    }
    #endregion

    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 시작했음을 알린다
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
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 끝났음을 알린다
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
    /// 중간에 Game을 나가지 않고 다 플레이 했을 경우 실행되는 함수
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
                default: //GameNotifyTpye.None을 의미
                    break;
            }
        }
    }
    #endregion
}