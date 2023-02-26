using HughUtility.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerInputController : MonoBehaviour
{
    //������ ��ǥ ����
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;
    [HideInInspector] public bool IsThrow = false;

    //Egg Object Pooling ���� 
    [SerializeField] private GameObject eggPrefab;
    private IObjectPool<Egg> eggPool;

    private void OnEnable()
    {
        InitPlayerPooling();
    }

    private void Update()
    {
        //Attack();
        AttackTestWindows();
    }

    /// <summary>
    /// Player�� Egg Prefab�� Pooling�ϱ� ���� �ʿ��� �ʱ� ����
    /// </summary>
    private void InitPlayerPooling()
    {
        eggPool = new ObjectPool<Egg>(CreateEgg, OnGetEgg, OnReleaseEgg, OnDestroyEgg, maxSize: 100);
    }

    /// <summary>
    /// Play Mode���� ���� ���� �� ȭ�� ��ġ�ϸ� ����Ǵ� �Լ�
    /// </summary>
    private void Attack()
    {
        if (Input.touchCount > 0)
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
    /// Windowsȯ�濡�� �׽�Ʈ�ϰ��� ����� �Լ���
    /// ����� ����� �� ����������. �ȱ׷��� �ѹ��� 2�� �������ɷ� �Ǵ��ؼ�
    /// Egg�� Enemy�� �浹�� 2���� �Ͼ��.
    /// </summary>
    private void AttackTestWindows()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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


    /// <summary>
    /// �ް��� ������ �Լ�
    /// </summary>
    private void ThrowEgg()
    {
        var egg = eggPool.Get();
        egg.transform.position = targetVec;
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
    /// <param name="egg">Pool�� �ִ� Egg�� ������ �Ű�����</param>
    private void OnGetEgg(Egg egg)
    {
        egg.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool�� Object�� ������ �Լ�
    /// </summary>
    /// <param name="egg">Pool�� ������ Egg �Ű�����</param>
    private void OnReleaseEgg(Egg egg)
    {
        egg.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool���� Object�� �ı��� �� �� �Լ�
    /// </summary>
    /// <param name="egg">Pool���� �ı��� Object type�� �Ű�����</param>
    private void OnDestroyEgg(Egg egg)
    {
        Destroy(egg.gameObject);
    }
    #endregion
}
