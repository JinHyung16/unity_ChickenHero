
/// <summary>
/// Observer�� Subject�� interface�� ��� �ֽ��ϴ�.
/// </summary>

namespace HughUtility
{
    public interface Observer
    {
        //���� ���� �� �ʱ�ȭ
        void ObserverUpdateScore(int _score);
    }

    public interface Subject
    {
        //Observer ���
        void RegisterObserver(Observer _observer);
        
        //Observer ����
        void RemoveObserver(Observer _observer);

        //��� Observer ������Ʈ
        void NotifyObservers();
    }
}