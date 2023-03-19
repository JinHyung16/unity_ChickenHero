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
    private IObjectPool<Enemy> ManageEnemyPool; //�⺻ enemy�����ϴ� pool
    private bool isPoolRelease = false;

    //Enemy HP ����
    private int enemyHP;

    //UniTask ����
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
    /// Egg.cs���� Enemy�� �浹�� ȣ���Ѵ�.
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
    /// Enemy ���� ��, ���� ������ �ڵ����� Ư�� �ð� ���� �� ������� ���ִ� �Լ�
    /// ������鼭 Player�� ������. ���� Player���� �������� �ִ� �Լ��� ȣ������� �Ѵ�
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
    /// Direction �������� FixedUpdate���� ����Ǵ� ������ �Լ�
    /// </summary>
    private void EnemyAutoMovement()
    {
        rigid2D.velocity = direction * moveSpeed;
    }

    /// <summary>
    /// UniTask Coroutine�� �����ϴ� ������ ���� �ٲٴ� �Լ�
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
