using Cysharp.Threading.Tasks.Triggers;
using HughUtility.Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayPresenter : MonoBehaviour
{
    #region Static
    public static SingleplayPresenter GetInstance;
    private void Awake()
    {
        GetInstance = this;
    }
    #endregion

    [SerializeField] private SingleplayViewer singleplayViewer;
    public int playerHP { get; set; }

    private int score; //적을 잡을때 마다 얻는 Score

    private int curStage = 1;
    private bool isStageClear = false;

    private int clearEnemyCount = 25;
    private int enemyDiedCntInStage = 0; //Stage마다 죽인 적이 몇마리인지

    private int earnAmountOfGold = 0;
    private void Start()
    {
        score = 0;
        clearEnemyCount = DataManager.GetInstance.GetEndGameRuleEnemyCount(curStage);
    }

    public void InitSinglePlayStart()
    {
        singleplayViewer.UpdateHPText(playerHP);
        singleplayViewer.UpdateScoreText(score);
    }

    /// <summary>
    /// 적을 잡지 못해 자동으로 적이 사라지면서 Player의 HP를 감소시킬 때 호출된다.
    /// SinglePlayCanvas에게 UI를 Update하라고 알린다.
    /// </summary>
    /// <param name="hp">줄어야 하는 hp의 양을 의미</param>
    public void UpdateHPInSingleplay(int hp)
    {
        playerHP -= hp;

        singleplayViewer.UpdateHPText(playerHP);
        singleplayViewer.GetDamaged();

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
        singleplayViewer.UpdateScoreText(score);
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
        singleplayViewer.UpdateEnemyDownCountAndStageText(isStageClear, curStage);
    }

    /// <summary>
    /// Stage Clear를 알리고, 현재 Stage를 up 시킨다.
    /// 이때, 현재 stage에 따른 잡아야 하는 몬스터의 수도 가져와서 바꿔준다.
    /// </summary>
    private void StageClearAndStageUp()
    {
        curStage += 1;
        if (DataManager.GetInstance.isEndStageNum < curStage)
        {
            GameManager.GetInstance.GameEnd();
            return;
        }
        enemyDiedCntInStage = 0;
        clearEnemyCount = DataManager.GetInstance.GetEndGameRuleEnemyCount(curStage);
    }


    public void UpdateGameResultWhenEnd()
    {
        earnAmountOfGold = (score % 10) * curStage;
        DataManager.GetInstance.Gold += earnAmountOfGold;
        singleplayViewer.ResultGameText(curStage, score, earnAmountOfGold);
    }
}
