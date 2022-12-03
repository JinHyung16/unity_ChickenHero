using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;

sealed class GameManager : Singleton<GameManager>, IDisposable
{
    #region Property
    /// <summary>
    /// single play, multi play ������� score �������ֱ�
    // observer ������ ���� �� ������ �����Ͽ���.
    // interface�� �����ϴ��� Singleton�� �̿��ؼ� property ���·� �ٲ�ô�.
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

    public GameObject OfflinePlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private GameObject offlinePlayer = null;

    private void Start()
    {
        curTime = 60.0f;

        if (OfflinePlayerPrefab == null)
        {
            OfflinePlayerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
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

    public void Dispose()
    {
        offlinePlayer.SetActive(false);
        GC.SuppressFinalize(offlinePlayer);
    }
}
