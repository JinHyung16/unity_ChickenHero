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
    private IEnumerator DeSpawnEggCoroutine;

    private void OnEnable()
    {
        Power = 1;
        spriteRenderer.sortingOrder = 0;
        DeSpawnEggCoroutine = DespawnEgg();
    }

    private void OnDisable()
    {
        StopCoroutine(DeSpawnEggCoroutine);
    }

    private void OnDestroy()
    {
        StopCoroutine(DeSpawnEggCoroutine);
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
        anim.SetInteger("IsBreak", 1);
        // ���� ���� Animation ȣ�� �� �ش� frame �� �������� Release�ǰ� ����
        StartCoroutine(DeSpawnEggCoroutine);
    }
    
    /// <summary>
    /// Enemy�� �ı��� �� ����Ǵ� Coroutine�̴�.
    /// 0.5�ʵڿ� Pool�� ��ȯ�ϰ� Animation�� ��ȯ��Ų��
    /// </summary>
    /// <returns> IEnumerator ��ȯ </returns>
    private IEnumerator DespawnEgg()
    {
        while (true)
        {
            yield return HughUtility.Cashing.YieldInstruction.WaitForSeconds(0.1f);
            anim.SetInteger("IsBreak", 0);
            ManageEggPool.Release(this);
        }
    }
    #endregion
}
