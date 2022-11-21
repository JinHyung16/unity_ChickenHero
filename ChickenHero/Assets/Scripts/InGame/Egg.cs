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
    /// 적 위치를 받았고, Egg가 생성되었으면 그 위치로 날아간다.
    /// </summary>
    private void ThrowingTarget()
    {
        transform.Translate(target * Time.deltaTime * speed);
    }

    /// <summary>
    /// PlayerInput에서 Egg 생성시켜 발사할 위치 전달받는다.
    /// </summary>
    /// <param name="_direction"> 적 위치를 받아서 Egg가 가야할 지점을 받는 매개변수 </param>
    public void ShootEgg(Vector2 _direction)
    {
        target = _direction;
    }

    /// <summary>
    /// PlayerInput에서 생성한 Egg Object를 넣어서 관리할 Pool
    /// 해당 Egg Object가 본인이 속한 Pool을 알고 있어야 한다
    /// </summary>
    /// <param name="pool"></param>
    public void SetManagePool(IObjectPool<Egg> pool)
    {
        ManageEggPool = pool;
    }

    /// <summary>
    /// 생성된 Egg object를 pool에 반환한다
    /// </summary>
    public void DestoryEgg()
    {
        ManageEggPool.Release(this);
    }



}
