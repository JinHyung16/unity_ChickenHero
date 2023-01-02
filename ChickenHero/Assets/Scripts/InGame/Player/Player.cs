using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    [SerializeField] private ChickenData chickenData;

    //chicken data 관련 바인딩할 변수
    private SpriteRenderer spriteRenderer;
    private int playerHP;
    public int PlayerHP
    {
        get
        {
            return playerHP;
        }
    }
    // 던지는 것 관련
    private Vector2 targetVec = Vector2.zero;
    [SerializeField] private Transform playerTrans;
    [HideInInspector] public bool IsThrow = false;

    // Egg Object Pooling 관련 
    [SerializeField] private GameObject eggPrefab;
    private IObjectPool<Egg> eggPool;

    private void OnEnable()
    {
        InitChickenData();
        InitPlayerPooling();
    }

    private void Update()
    {
        Attack();
    }

    private void InitChickenData()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = chickenData.playerSprite;
        playerHP = chickenData.chickenHP;
    }

    /// <summary>
    /// Player가 Egg Prefab을 Pooling하기 위해 필요한 초기 설정
    /// </summary>
    private void InitPlayerPooling()
    {
        eggPool = new ObjectPool<Egg>(CreateEgg, OnGetEgg, OnReleaseEgg, OnDestroyEgg, true, 20, maxSize: 10000);
    }

    /// <summary>
    /// Play Mode에서 게임 진행 시 화면 터치하면 실행되는 함수
    /// 추후 Enemy를 타겟했을 때 뿐만 아니라 해당 위치에 걍 달걀 던지도록 수정할수도 있음
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
    /// 달걀을 던지는 함수
    /// </summary>
    private void ThrowEgg()
    {
        var egg = eggPool.Get();
        egg.transform.position = targetVec;
    }

    #region Object Pool Function
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
    /// <param name="egg">Pool에 있는 Egg를 가져올 매개변수</param>
    private void OnGetEgg(Egg egg)
    {
        egg.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pool에 Object를 돌려줄 함수
    /// </summary>
    /// <param name="egg">Pool에 돌려줄 Egg 매개변수</param>
    private void OnReleaseEgg(Egg egg)
    {
        egg.gameObject.SetActive(false);
    }

    /// <summary>
    /// Pool에서 Object를 파괴할 때 쓸 함수
    /// </summary>
    /// <param name="egg">Pool에서 파괴할 Object type의 매개변수</param>
    private void OnDestroyEgg(Egg egg)
    {
        Destroy(egg.gameObject);
    }
    #endregion
}
