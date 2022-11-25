using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;

public class GameManager : Singleton<GameManager>
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
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� ���������� �˸���
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
    /// OffLine�� �� OffLinePlayer ������ ���� if������ ó���ؼ� ���� �������� �˸���
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
