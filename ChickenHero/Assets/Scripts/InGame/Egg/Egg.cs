using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour, IEggPower
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    //object pool ����
    private IObjectPool<Egg> ManageEggPool;

    private bool IsRelease = true;

    //interface ����
    [HideInInspector] public int Power { get; set; }

    private void OnEnable()
    {
        Power = 1;
        spriteRenderer.sortingOrder = 0;

        anim.SetInteger("IsBreak", 0);
        IsRelease = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    Handheld.Vibrate(); //�޴������� ���� �︮��
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
        if (!IsRelease)
        {
            ManageEggPool.Release(this);
            IsRelease = true;
        }
    }
    #endregion
}
