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

    //object pool ����
    private IObjectPool<Egg> ManageEggPool;
    private bool IsPoolRelease = false;

    //interface ����
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
    /// PlayerInput���� ������ Egg Object�� �־ ������ Pool
    /// �ش� Egg Object�� ������ ���� Pool�� �˰� �־�� �Ѵ�
    /// </summary>
    /// <param name="pool"> Egg�� Pooling�� �ʿ��� �����ϴ� �Ű����� </param>
    public void SetManagePool(IObjectPool<Egg> pool)
    {
        ManageEggPool = pool;
    }

    /// <summary>
    /// ������ Egg object�� pool�� ��ȯ�Ѵ�
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
