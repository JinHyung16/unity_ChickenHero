using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

sealed class GameManager : Singleton<GameManager>, IDisposable
{
    public string curUserName { get; set; } //User�� �̸��� ������ �ִٰ� Lobby�� �̵��Ҷ����� �̰ɷ� �����͸� �о�´�.
    public bool isSingleplay { get; set; } = false; //false�� default value �ʱ�ȭ

    [SerializeField] private GameObject offLinePlayerPrefab;
    [SerializeField] private Vector3 playerSpawnPoint;

    private GameObject offlinePlayer = null;

    private void Start()
    {
        if (offLinePlayerPrefab == null)
        {
            offLinePlayerPrefab = Resources.Load("Player/Offline Player") as GameObject;
        }
        playerSpawnPoint = new Vector3(0, -5.5f, 0);
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
        if (isSingleplay)
        {
            offlinePlayer = Instantiate(offLinePlayerPrefab);
            offlinePlayer.transform.SetParent(this.gameObject.transform);
            offlinePlayer.SetActive(true);
            offlinePlayer.transform.position = playerSpawnPoint;

            EnemySpawnManager.GetInstance.StartEnemySpawnerPooling();
        }
    }

    /// <summary>
    /// �߰��� Game�� ������ �ʰ� �� �÷��� ���� ��� ����Ǵ� �Լ�
    /// </summary>
    public void GameEnd()
    {
        EnemySpawnManager.GetInstance.StopEnemySpawnerPooling();

        if (isSingleplay)
        {

            SingleplayManager.GetInstance.UpdateGameResultWhenEnd();

            //off-line player�� �����.
            Dispose();
        }
        else
        {
            SceneController.GetInstance.GoToScene("Lobby").Forget();
        }
    }
}