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

    private int score; //적을 잡을때 마다 얻는 Score

    private int curStage = 1;
    private bool isStageClear = false;

    private int clearEnemyCount = 25;
    private int enemyDiedCntInStage = 0; //Stage마다 죽인 적이 몇마리인지

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
    /// 적을 잡지 못해 자동으로 적이 사라지면서 Player의 HP를 감소시킬 때 호출된다.
    /// SinglePlayCanvas에게 UI를 Update하라고 알린다.
    /// </summary>
    /// <param name="hp">줄어야 하는 hp의 양을 의미</param>
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
    /// Enemy가 Egg에 맞아 죽으면서 점수를 Update시킬 때 호출된다.
    /// SinglePlayCanvas에게 UI를 Update하라고 알린다.
    /// </summary>
    public void UpdateScoreInSingleplay()
    {
        score++;
        NotifyObservers(SingleplayNotifyType.Score);
    }

    /// <summary>
    /// Enemy가 Egg에 맞아 죽으면서 죽인 Enemy의 Count를 증가시킬 때 호출된다.
    /// SinglePlayCanvas에게 UI를 Update하라고 알린다.
    /// 만약 Stage Clear조건이 만족되었다면 Clear UI를 띄우고 Stage의 변경도 알린다.
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
    /// Stage Clear를 알리고, 현재 Stage를 up 시킨다.
    /// 이때, 현재 stage에 따른 잡아야 하는 몬스터의 수도 가져와서 바꿔준다.
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
    #region Observer pattern interface구현
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
