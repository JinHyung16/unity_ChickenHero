using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //enemy move
    private Vector2 direction;
    [SerializeField] private float moveSpeed;

    //object pooy
    private IObjectPool<Enemy> ManageEnemyPool; //기본 enemy관리하는 pool
    private bool isPoolRelease = false;

    //Enemy HP 관련
    private int enemyHP;

    //UniTask 관련
    private CancellationTokenSource tokenSource;

    private void OnEnable()
    {
        if (tokenSource != null)
        {
            tokenSource.Dispose();
        }
        tokenSource = new CancellationTokenSource();

        spriteRenderer.sortingOrder = -2;

        moveSpeed = 4.0f;

        AutoDespawnEnemy().Forget();
        TinkMoveDirection().Forget();

        enemyHP = 100;

        isPoolRelease = false;
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }

    private void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    private void FixedUpdate()
    {
        EnemyAutoMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CollisionObj"))
        {
            switch (collision.gameObject.name)
            {
                case "Top Collision":
                    direction.y *= -1;
                    break;
                case "Bottom Collision":
                    direction.y *= -1;
                    break;
                case "Right Collision":
                    direction.x *= -1;
                    break;
                case "Left Collision":
                    direction.x *= -1;
                    break;
            }
        }

        if (collision.gameObject.CompareTag("Egg"))
        {
            anim.SetTrigger("IsHurt");
            DamagedToEgg(LocalDataManager.GetInstance.Power);
        }
    }


    /// <summary>
    /// Egg.cs에서 Enemy와 충돌시 호출한다.
    /// </summary>
    /// <param name="damage"></param>
    private void DamagedToEgg(int damage)
    {
        this.enemyHP -= damage;
        if (enemyHP <= 0)
        {
            if (GameManager.GetInstance.isSingleplay)
            {
                SingleplayPresenter.GetInstance.UpdateEnemyDown();
                SingleplayPresenter.GetInstance.UpdateScoreInSingleplay();
            }
            else
            {
                MultiplayPresenter.GetInstance.UpdateLocalScoreInMultiplay();
            }

            DestroyEnemy();
        }
    }

    /// <summary>
    /// Enemy 생성 후, 죽지 않으면 자동으로 특정 시간 지난 후 사라지게 해주는 함수
    /// 사라지면서 Player를 때린다. 따라서 Player에게 데미지를 주는 함수들 호출해줘야 한다
    /// </summary>
    private async UniTaskVoid AutoDespawnEnemy()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(15.0f), cancellationToken: tokenSource.Token);
        if (GameManager.GetInstance.isSingleplay)
        {
            SingleplayPresenter.GetInstance.UpdateHPInSingleplay(10);
        }
        else
        {
            MultiplayPresenter.GetInstance.UpdateHPInMultiplay(10);
        }
        DestroyEnemy();
    }

    /// <summary>
    /// Direction 방향으로 FixedUpdate에서 실행되는 움직임 함수
    /// </summary>
    private void EnemyAutoMovement()
    {
        rigid2D.velocity = direction * moveSpeed;
    }

    /// <summary>
    /// UniTask Coroutine을 실행하는 움직임 방향 바꾸는 함수
    /// </summary>
    private async UniTaskVoid TinkMoveDirection()
    {
        while (true)
        {
            float x = UnityEngine.Random.Range(-1.0f, 1.0f);
            float y = UnityEngine.Random.Range(-1.0f, 1.0f);

            direction = new Vector2(x, y);
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: tokenSource.Token);
        }
    }

    #region Object Pool Function
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
    /// GameManager의 LocalUser 점수를 Update 해준다.
    /// GameManager의 점수를 Update하라고 신호를 준다.
    /// </summary>
    private void DestroyEnemy()
    {
        if (!isPoolRelease)
        {
            ManageEnemyPool.Release(this);
            isPoolRelease = true;
        }
    }
    #endregion
}
