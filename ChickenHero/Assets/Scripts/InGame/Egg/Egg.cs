using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System;
using HughUtility;
using System.Threading;

public class Egg : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    //object pool ����
    private IObjectPool<Egg> ManageEggPool;
    private bool isPoolRelease = false;

    //UniTask ����
    private CancellationTokenSource tokenSource;

    private void OnEnable()
    {
        if (tokenSource != null)
        {
            tokenSource.Dispose();
        }
        tokenSource = new CancellationTokenSource();

        spriteRenderer.sortingOrder = 0;
        isPoolRelease = false;

        anim.SetInteger("IsBreak", 0);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
#if UNITY_ANDROID
                HughUtility.Vibration.Vibrate(50);
#endif
            }
            DestroyEggCoroutine().Forget();
        }

    }
    
    private async UniTaskVoid DestroyEggCoroutine()
    {
        anim.SetInteger("IsBreak", 1);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: tokenSource.Token);
        DestroyEgg();
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
    private void DestroyEgg()
    {   
        if (!isPoolRelease)
        {
            ManageEggPool.Release(this);
            isPoolRelease = true;
        }     
    }
    #endregion
}
