using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class PlayerInput : MonoBehaviour, IPointerDownHandler
{
    // 던지는 것 관련
    private bool IsThrow = false;
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;

    // Egg Object Pooling 관련 
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private int eggCount = 0;
    private IObjectPool<Egg> eggPool;


    private void Awake()
    {
        eggPool = new ObjectPool<Egg>(CreateEgg, OnGetEgg, OnReleaseEgg, OnDestroyEgg, maxSize:eggCount);
    }

    private void Update()
    {
        if (IsThrow)
        {
            var direction = new Vector2(targetVec.x, targetVec.y);
            var egg = eggPool.Get();
            egg.transform.position = playerTrans.position;
            egg.ShootEgg(direction.normalized);
        }
    }

    /// <summary>
    /// Unity에서 제공하는 IPointerDownHandler interface에 속한 함수
    /// Touch받을 GameObject는 반득시 Collider가 붙어있어야한다.
    /// </summary>
    /// <param name="pointer"> pointer 된 지점의 값을 넣어준다 </param>
    public virtual void OnPointerDown(PointerEventData pointer)
    {
        if (pointer.pointerClick.gameObject.CompareTag("Enemy"))
        {
            targetVec = pointer.position;
            IsThrow = true;
        }
    }

    #region Object Pool
    /// <summary>
    /// Egg Object를 생성한다.
    /// 생성된 후 Egg Object에 자신이 등록되어야 할 Pool을 알려주고 반환한다.
    /// </summary>
    /// <returns> Egg type을 반환한다 </returns>
    private Egg CreateEgg()
    {
        Egg egg = Instantiate(eggPrefab).GetComponent<Egg>();
        egg.SetManagePool(eggPool);
        return egg;
    }

    /// <summary>
    /// Pool에서 Object를 가져올 함수
    /// </summary>
    /// <param name="e">Pool에 있는 Egg를 가져올 매개변수</param>
    private void OnGetEgg(Egg e)
    {
        e.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool에 Object를 돌려줄 함수
    /// </summary>
    /// <param name="e">Pool에 돌려줄 Egg 매개변수</param>
    private void OnReleaseEgg(Egg e)
    {
        e.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool에서 Object를 파괴할 때 쓸 함수
    /// </summary>
    /// <param name="e">Pool에서 파괴할 Object type의 매개변수</param>
    private void OnDestroyEgg(Egg e)
    {
        Destroy(e.gameObject);
    }
    #endregion
}
