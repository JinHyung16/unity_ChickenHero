using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
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
    [SerializeField] private float enemySpawnTime = 0;
    private IObjectPool<Enemy> enemyPool;

    IEnumerator enemySpawn;

    public void InitEnemySpawnerPooling()
    {
        xPosRight = 2.0f; 
        xPosLeft = -2.0f;
        yPosUp = 3.0f;
        yPosDown = -3.0f;

        enemySpawnTime = 1.0f;
        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, true, 10, maxSize: 20);
        enemySpawn = EnemySpawnCoroutine();

        StartCoroutine(enemySpawn);
    }

    public void EnemySpanwStop()
    {
        StopCoroutine(enemySpawn);
    }

    private IEnumerator EnemySpawnCoroutine()
    {
        while (true)
        {
            var posX = Random.Range(xPosLeft, xPosRight);
            var posY = Random.Range(yPosUp, yPosDown);
            Vector2 posVec = new(posX, posY);

            var enemy = enemyPool.Get();
            enemy.transform.position = posVec;
            yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(enemySpawnTime);
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
