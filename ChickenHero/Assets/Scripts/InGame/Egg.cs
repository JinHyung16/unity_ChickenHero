using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour
{
    private Vector2 target;
    [SerializeField] private float speed = 3.0f;

    private IObjectPool<Egg> ManageEggPool;

    private void Update()
    {
        ThrowingTarget();
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

    /// <summary>
    /// PlayerInput���� ������ Egg Object�� �־ ������ Pool
    /// �ش� Egg Object�� ������ ���� Pool�� �˰� �־�� �Ѵ�
    /// </summary>
    /// <param name="pool"></param>
    public void SetManagePool(IObjectPool<Egg> pool)
    {
        ManageEggPool = pool;
    }

    /// <summary>
    /// ������ Egg object�� pool�� ��ȯ�Ѵ�
    /// </summary>
    public void DestoryEgg()
    {
        ManageEggPool.Release(this);
    }



}
