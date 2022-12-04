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

    //enemy 사라지는 시간 관리하기 -> 자동으로 해제
    private IEnumerator DeSpawnEnemy;
    [SerializeField] private float despawnTime;

    //object pooy
    private IObjectPool<Enemy> ManageEnemyPool; //기본 enemy관리하는 pool

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
    /// Enemy가 파괴될 때 실행되는 Coroutine이다.
    /// 0.5초뒤에 Pool에 반환하고 Animation을 변환시킨다
    /// </summary>
    /// <returns> IEnumerator 반환 </returns>
    private IEnumerator DespawnEnemyCoroutine()
    {
        yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(despawnTime);
        DestoryEnemy();
    }

    /// <summary>
    /// 움직임 관련
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
    /// 움직일 방향을 특정 시간마다 설정하는 코루틴
    /// </summary>
    /// <returns> IEnumerator 객체 반환 </returns>
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
    private void DestoryEnemy()
    {
        ManageEnemyPool.Release(this);
    }
    #endregion
}
