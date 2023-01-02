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
    private bool IsRelease = false;

    //Enemy HP ����
    private int enemyHP;

    //UniTask ����
    private CancellationTokenSource tokenSource;

    private void OnEnable()
    {
        tokenSource = new CancellationTokenSource();
        spriteRenderer.sortingOrder = -2;

        moveSpeed = 3.0f;

        AutoDespawnEnemy().Forget();
        TinkMoveDirection().Forget();

        enemyHP = 4;

        IsRelease = false;
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
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
        }
    }

    public void DamagedToEgg(int damage)
    {
        this.enemyHP -= damage;
        if (enemyHP <= 0)
        {
            Debug.Log("Enemy HP: " + enemyHP);

            tokenSource.Cancel();
            DestroyEnemy();
        }
    }

    /// <summary>
    /// Enemy ���� ��, ���� ������ �ڵ����� Ư�� �ð� ���� �� ������� ���ִ� �Լ�
    /// </summary>
    private async UniTaskVoid AutoDespawnEnemy()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false, cancellationToken: tokenSource.Token);
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
            await UniTask.Delay(TimeSpan.FromSeconds(3), ignoreTimeScale: false, cancellationToken: tokenSource.Token);
        }
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
    private void DestroyEnemy()
    {
        if (!IsRelease)
        {
            ManageEnemyPool.Release(this);
            IsRelease = true;
        }
    }
    #endregion
}
