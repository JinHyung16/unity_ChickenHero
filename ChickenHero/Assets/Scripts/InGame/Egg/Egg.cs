using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;
using HughUtility;

public class Egg : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    //object pool ����
    private IObjectPool<Egg> ManageEggPool;
    private bool IsPoolRelease = false;

    //Egg Power
    [SerializeField] private int eggPower;

    private void OnEnable()
    {
        eggPower = 1;
        spriteRenderer.sortingOrder = 0;
        IsPoolRelease = false;

        anim.SetInteger("IsBreak", 0);
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
#if UNITY_ANDROID
                HughUtility.Vibration.Vibrate((long)100);
#endif
                collision.gameObject.GetComponent<Enemy>().DamagedToEgg(eggPower);
            }

            await DestroyEggCoroutine();
        }

    }

    private IEnumerator DestroyEggCoroutine() => UniTask.ToCoroutine(async () =>
    {
        anim.SetInteger("IsBreak", 1);
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f), ignoreTimeScale: false);
        DestroyEgg();
    });

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
    private void DestroyEgg()
    {   
        if (!IsPoolRelease)
        {
            ManageEggPool.Release(this);
            IsPoolRelease = true;
        }     
    }
    #endregion
}
