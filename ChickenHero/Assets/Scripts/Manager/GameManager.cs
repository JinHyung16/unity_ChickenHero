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

sealed class GameManager : Singleton<GameManager>, IDisposable
{
    public string CurUserName { get; set; } //User의 이름을 가지고 있다가 Lobby로 이동할때마다 이걸로 데이터를 읽어온다.
    public bool IsSinglePlay { get; set; } = false; //false로 default value 초기화
    public int Score { get; set; } = 0;

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
}