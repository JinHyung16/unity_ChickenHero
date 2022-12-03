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

    //interface 구현
    [HideInInspector] public int Power { get; set; }

    //Egg 사라지는 시간 관리하는 코루틴
    private IEnumerator DeSpawnEgg;

    private void OnEnable()
    {
        Power = 1;
        spriteRenderer.sortingOrder = 0;
        DeSpawnEgg = DespawnEggCoroutine();
    }

    private void OnDisable()
    {
        StopCoroutine(DeSpawnEgg);
    }

    private void OnDestroy()
    {
        StopCoroutine(DeSpawnEgg);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Handheld.Vibrate(); //휴대폰에서 진동 울리기
            collision.gameObject.GetComponent<Enemy>().Damaged(Power);
            DestoryEgg();
        }
        else if (collision.CompareTag("CollisionObj"))
        {
            DestoryEgg();
        }
    }

    /// <summary>
    /// Enemy가 파괴될 때 실행되는 Coroutine이다.
    /// 0.5초뒤에 Pool에 반환하고 Animation을 변환시킨다
    /// </summary>
    /// <returns> IEnumerator 반환 </returns>
    private IEnumerator DespawnEggCoroutine()
    {
        while (true)
        {
            yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(0.05f);
            anim.SetInteger("IsBreak", 0);
            ManageEggPool.Release(this);
        }
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
    private void DestoryEgg()
    {
        anim.SetInteger("IsBreak", 1);
        // 깨란 깨진 Animation 호출 후 해당 frame 다 끝난다음 Release되게 수정
        StartCoroutine(DeSpawnEgg);
    }
    #endregion
}
