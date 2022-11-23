using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamage
{
    public int HP { get; set; }

    //object pooy
    private IObjectPool<Enemy> ManageEnemyPool; //기본 enemy관리하는 pool

    private void Start()
    {
        HP = 10;
    }

    public void Damaged(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            DestoryEnemy();
        }
    }

    #region Object Pool
    /// <summary>
    /// EnemySpawer에서 생성한 Enemy 관리
    /// 해당 Enemy Object가 본인이 속한 Pool을 알고 있어야 한다
    /// </summary>
    /// <param name="pool"></param>
    public void SetManagePool(IObjectPool<Enemy> pool)
    {
        ManageEnemyPool = pool;
    }

    /// <summary>
    /// 생성된 Enemy Object를 pool에 반환한다.
    /// GameManager의 점수를 Update하라고 신호를 준다.
    /// </summary>
    private void DestoryEnemy()
    {
        ManageEnemyPool.Release(this);
        
        GameManager.GetInstance.LocalUserScore += 1;
        GameManager.GetInstance.IsScoreUpdate = true;
    }
    #endregion
}
