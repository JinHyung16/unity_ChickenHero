using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    // ������ �� ����
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;
    [HideInInspector] public bool IsThrow = false;

    // Egg Object Pooling ���� 
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private int eggCount = 0;
    private IObjectPool<Egg> eggPool;

    private void Update()
    {
        Attack();
    }

    private void OnEnable()
    {
        InitPlayerPooling();
    }

    /// <summary>
    /// Player�� Egg Prefab�� Pooling�ϱ� ���� �ʿ��� �ʱ� ����
    /// </summary>
    private void InitPlayerPooling()
    {
        eggCount = 20;
        eggPool = new ObjectPool<Egg>(CreateEgg, OnGetEgg, OnReleaseEgg, OnDestroyEgg, maxSize: eggCount);
    }

    /// <summary>
    /// Play Mode���� ���� ���� �� ȭ�� ��ġ�ϸ� ����Ǵ� �Լ�
    /// ���� Enemy�� Ÿ������ �� �Ӹ� �ƴ϶� �ش� ��ġ�� �� �ް� �������� �����Ҽ��� ����
    /// </summary>
    private void Attack()
    {
        if (Input.touchCount > 0 && GameManager.GetInstance.IsGameStart)
        {
            Touch myTouch = Input.GetTouch(0);
            if (myTouch.phase == TouchPhase.Began)
            {
                targetVec = Camera.main.ScreenToWorldPoint(myTouch.position);
                RaycastHit2D hit2D = Physics2D.Raycast(targetVec, targetVec);
                if (hit2D.collider != null)
                {
                    IsThrow = true;
                    ThrowEgg();
                }
            }
            else
            {
                IsThrow = false;
            }
        }
    }

    /// <summary>
    /// �ް��� ������ �Լ�
    /// </summary>
    private void ThrowEgg()
    {
        var egg = eggPool.Get();
        egg.transform.position = targetVec;
    }

    public void PlayerDieAnimation()
    {
        Debug.Log("�÷��̾� ����");
    }


    #region Object Pool Function
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
