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
    /// EnemySpanwer의 경우, GameManager하위에 붙여놓고 사용한다.
    /// Single Play, Multi Play 상관없이 Enemy는 생성시켜야 하기 때문에
    /// Game을 시작하려고 Scene을 이동함에 따라 GameManger에서 이를 실행시키도록 한다.
    /// </summary>

    //enemy pooling해서 배치할 위치 범위 지정하기
    [SerializeField] private float xPosRight;
    [SerializeField] private float xPosLeft;
    [SerializeField] private float yPosUp;
    [SerializeField] private float yPosDown;

    //기본 enemy pooling
    [SerializeField] private GameObject enemyPrefab;
    private IObjectPool<Enemy> enemyPool;

    //UniTask 관련
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

        //tokenSource가 이미 할당되어 있다면 해제하고 다시 생성하자
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
    /// Enemy Object를 생성한다.
    /// 생성된 후 Enemy Object에 자신이 등록되어야 할 Pool을 알려주고 반환한다.
    /// </summary>
    /// <returns> Enemy type을 반환한다 </returns>
    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.SetManagePool(enemyPool);
        return enemy;
    }

    /// <summary>
    /// Pool에서 Object를 가져올 함수
    /// </summary>
    /// <param name="enemy">Pool에 있는 Enemy를 가져올 매개변수</param>
    private void OnGetEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool에 Object를 돌려줄 함수
    /// </summary>
    /// <param name="enemy">Pool에 돌려줄 Enemy 매개변수</param>
    private void OnReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool에서 Object를 파괴할 때 쓸 함수
    /// </summary>
    /// <param name="enemy">Pool에서 파괴할 Object type의 매개변수</param>
    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
    #endregion
}
