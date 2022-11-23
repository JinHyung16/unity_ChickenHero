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

    public bool IsUpdateScore { get; set; }
    #endregion

    [SerializeField] private EnemySpawner enemySpawner;

    public void GameStart()
    {
        enemySpawner.EnemySpwnStart();
    }

    public void GameExit()
    {
        enemySpawner.EnemySpanwStop();
    }
}
