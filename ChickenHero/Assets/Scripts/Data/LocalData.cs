using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
using HughUtility;

public class LocalData : Singleton<LocalData>
{
    /// <summary>
    /// PlayerPrefs�� �̿��� OffLine�� �� ����� Local Data
    /// User�� ����, ��差���� �����صΰ� Server�����Ǹ� ����ȭ �� ������ ����
    /// </summary>

    private void Start()
    {
        ReadCSVData();
        PlayerPrefs.DeleteAll();
    }

    #region PlayerPrefs Save Data Property
    private string userName = string.Empty;
    public string Name
    {
        get
        {
            return PlayerPrefs.GetString("Name");
        }
        set
        {
            userName = value;
            PlayerPrefs.SetString("Name", value);
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
    #endregion

    #region PlayerPrefs Control Function
    /// <summary>
    /// ���� PlayerPrefs�ʱ�ȭ �� ����� �Լ�
    /// �α��� �Ǿ��ִٸ� ������ �����͵� �����ش�.
    /// </summary>
    public void ClearAllUserInfo()
    {
        PlayerPrefs.DeleteAll();

        if (GameServer.GetInstance.GetIsServerConnect())
        {
            MatchManager.GetInstance.RemoveUserInfoServer(GameServer.GetInstance.userid);
        }

#if UNITY_EDITOR
        Debug.Log("<color=black><br> Delete All User Info PlayerPrefs </br></color>");
#endif
    }

    /// <summary>
    /// PlayerPrefs�� �ش� ���� ������ ����Ǿ� �ִ��� Ȯ���Ѵ�.
    /// ��� key���� User�� �̸��� ���ԵǾ� �־ �̸� �ϳ������� Ȯ��
    /// </summary>
    /// <param name="name"> ������ Nick Name�� �޴´� </param>
    /// <returns> PlayerPrefs�� �ش� Nick Name�� ������ true�� ������ false�� ����</returns>
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

    #region CSV Data Contorller Functions

    public int PickCost { get; private set; }

    private Dictionary<string, int> UpgradeCostDictionary = new Dictionary<string, int>();
    
    private void ReadCSVData()
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
            AddDictinary(level, cost);
        }
    }

    private void AddDictinary(string level, int cost)
    {
        UpgradeCostDictionary.Add(level, cost);
    }

    public int GetUpgradeCost(string level)
    {
        if (UpgradeCostDictionary.TryGetValue(level, out int value))
        {
            return value;
        }
        else
        {
            return 0;
        }
    }
    #endregion
}
