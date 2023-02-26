using Cysharp.Threading.Tasks.Triggers;
using HughUtility.Observer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayManager : MonoBehaviour, SingleplaySubject
{
    #region Singleton
    private static SingleplayManager instance;
    public static SingleplayManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public int playerHP { get; set; }

    private int score; //���� ������ ���� ��� Score

    private int curStage = 1;
    private bool isStageClear = false;

    private int clearEnemyCount = 25;
    private int enemyDiedCntInStage = 0; //Stage���� ���� ���� �������

    private int earnAmountOfGold = 0;
    private void OnDisable()
    {
        RemoveAllObserver();
    }
    private void Start()
    {
        score = 0;
        clearEnemyCount = LocalData.GetInstance.GetEndGameRuleEnemyCount(curStage);
    }

    /// <summary>
    /// ���� ���� ���� �ڵ����� ���� ������鼭 Player�� HP�� ���ҽ�ų �� ȣ��ȴ�.
    /// SinglePlayCanvas���� UI�� Update�϶�� �˸���.
    /// </summary>
    /// <param name="hp">�پ�� �ϴ� hp�� ���� �ǹ�</param>
    public void UpdateHPInSingleplay(int hp)
    {
        playerHP -= hp;
        NotifyObservers(SingleplayNotifyType.HP);

        if (playerHP <= 0)
        {
            GameManager.GetInstance.GameEnd();
        }
    }

    /// <summary>
    /// Enemy�� Egg�� �¾� �����鼭 ������ Update��ų �� ȣ��ȴ�.
    /// SinglePlayCanvas���� UI�� Update�϶�� �˸���.
    /// </summary>
    public void UpdateScoreInSingleplay()
    {
        score++;
        NotifyObservers(SingleplayNotifyType.Score);
    }

    /// <summary>
    /// Enemy�� Egg�� �¾� �����鼭 ���� Enemy�� Count�� ������ų �� ȣ��ȴ�.
    /// SinglePlayCanvas���� UI�� Update�϶�� �˸���.
    /// ���� Stage Clear������ �����Ǿ��ٸ� Clear UI�� ���� Stage�� ���浵 �˸���.
    /// </summary>
    public void UpdateEnemyDown()
    {
        enemyDiedCntInStage++;
        isStageClear = false;
        if (clearEnemyCount <= enemyDiedCntInStage)
        {
            isStageClear = true;
            StageClearAndStageUp();
        }
        NotifyObservers(SingleplayNotifyType.EnemyDown);
    }

    /// <summary>
    /// Stage Clear�� �˸���, ���� Stage�� up ��Ų��.
    /// �̶�, ���� stage�� ���� ��ƾ� �ϴ� ������ ���� �����ͼ� �ٲ��ش�.
    /// </summary>
    private void StageClearAndStageUp()
    {
        curStage += 1;
        if (LocalData.GetInstance.isEndStageNum < curStage)
        {
            GameManager.GetInstance.GameEnd();
            return;
        }
        enemyDiedCntInStage = 0;
        clearEnemyCount = LocalData.GetInstance.GetEndGameRuleEnemyCount(curStage);
    }


    public void UpdateGameResultWhenEnd()
    {
        earnAmountOfGold = curStage * (score / 5);
        NotifyObservers(SingleplayNotifyType.GameEnd);
    }
    #region Observer pattern interface����
    private List<SingleplayObserver> observerList = new List<SingleplayObserver>();

    public void RegisterObserver(SingleplayObserver observer)
    {
        observerList.Add(observer);
    }

    public void RemoveAllObserver()
    {
        observerList.Clear();
    }

    public void NotifyObservers(SingleplayNotifyType notifyType)
    {
        foreach (var observer in observerList)
        {

            switch (notifyType)
            {
                case SingleplayNotifyType.None:
                    observer.UpdateHPText(playerHP);
                    observer.UpdateScoreText(score);
                    break;
                case SingleplayNotifyType.HP:
                    observer.UpdateHPText(playerHP);
                    observer.GetDamaged();
                    break;
                case SingleplayNotifyType.Score:
                    observer.UpdateScoreText(score);
                    break;
                case SingleplayNotifyType.EnemyDown:
                    observer.UpdateEnemyDownCountAndStageText(isStageClear, curStage);
                    break;
                case SingleplayNotifyType.GameEnd:
                    observer.ResultGameText(curStage, score, earnAmountOfGold);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}
