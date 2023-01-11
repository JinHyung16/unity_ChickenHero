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

    //object pool 관련
    private IObjectPool<Egg> ManageEggPool;
    private bool IsPoolRelease = false;

    private void OnEnable()
    {
        spriteRenderer.sortingOrder = 0;
        IsPoolRelease = false;

        anim.SetInteger("IsBreak", 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                HughUtility.Vibration.Vibrate(50);
            }
            DestroyEggCoroutine().Forget();
        }

    }
    
    private async UniTaskVoid DestroyEggCoroutine()
    {
        anim.SetInteger("IsBreak", 1);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
        DestroyEgg();
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
