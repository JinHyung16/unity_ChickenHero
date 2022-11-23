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
    
    //기본 enemy pooling
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 0;
    [SerializeField] private float enemySpawnTime = 0;
    private IObjectPool<Enemy> enemyPool;

    IEnumerator enemyIEnumerator;

    public void EnemySpwnStart()
    {
        enemySpawnTime = 3.0f;
        enemyCount = 10;
        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, maxSize: enemyCount);
        enemyIEnumerator = EnemySpawnTimer();

        StartCoroutine(enemyIEnumerator);
    }

    public void EnemySpanwStop()
    {
        StopCoroutine(enemyIEnumerator);
    }

    private IEnumerator EnemySpawnTimer()
    {
        while (true)
        {
            var enemy = enemyPool.Get();
            enemy.transform.position = transform.position;
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
    /// <param name="e">Pool에 있는 Enemy를 가져올 매개변수</param>
    private void OnGetEnemy(Enemy e)
    {
        e.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool에 Object를 돌려줄 함수
    /// </summary>
    /// <param name="e">Pool에 돌려줄 Enemy 매개변수</param>
    private void OnReleaseEnemy(Enemy e)
    {
        e.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool에서 Object를 파괴할 때 쓸 함수
    /// </summary>
    /// <param name="e">Pool에서 파괴할 Object type의 매개변수</param>
    private void OnDestroyEnemy(Enemy e)
    {
        Destroy(e.gameObject);
    }
    #endregion
}
