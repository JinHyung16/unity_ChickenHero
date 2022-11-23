using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// EnemySpanwer�� ���, GameManager������ �ٿ����� ����Ѵ�.
    /// Single Play, Multi Play ������� Enemy�� �������Ѿ� �ϱ� ������
    /// Game�� �����Ϸ��� Scene�� �̵��Կ� ���� GameManger���� �̸� �����Ű���� �Ѵ�.
    /// </summary>
    
    //�⺻ enemy pooling
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
    /// <param name="e">Pool�� �ִ� Enemy�� ������ �Ű�����</param>
    private void OnGetEnemy(Enemy e)
    {
        e.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool�� Object�� ������ �Լ�
    /// </summary>
    /// <param name="e">Pool�� ������ Enemy �Ű�����</param>
    private void OnReleaseEnemy(Enemy e)
    {
        e.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool���� Object�� �ı��� �� �� �Լ�
    /// </summary>
    /// <param name="e">Pool���� �ı��� Object type�� �Ű�����</param>
    private void OnDestroyEnemy(Enemy e)
    {
        Destroy(e.gameObject);
    }
    #endregion
}
