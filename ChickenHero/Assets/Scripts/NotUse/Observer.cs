
/// <summary>
/// Observer와 Subject의 interface를 담고 있습니다.
/// </summary>

namespace HughUtility
{
    public interface Observer
    {
        //정보 갱신 및 초기화
        UIType ObserverNotifyCanvas(UIType type);
    }

    public interface Subject
    {
        //Observer 등록
        void RegisterObserver(Observer observer);
        
        //Observer 해제
        void RemoveObserver(Observer observer);

        //모든 Observer 업데이트
        void NotifyObservers(UIType type, bool active);
    }

    /// <summary>
    /// Observer 패턴을 통해 관찰하고 관리할 UI 모음
    /// Name + UI type으로 명시해놨다.
    /// </summary>
    public enum UIType
    {
        NoneUI,

        InventoryPanel,
        PlayModePanel,
        OptionPanel,

        InventoryToggle,
        PlayModeToggle,
        OptionToggle,
    }
}