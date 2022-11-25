using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;

public class GameManager : Singleton<GameManager>
{
    #region Property
    /// <summary>
    /// single play, multi play 상관없이 score 관리해주기
    // observer 패턴을 보고 그 원리를 응용하였다.
    // interface로 관리하던걸 Singleton을 이용해선 property 형태로 바꿔봤다.
    /// </summary>

    public int LocalUserScore { get; set; }
    public int RemoteUserScore { get; set; }
    public int UserGold { get; set; }

    public bool IsGameStart { get; set; }
    public bool IsEnemyDown { get; set; }

    [SerializeField] private float curTime;
    public float GameTime
    {
        get
        {
            return curTime;
        }
    }
    #endregion

    [SerializeField] private GameObject offLinePlayer;
    [SerializeField] private GameObject playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        offLinePlayer.SetActive(false);

        curTime = 60.0f;

        PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        if (IsGameStart)
        {
            curTime -= Time.deltaTime;
            if (curTime <= 0)
            {
                IsGameStart = false;
            }
        }
    }


    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 시작했음을 알린다
    /// </summary>
    public void GameStart()
    {
        IsGameStart = true;
        enemySpawner.InitEnemySpawnerPooling();

        if (!GameServer.GetInstance.IsLogin)
        {
            offLinePlayer.SetActive(true);
            offLinePlayer.transform.position = playerSpawnPoint.transform.position;
        }
    }

    /// <summary>
    /// OffLine일 땐 OffLinePlayer 생성만 따로 if문으로 처리해서 게임 끝났음을 알린다
    /// </summary>
    public void GameExit()
    {
        IsGameStart = false;
        enemySpawner.EnemySpanwStop();

        if (!GameServer.GetInstance.IsLogin)
        {
            offLinePlayer.SetActive(false);
            offLinePlayer.transform.position = playerSpawnPoint.transform.position;
        }
    }
}
