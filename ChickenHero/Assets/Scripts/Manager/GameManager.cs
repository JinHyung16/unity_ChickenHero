using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

sealed class GameManager : Singleton<GameManager>, IDisposable
{
    public bool isSingleplay { get; set; } = false; //false로 default value 초기화

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
    /// 중간에 Game을 나가지 않고 다 플레이 했을 경우 실행되는 함수
    /// </summary>
    public void GameEnd()
    {
        EnemySpawnManager.GetInstance.StopEnemySpawnerPooling();

        if (isSingleplay)
        {

            SingleplayPresenter.GetInstance.UpdateGameResultWhenEnd();

            //off-line player를 지운다.
            Dispose();
        }
        else
        {
            SceneController.GetInstance.GoToScene("Lobby").Forget();
        }
    }
}