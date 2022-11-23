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
