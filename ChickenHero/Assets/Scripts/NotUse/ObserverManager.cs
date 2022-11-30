using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;
using HughGeneric;

public class ObserverManager : Singleton<ObserverManager>
{
    /// <summary>
    /// Lobby Scene���� Toggle�� �����ϰ� �ִ� Panel �����Ͽ�
    /// ��ư Ŭ���ϸ� ������ Panle���� �ڵ����� SetActive(false)�Ǵ� ������ ���� �����ϱ�
    /// </summary>
    /// 
    Hashtable NoticeData = new Hashtable();

    public delegate void DelFunction(Notice notice);

    /// <summary>
    /// Observer���� ������ش�
    /// </summary>
    /// <param name="observer"> Observer �־��ֱ� </param>
    /// <param name="nType"> � UI ������ ���� �������� �޴´� </param>
    public void RegisterObserver(DelFunction observer, NoticeType nType)
    {
        if (NoticeData[nType] == null)
        {
            NoticeData[nType] = new List<DelFunction>();
        }

        List<DelFunction> noticeList = NoticeData[nType] as List<DelFunction>;
        if (noticeList.Contains(observer) == false)
        {
            noticeList.Add(observer);
        }
    }

    /// <summary>
    /// Observer���� �����Ѵ�
    /// </summary>
    /// <param name="observer"> ������ observer </param>
    /// <param name="nType"> Hashtable�ȿ� ����� ���� �ش� Observer�� ����ִ� type </param>
    public void RemoveObserver(DelFunction observer, NoticeType nType)
    {
        List<DelFunction> noticeList = (List<DelFunction>)NoticeData[nType];
        if (noticeList == null)
        {
            return;
        }

        if (noticeList.Contains(observer))
        {
            noticeList.Remove(observer);
        }
        if (noticeList.Count <= 0)
        {
            NoticeData.Remove(nType);
        }
    }

    /// <summary>
    /// � observer�� ���� notice type�� �ٸ� observer�鿡�� ������.
    /// </summary>
    /// <param name="nType"> notice type </param>
    public void PostNotice(NoticeType nType)
    {
        PostNotice(nType, null);
    }

    private void PostNotice(NoticeType type, Hashtable data)
    {
        Notice notice = Notice.Instantiate(type, data);

        List<DelFunction> notifyList = (List<DelFunction>)NoticeData[notice.noticeMSG];
        if (notifyList == null)
        {
#if UNITY_EDITOR
            Debug.Log("<color=green><br> Notify List not found: " + notice.noticeMSG + "</br></color>");
#endif
            return;
        }

        List<DelFunction> RemoveList = new List<DelFunction>();

        foreach (DelFunction observer in notifyList)
        {
            if (observer == null)
            {
                RemoveList.Add(observer);
            }
            else
            {
                observer(notice);
            }
        }

        foreach (DelFunction observer in RemoveList)
        {
            notifyList.Remove(observer);
        }
    }
}

public class Notice
{
    public NoticeType noticeMSG;
    public Hashtable noticeData;
    private static Notice instnace = new Notice(NoticeType.None);

    private Notice(NoticeType nType)
    {
        noticeData = new Hashtable();
        noticeMSG = nType;
    }

    public static Notice Instantiate(NoticeType nType, Hashtable data)
    {
        instnace.noticeMSG = nType;
        if (data != null)
        {
            instnace.noticeData.Clear();
            instnace.noticeData = data;
        }

        return instnace;
    }
}

/// <summary>
/// Game State������ ���� ��ȯ���� �� ȣ��
/// ���� ��� Scene�� �ִ��� �˷��ش�.
/// </summary>
public enum NoticeType
{
    None = 0,

    Login,
    Lobby,
    Play,
}
