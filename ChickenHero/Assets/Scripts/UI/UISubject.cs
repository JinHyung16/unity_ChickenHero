using UnityEngine;
using UnityEngine.EventSystems;
using HughUtility;

public class UISubject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, Subject
{

    /// <summary>
    /// � UI�� ���õǾ�����, ��ġ ���� �����ϰ� �ִ� ��ũ��Ʈ
    /// ��� Canvas Script�� UI Observer�� �ڽ��� �Ǿ�� �Ѵ�.
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
