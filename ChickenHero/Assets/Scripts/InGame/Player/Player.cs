using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class Player : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //player died event
    public PlayerDiedEvent dieEvent;

    // ������ �� ����
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;
    [HideInInspector] public bool IsThrow = false;
    [HideInInspector] public int HitPoint;

    // Egg Object Pooling ���� 
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private int eggCount = 0;
    private IObjectPool<Egg> eggPool;

    private void OnEnable()
    {
        InitPlayerPooling();
    }

    public void PlayerDieAnimation()
    {
        Debug.Log("�÷��̾� ����");
    }

    private void InitPlayerPooling()
    {
        eggCount = 20;
        eggPool = new ObjectPool<Egg>(CreateEgg, OnGetEgg, OnReleaseEgg, OnDestroyEgg, maxSize: eggCount);
        if (dieEvent == null)
        {
            dieEvent = new PlayerDiedEvent();
        }
    }

    private void ThrowEgg()
    {
        var direction = new Vector2(targetVec.x, targetVec.y);
        var egg = eggPool.Get();
        egg.transform.position = playerTrans.position;
        egg.ShootEgg(direction.normalized);
    }

    /// <summary>
    /// GameManager���� Matching �� Local Player�� Die Event�� �������ִ� �Լ�
    /// </summary>
    public void DeadDueToTimeDone()
    {
        dieEvent.Invoke(gameObject);
    }

    #region Touch Event Function
    /// <summary>
    /// Unity���� �����ϴ� IPointerDownHandler interface�� ���� �Լ�
    /// Touch Down�� ����ȴ�.
    /// Touch���� GameObject�� �ݵ�� Collider�� �پ��־���Ѵ�.
    /// </summary>
    /// <param name="pointer"> ȭ��� ��ġ�� ������ �޴� �Ű����� </param>
    public void OnPointerDown(PointerEventData pointer)
    {
        if (pointer.pointerClick.gameObject.CompareTag("Enemy"))
        {
            targetVec = pointer.position;
            ThrowEgg();
            HitPoint = 50;
        }
    }

    /// <summary>
    /// Unity���� �����ϴ� IPointerUpHandler interface�� ���� �Լ�
    /// Touch Up�� ����Ǵµ�, �̶� IsThrow�� flase�� ������Ѵ�.
    /// </summary>
    /// <param name="pointer"> ȭ��� ��ġ�� ������ �޴� �Ű����� </param>
    public void OnPointerUp(PointerEventData pointer)
    {
        IsThrow = false;
    }
    #endregion

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
