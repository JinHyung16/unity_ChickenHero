using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour, IEggPower
{
    [SerializeField] private Rigidbody2D rigid2D;

    private Vector2 target;
    [SerializeField] private float throwSpeed = 3.0f;
    
    //object pool 관련
    private IObjectPool<Egg> ManageEggPool;

    //interface 구현
    [HideInInspector] public int Power { get; set; }


    private void OnEnable()
    {
        rigid2D.bodyType = RigidbodyType2D.Kinematic;
        if (rigid2D == null)
        {
            rigid2D = GetComponent<Rigidbody2D>();
        }
        Power = 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Handheld.Vibrate(); //휴대폰에서 진동 울리기
            collision.gameObject.GetComponent<Enemy>().Damaged(Power);
            DestoryEgg();
        }
    }

    #region Object Pool Manage Function
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
        ManageEggPool.Release(this);
    }
    #endregion
}
