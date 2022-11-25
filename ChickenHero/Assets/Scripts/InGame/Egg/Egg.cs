using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour, IEggPower
{  
    //object pool ����
    private IObjectPool<Egg> ManageEggPool;

    //interface ����
    [HideInInspector] public int Power { get; set; }


    private void OnEnable()
    {
        Power = 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Handheld.Vibrate(); //�޴������� ���� �︮��
            collision.gameObject.GetComponent<Enemy>().Damaged(Power);
            DestoryEgg();
        }
        else
        {
            DestoryEgg();
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
    private void DestoryEgg()
    {
        ManageEggPool.Release(this);
    }
    #endregion
}
