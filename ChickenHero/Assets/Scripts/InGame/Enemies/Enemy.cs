using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private Animator anim;

    [SerializeField] private Rigidbody2D rigid2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    //enemy move
    private Vector2 direction;
    [SerializeField] private float moveSpeed;
    private IEnumerator DirectionThink;
    [SerializeField] private float thinkTime;

    //enemy HP
    public int HP { get; set; }

    //enemy ������� �ð� �����ϱ� -> �ڵ����� ����
    private IEnumerator DeSpawnEnemy;
    [SerializeField] private float despawnTime;

    //object pooy
    private IObjectPool<Enemy> ManageEnemyPool; //�⺻ enemy�����ϴ� pool

    private void FixedUpdate()
    {
        EnemyAutoMove();
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
    }

    private void OnEnable()
    {
        HP = 10;
        spriteRenderer.sortingOrder = -2;

        despawnTime = 5.0f;
        DeSpawnEnemy = DespawnEnemyCoroutine();
        StartCoroutine(DeSpawnEnemy);

        moveSpeed = 3.0f;
        thinkTime = 3.0f;
        DirectionThink = ThinkMoveDirectionCoroutine();
        StartCoroutine(DirectionThink);
    }

    public void Damaged(int damage)
    {
        HP -= damage;

        anim.SetTrigger("IsHurt");

        if (HP <= 0)
        {
            DestoryEnemy();
            EnemyDieUpdateToDisplay();
        }
    }

    /// <summary>
    /// Enemy�� �ı��� �� ����Ǵ� Coroutine�̴�.
    /// 0.5�ʵڿ� Pool�� ��ȯ�ϰ� Animation�� ��ȯ��Ų��
    /// </summary>
    /// <returns> IEnumerator ��ȯ </returns>
    private IEnumerator DespawnEnemyCoroutine()
    {
        yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(despawnTime);
        DestoryEnemy();
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    private void EnemyAutoMove()
    {
        rigid2D.velocity = direction * moveSpeed;

        if (direction.x <= rigid2D.position.x && direction.y <= rigid2D.position.y)
        {
            thinkTime -= 1.0f;
        }
    }

    /// <summary>
    /// ������ ������ Ư�� �ð����� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns> IEnumerator ��ü ��ȯ </returns>
    private IEnumerator ThinkMoveDirectionCoroutine()
    {
        while (true)
        {
            float x = Random.Range(-1.0f, 1.0f);
            float y = Random.Range(-1.0f, 1.0f);

            direction = new Vector2(x, y);
            yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(thinkTime);
        }
    }

    private void EnemyDieUpdateToDisplay()
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
