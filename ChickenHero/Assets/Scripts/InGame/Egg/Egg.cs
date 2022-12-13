using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class Egg : MonoBehaviour, IEggPower
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    //object pool 관련
    private IObjectPool<Egg> ManageEggPool;
    private bool IsPoolRelease = false;

    //interface 구현
    [HideInInspector] public int Power { get; set; }

    private void OnEnable()
    {
        Power = 1;
        spriteRenderer.sortingOrder = 0;
        IsPoolRelease = false;

        anim.SetInteger("IsBreak", 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
#if UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                    collision.gameObject.GetComponent<Enemy>().Damaged(Power);
                    break;
            }
            anim.SetInteger("IsBreak", 1);
            Invoke("DestoryEgg", 0.1f);
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
    public void DestoryEgg()
    {
        if (!IsPoolRelease)
        {
            ManageEggPool.Release(this);
            IsPoolRelease = true;
        }
    }
    #endregion
}
