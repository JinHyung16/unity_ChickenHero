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

    public bool IsGameStart { get; set; }
    public bool IsScoreUpdate { get; set; }
    #endregion

    [SerializeField] private GameObject offLinePlayer;
    [SerializeField] private GameObject playerSpawnPoint;

    [SerializeField] private EnemySpawner enemySpawner;

    private void Start()
    {
        offLinePlayer.SetActive(false);
        PlayerPrefs.DeleteAll();
    }

    public void GameStart()
    {
        IsGameStart = true;

        offLinePlayer.SetActive(true);
        offLinePlayer.transform.position = playerSpawnPoint.transform.position;

        enemySpawner.InitEnemySpawnerPooling();
    }

    public void GameExit()
    {
        IsGameStart = false;

        offLinePlayer.SetActive(false);

        enemySpawner.EnemySpanwStop();
    }
}
