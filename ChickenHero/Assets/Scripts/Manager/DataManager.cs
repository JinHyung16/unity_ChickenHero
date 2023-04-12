using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using HughUtility;

public class DataManager : Singleton<DataManager>
{
    /// <summary>
    /// PlayerPrefs을 이용해 OffLine일 때 사용할 Local Data
    /// User의 레벨, 골드량만을 저장해두고 Server연동되면 동기화 할 목적도 포함
    /// </summary>

    private void Start()
    {
        ReadShopCSVData();
        ReadEndGameRuleCSVData();
    }

    #region PlayerPrefs Save Data Property
    private string userName = string.Empty;
    public string Name
    {
        get
        {
            return PlayerPrefs.GetString(userName);
        }
        set
        {
            userName = value;
            PlayerPrefs.SetString(userName, value);
        }
    }

    public int Gold
    {
        get
        {
            return PlayerPrefs.GetInt(userName + "Gold");
        }
        set
        {
            PlayerPrefs.SetInt(userName + "Gold", value);
        }
    }

    public int Power
    {
        get
        {
            return PlayerPrefs.GetInt(userName + "Power");
        }
        set
        {
            PlayerPrefs.SetInt(userName + "Power", value);
        }
    }

    public int UpgradeLevel
    {
        get
        {
            return PlayerPrefs.GetInt(userName + "UpgradeLevel");
        }
        set
        {
            PlayerPrefs.SetInt(userName + "UpgradeLevel", value);
        }
    }

    private void PlayerPrefsClear()
    {
        var name = PlayerPrefs.GetString(Name);
        PlayerPrefs.DeleteKey(name);
        PlayerPrefs.DeleteKey(name + "Gold");
        PlayerPrefs.DeleteKey(name + "Power");
        PlayerPrefs.DeleteKey(name + "UpgradeLevel");
    }
    #endregion

    #region PlayerPrefs Control Function
    /// <summary>
    /// 로컬 PlayerPrefs초기화 시 사용할 함수
    /// 로그인 되어있다면 서버의 데이터도 지워준다.
    /// </summary>
    public void ClearAllUserInfo()
    {
        if (GameServer.GetInstance.GetIsServerConnect())
        {
            GameServer.GetInstance.RemoveUserInfoServer(GameServer.GetInstance.userid);
        }

        PlayerPrefsClear();
#if UNITY_EDITOR
        Debug.Log("<color=black><br> Delete All User Info PlayerPrefs </br></color>");
#endif
    }

    /// <summary>
    /// PlayerPrefs에 해당 유저 정보가 저장되어 있는지 확인한다.
    /// 모든 key에는 User의 이름이 포함되어 있어서 이름 하나만으로 확인
    /// </summary>
    /// <param name="name"> 유저의 Nick Name을 받는다 </param>
    /// <returns> PlayerPrefs에 해당 Nick Name이 있으면 true를 없으면 false를 리턴</returns>
    public bool CheckForUserInfo(string name)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(name)))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region CSV Shop Data Contorller Functions

    //게임 시작시 최초로 현재 업그레이드 코스트가 어딘지 가져올때 사용
    public int PickCost { get; private set; }

    private Dictionary<string, int> UpgradeCostDictionary = new Dictionary<string, int>();
    
    private void ReadShopCSVData()
    {
        string shopFile = "CSVData/ShopData";
        List<Dictionary<string, string>> csvDataList = CSVReader.ReadFile(shopFile);

        for (int i = 0; i < csvDataList.Count; i++)
        {
            string level = csvDataList[i]["UpgradeLevel"].ToString();
            int cost = int.Parse(csvDataList[i]["UpgradeCost"].ToString(), System.Globalization.NumberStyles.Integer);
            int pickGold = int.Parse(csvDataList[i]["PickCost"].ToString(), System.Globalization.NumberStyles.Integer);
            if (pickGold != 0)
            {
                PickCost = pickGold;
            }
            UpgradeCostDictionary.Add(level, cost);
        }
    }

    /// <summary>
    /// level을 10으로 나눠 몫을 통해 구간 파악
    /// 0이면 1~10, 1이면 11~20 etc...
    /// </summary>
    /// <param name="level"> 현재 유저의 업그레이드 레벨을 받는다</param>
    /// <returns>해당 업그레이드 구간에 맞는 업그레이드 비용을 리턴</returns>
    public int GetUpgradeCost(string level)
    {
        int upgradeCost = UpgradeCostDictionary["1"];
        switch (int.Parse(level) / 10)
        {
            case 0:
                upgradeCost = UpgradeCostDictionary["1"];
                break;
            case 1:
                upgradeCost = UpgradeCostDictionary["11"];
                break;
            case 2:
                upgradeCost = UpgradeCostDictionary["21"];
                break;
            case 3:
                upgradeCost = UpgradeCostDictionary["31"];
                break;
            case 4:
                upgradeCost = UpgradeCostDictionary["41"];
                break;
            case 5:
                upgradeCost = UpgradeCostDictionary["51"];
                break;
            case 6:
                upgradeCost = UpgradeCostDictionary["61"];
                break;
            case 7:
                upgradeCost = UpgradeCostDictionary["71"];
                break;
            case 8:
                upgradeCost = UpgradeCostDictionary["81"];
                break;
            case 9:
                upgradeCost = UpgradeCostDictionary["91"];
                break;
            case 10:
                upgradeCost = UpgradeCostDictionary["101"];
                break;

        }

        return upgradeCost;
    }
    #endregion

    #region CSV End Rule Data Controller Functions;
    private Dictionary<int, int> EndGameRuleDictionary = new Dictionary<int, int>();

    public int isEndStageNum { get; private set; } = 0;
    private int endGameRuleEnemyCnt;
    private void ReadEndGameRuleCSVData()
    {
        string endGameRuleFile = "CSVData/EndGameRuleData";
        List<Dictionary<string, string>> csvDataList = CSVReader.ReadFile(endGameRuleFile);

        for (int i = 0; i < csvDataList.Count; i++)
        {
            int stage = int.Parse(csvDataList[i]["Stage"].ToString(), System.Globalization.NumberStyles.Integer);
            int endRuleCnt = int.Parse(csvDataList[i]["EndRule"].ToString(), System.Globalization.NumberStyles.Integer);
            int endStage = int.Parse(csvDataList[i]["EndStage"].ToString(), System.Globalization.NumberStyles.Integer);
            if (endStage == 1)
            {
                isEndStageNum = stage;
            }
            EndGameRuleDictionary.Add(stage, endRuleCnt);
        }
    }

    public int GetEndGameRuleEnemyCount(int curStage)
    {
        endGameRuleEnemyCnt = EndGameRuleDictionary[curStage];
        return endGameRuleEnemyCnt;
    }
    #endregion
}
