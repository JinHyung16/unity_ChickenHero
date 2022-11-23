using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour, IEggPower
{
    private Vector2 target;
    [SerializeField] private float speed = 3.0f;
    
    //object pool ����
    private IObjectPool<Egg> ManageEggPool;

    //interface ����
    [HideInInspector] public int Power { get; set; }

    private void Start()
    {
        Power = 1;
    }

    private void Update()
    {
        ThrowingTarget();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().Damaged(Power);
            DestoryEgg();
        }
    }
    /// <summary>
    /// �� ��ġ�� �޾Ұ�, Egg�� �����Ǿ����� �� ��ġ�� ���ư���.
    /// </summary>
    private void ThrowingTarget()
    {
        transform.Translate(target * Time.deltaTime * speed);
    }

    /// <summary>
    /// PlayerInput���� Egg �������� �߻��� ��ġ ���޹޴´�.
    /// </summary>
    /// <param name="_direction"> �� ��ġ�� �޾Ƽ� Egg�� ������ ������ �޴� �Ű����� </param>
    public void ShootEgg(Vector2 _direction)
    {
        target = _direction;
    }


    #region Object Pool Manage Function
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
