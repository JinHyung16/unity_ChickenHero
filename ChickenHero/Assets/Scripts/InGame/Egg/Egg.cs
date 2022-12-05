using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour, IEggPower
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    //object pool 관련
    private IObjectPool<Egg> ManageEggPool;

    private bool IsRelease = true;

    private IEnumerator DeSpawn;
    //interface 구현
    [HideInInspector] public int Power { get; set; }

    private void OnEnable()
    {
        Power = 1;
        spriteRenderer.sortingOrder = 0;

        anim.SetInteger("IsBreak", 0);
        IsRelease = false;

        DeSpawn = DeSpawnCoroutine();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    HughUtility.Vibration.Vibrate(1);
                    collision.gameObject.GetComponent<Enemy>().Damaged(Power);
                    break;
            }
            anim.SetInteger("IsBreak", 1);
            StartCoroutine(DeSpawn);
        }
    }

    private IEnumerator DeSpawnCoroutine()
    {
        yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(0.5f);
        DestoryEgg();
    }

    #region Object Pool Function
    /// <summary>
    /// PlayerInput에서 생성한 Egg Object를 넣어서 관리할 Pool
    /// 해당 Egg Object가 본인이 속한 Pool을 알고 있어야 한다
    /// </summary>
    /// <param name="pool"> Egg를 Pooling한 쪽에서 전달하는 매개변수 </param>
    public void SetManagePool(IObjectPool<Egg> pool)
    {
        ManageEggPool = pool;
    }

    /// <summary>
    /// 생성된 Egg object를 pool에 반환한다
    /// </summary>
    public void DestoryEgg()
    {
        if (!IsRelease)
        {
            ManageEggPool.Release(this);
            IsRelease = true;
        }
    }
    #endregion
}
