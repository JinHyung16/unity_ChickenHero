using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughGeneric;
public class LocalData : Singleton<LocalData>
{
    /// <summary>
    /// PlayerPrefs�� �̿��� OffLine�� �� ����� Local Data
    /// User�� ����, ��差���� �����صΰ� Server�����Ǹ� ����ȭ �� ������ ����
    /// </summary>

    #region PlayerPrefs Bool type
    /// <summary>
    /// PlayerPrefs���� Bool Ÿ�� �������� �ʾ� ���� Ŀ���� �Լ�
    /// PlayerPrefs�� ������ �� ���̴� �Լ�
    /// </summary>
    /// <param name="key"> PlayerPrefs�� ������ key��</param>
    /// <param name="state"> PlayerPrefs�� ������ ���� ������ false, true�� ���� 0, 1 Int�� ��ȯ�� �����Ѵ�</param>
    public static void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    /// <summary>
    /// PlayerPrefs���� Bool Ÿ���� �������� �ʾ� ���� Ŀ���� �Լ�
    /// PlayerPrefs���� Bool ������ �� ���̴� �Լ�
    /// </summary>
    /// <param name="key"> key���� �޾� Bool Type���� �����Ѵ� </param>
    /// <returns> key������ ã�� value int�� 0���� 1������ ���� bool type���� return�Ѵ� </returns>

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
    /// ���� PlayerPrefs�ʱ�ȭ �� ����� �Լ�
    /// </summary>
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
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
}
