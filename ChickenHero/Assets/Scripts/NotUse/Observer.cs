
/// <summary>
/// Observer�� Subject�� interface�� ��� �ֽ��ϴ�.
/// </summary>

namespace HughUtility
{
    public interface Observer
    {
        //���� ���� �� �ʱ�ȭ
        UIType ObserverNotifyCanvas(UIType type);
    }

    public interface Subject
    {
        //Observer ���
        void RegisterObserver(Observer observer);
        
        //Observer ����
        void RemoveObserver(Observer observer);

        //��� Observer ������Ʈ
        void NotifyObservers(UIType type, bool active);
    }

    /// <summary>
    /// Observer ������ ���� �����ϰ� ������ UI ����
    /// Name + UI type���� ����س���.
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