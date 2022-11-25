using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamage
{
    public int HP { get; set; }

    //object pooy
    private IObjectPool<Enemy> ManageEnemyPool; //�⺻ enemy�����ϴ� pool

    private void OnEnable()
    {
        HP = 10;
    }

    public void Damaged(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            DestoryEnemy();
            EnemyDieUpdateUI();
        }
    }

    private void EnemyDieUpdateUI()
    {
        GameManager.GetInstance.UserGold += 10;
        GameManager.GetInstance.LocalUserScore += 1;
        GameManager.GetInstance.IsEnemyDown = true;
    }

    #region Object Pool
    /// <summary>
    /// EnemySpawer���� ������ Enemy ����
    /// �ش� Enemy Object�� ������ ���� Pool�� �˰� �־�� �Ѵ�
    /// </summary>
    /// <param name="pool"></param>
    public void SetManagePool(IObjectPool<Enemy> pool)
    {
        ManageEnemyPool = pool;
    }

    /// <summary>
    /// ������ Enemy Object�� pool�� ��ȯ�Ѵ�.
    /// GameManager�� LocalUser ������ Update ���ش�.
    /// GameManager�� ������ Update�϶�� ��ȣ�� �ش�.
    /// </summary>
    private void DestoryEnemy()
    {
        ManageEnemyPool.Release(this);
    }
    #endregion
}
