
/// <summary>
/// Observer와 Subject의 interface를 담고 있습니다.
/// </summary>

namespace HughUtility
{
    public interface Observer
    {
        //정보 갱신 및 초기화
        void ObserverUpdate(bool IsCanvas);
    }

    public interface Subject
    {
        //Observer 등록
        void RegisterObserver(Observer _observer);
        //Observer 해제
        void RemoveObserver(Observer _observer);
        //모든 Observer 업데이트
        void NotifyObservers();
    }
}