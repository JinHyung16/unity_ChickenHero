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


    //enemy pooling�ؼ� ��ġ�� ��ġ ���� �����ϱ�
    [SerializeField] private float xPosRight;
    [SerializeField] private float xPosLeft;
    [SerializeField] private float yPosUp;
    [SerializeField] private float yPosDown;

    //�⺻ enemy pooling
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
