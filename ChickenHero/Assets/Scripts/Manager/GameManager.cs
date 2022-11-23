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

    public bool IsScoreUpdate { get; set; }
    #endregion

    [SerializeField] private GameObject offLinePlayer;
    [SerializeField] private GameObject playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        offLinePlayer.SetActive(false);
    }

    public void GameStart()
    {
        offLinePlayer.SetActive(true);
        offLinePlayer.transform.position = playerSpawnPoint.transform.position;

        enemySpawner.InitEnemySpawnerPooling();
    }

    public void GameExit()
    {
        enemySpawner.EnemySpanwStop();
        offLinePlayer.SetActive(false);
    }
}
