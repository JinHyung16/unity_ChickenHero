using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;
using HughGeneric;

public class ObserverManager : Singleton<ObserverManager>
{
    /// <summary>
    /// Lobby Scene에서 Toggle로 관리하고 있는 Panel 관련하여
    /// 버튼 클릭하면 나머지 Panle들이 자동으로 SetActive(false)되는 옵저버 패턴 적용하기
    /// </summary>
    /// 
    Hashtable NoticeData = new Hashtable();

    public delegate void DelFunction(Notice notice);

    /// <summary>
    /// Observer들을 등록해준다
    /// </summary>
    /// <param name="observer"> Observer 넣어주기 </param>
    /// <param name="nType"> 어떤 UI 정보를 갖고 있을건지 받는다 </param>
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
    /// Observer들을 해제한다
    /// </summary>
    /// <param name="observer"> 해제할 observer </param>
    /// <param name="nType"> Hashtable안에 등록한 값과 해당 Observer가 들고있는 type </param>
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
    /// 어떤 observer가 던진 notice type을 다른 observer들에게 보낸다.
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
    private static Notice instnace = new Notice(NoticeType.NoneUI);

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
/// UI관련하여 Notice할 Data 모음
/// Toggle 형태의 Panel만 모여져 있다.
/// Toggle 형태의 Panel은 다른 Panel을 누르면 자동으로 꺼지는 Panel을 말한다.
/// </summary>
public enum NoticeType
{
    NoneUI = 0,

    InventoryPanel,
    PlayModePanel,
    OptionPanel,
}
