using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using HughGeneric;

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    /// <summary>
    /// EnemySpanwer�� ���, GameManager������ �ٿ����� ����Ѵ�.
    /// Single Play, Multi Play ������� Enemy�� �������Ѿ� �ϱ� ������
    /// Game�� �����Ϸ��� Scene�� �̵��Կ� ���� GameManger���� �̸� �����Ű���� �Ѵ�.
    /// </summary>

    //enemy pooling�ؼ� ��ġ�� ��ġ ���� �����ϱ�
    [SerializeField] private float xPosRight;
    [SerializeField] private float xPosLeft;
    [SerializeField] private float yPosUp;
    [SerializeField] private float yPosDown;

    //�⺻ enemy pooling
    [SerializeField] private GameObject enemyPrefab;
    private IObjectPool<Enemy> enemyPool;

    //UniTask ����
    private CancellationTokenSource tokenSource;

    private bool isSpawnStart = false;

    private void Start()
    {
        InitEnemySpawnManager();
    }

    private void InitEnemySpawnManager()
    {
        xPosRight = 2.0f;
        xPosLeft = -2.0f;
        yPosUp = 3.0f;
        yPosDown = -3.0f;

        //tokenSource�� �̹� �Ҵ�Ǿ� �ִٸ� �����ϰ� �ٽ� ��������
        if (tokenSource != null)
        {
            tokenSource.Dispose();
        }
        tokenSource = new CancellationTokenSource();

        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize: 10);
    }

    public void StartEnemySpawnerPooling()
    {
        isSpawnStart = true;

        if (tokenSource != null)
        {
            tokenSource.Dispose();
        }
        tokenSource = new CancellationTokenSource();

        EnemySpawn().Forget();
    }

    public void StopEnemySpawnerPooling()
    {
        isSpawnStart = false;

        tokenSource.Cancel();
    }

    private async UniTaskVoid EnemySpawn()
    {
        while (isSpawnStart)
        {
            var posX = UnityEngine.Random.Range(xPosLeft, xPosRight);
            var posY = UnityEngine.Random.Range(yPosUp, yPosDown);
            Vector2 posVec = new(posX, posY);

            var enemy = enemyPool.Get();
            enemy.transform.position = posVec;

            float spawnTime = UnityEngine.Random.Range(0.5f, 2.0f);
            await UniTask.Delay(TimeSpan.FromSeconds(spawnTime), cancellationToken: tokenSource.Token);
        }
    }

    #region Object Pool Function
    /// <summary>
    /// Enemy Object�� �����Ѵ�.
    /// ������ �� Enemy Object�� �ڽ��� ��ϵǾ�� �� Pool�� �˷��ְ� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns> Enemy type�� ��ȯ�Ѵ� </returns>
    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.SetManagePool(enemyPool);
        return enemy;
    }

    /// <summary>
    /// Pool���� Object�� ������ �Լ�
    /// </summary>
    /// <param name="enemy">Pool�� �ִ� Enemy�� ������ �Ű�����</param>
    private void OnGetEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool�� Object�� ������ �Լ�
    /// </summary>
    /// <param name="enemy">Pool�� ������ Enemy �Ű�����</param>
    private void OnReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool���� Object�� �ı��� �� �� �Լ�
    /// </summary>
    /// <param name="enemy">Pool���� �ı��� Object type�� �Ű�����</param>
    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
    #endregion
}
