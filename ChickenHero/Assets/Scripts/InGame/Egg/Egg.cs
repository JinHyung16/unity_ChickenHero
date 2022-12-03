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

    //interface ����
    [HideInInspector] public int Power { get; set; }

    //Egg ������� �ð� �����ϴ� �ڷ�ƾ
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
            Handheld.Vibrate(); //�޴������� ���� �︮��
            collision.gameObject.GetComponent<Enemy>().Damaged(Power);
            DestoryEgg();
        }
        else if (collision.CompareTag("CollisionObj"))
        {
            DestoryEgg();
        }
    }

    /// <summary>
    /// Enemy�� �ı��� �� ����Ǵ� Coroutine�̴�.
    /// 0.5�ʵڿ� Pool�� ��ȯ�ϰ� Animation�� ��ȯ��Ų��
    /// </summary>
    /// <returns> IEnumerator ��ȯ </returns>
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
        anim.SetInteger("IsBreak", 1);
        // ���� ���� Animation ȣ�� �� �ش� frame �� �������� Release�ǰ� ����
        StartCoroutine(DeSpawnEgg);
    }
    #endregion
}
