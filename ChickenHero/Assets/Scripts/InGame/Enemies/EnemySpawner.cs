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
    [SerializeField] private int enemyCount = 0;
    [SerializeField] private float enemySpawnTime = 0;
    private IObjectPool<Enemy> enemyPool;

    IEnumerator enemyIEnumerator;

    public void InitEnemySpawnerPooling()
    {
        xPosRight = 2.0f; 
        xPosLeft = -2.0f;
        yPosUp = 4.0f;
        yPosDown = -4.0f;

        enemySpawnTime = 5.0f;
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
