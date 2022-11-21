using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class PlayerInput : MonoBehaviour, IPointerDownHandler
{
    // ������ �� ����
    private bool IsThrow = false;
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;

    // Egg Object Pooling ���� 
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
    /// Unity���� �����ϴ� IPointerDownHandler interface�� ���� �Լ�
    /// Touch���� GameObject�� �ݵ�� Collider�� �پ��־���Ѵ�.
    /// </summary>
    /// <param name="pointer"> pointer �� ������ ���� �־��ش� </param>
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
    /// Egg Object�� �����Ѵ�.
    /// ������ �� Egg Object�� �ڽ��� ��ϵǾ�� �� Pool�� �˷��ְ� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns> Egg type�� ��ȯ�Ѵ� </returns>
    private Egg CreateEgg()
    {
        Egg egg = Instantiate(eggPrefab).GetComponent<Egg>();
        egg.SetManagePool(eggPool);
        return egg;
    }

    /// <summary>
    /// Pool���� Object�� ������ �Լ�
    /// </summary>
    /// <param name="e">Pool�� �ִ� Egg�� ������ �Ű�����</param>
    private void OnGetEgg(Egg e)
    {
        e.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool�� Object�� ������ �Լ�
    /// </summary>
    /// <param name="e">Pool�� ������ Egg �Ű�����</param>
    private void OnReleaseEgg(Egg e)
    {
        e.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool���� Object�� �ı��� �� �� �Լ�
    /// </summary>
    /// <param name="e">Pool���� �ı��� Object type�� �Ű�����</param>
    private void OnDestroyEgg(Egg e)
    {
        Destroy(e.gameObject);
    }
    #endregion
}
