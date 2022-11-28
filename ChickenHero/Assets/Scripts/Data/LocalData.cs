using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
public class LocalData : Singleton<LocalData>
{
    /// <summary>
    /// PlayerPrefs을 이용해 OffLine일 때 사용할 Local Data
    /// User의 레벨, 골드량만을 저장해두고 Server연동되면 동기화 할 목적도 포함
    /// </summary>

    #region PlayerPrefs Bool type
    /// <summary>
    /// PlayerPrefs에서 Bool 타입 지원하지 않아 만든 커스텀 함수
    /// PlayerPrefs에 저장할 때 쓰이는 함수
    /// </summary>
    /// <param name="key"> PlayerPrefs에 저장할 key값</param>
    /// <param name="state"> PlayerPrefs에 저장할 실제 값으로 false, true에 따라 0, 1 Int로 변환해 저장한다</param>
    public static void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    /// <summary>
    /// PlayerPrefs에서 Bool 타입은 지원하지 않아 만든 커스텀 함수
    /// PlayerPrefs에서 Bool 가져올 때 쓰이는 함수
    /// </summary>
    /// <param name="key"> key값을 받아 Bool Type으로 저장한다 </param>
    /// <returns> key값으로 찾은 value int가 0인지 1인지에 따라 bool type으로 return한다 </returns>

    public static bool GetBool(string key)
    {
        int value = PlayerPrefs.GetInt(key);
        if (value == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

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
    #endregion

    /// <summary>
    /// 로컬 PlayerPrefs초기화 시 사용할 함수
    /// </summary>
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
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
}
