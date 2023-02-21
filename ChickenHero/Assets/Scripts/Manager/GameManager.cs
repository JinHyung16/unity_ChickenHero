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
    public string CurUserName { get; set; } //User�� �̸��� ������ �ִٰ� Lobby�� �̵��Ҷ����� �̰ɷ� �����͸� �о�´�.
    public bool IsSinglePlay { get; set; } = false; //false�� default value �ʱ�ȭ
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
}