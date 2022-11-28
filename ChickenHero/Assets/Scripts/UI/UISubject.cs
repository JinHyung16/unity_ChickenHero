using UnityEngine;
using UnityEngine.EventSystems;
using HughUtility;

public class UISubject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, Subject
{

    /// <summary>
    /// 어떤 UI가 선택되었는지, 터치 관련 관리하고 있는 스크립트
    /// 모든 Canvas Script가 UI Observer의 자식이 되어야 한다.
    /// </summary>
    /// 
    public virtual void NotifyObservers(UIType type, bool active)
    {
    }

    public virtual void RegisterObserver(Observer observer)
    {
    }

    public virtual void RemoveObserver(Observer observer)
    {
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}
