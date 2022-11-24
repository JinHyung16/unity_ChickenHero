using Nakama.TinyJson;
using System.Collections.Generic;

public class MatchDataJson
{
    /// <summary>
    /// ������ ������ Update���� ������
    /// </summary>
    /// <param name="point"> point�� ���� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string Point(int point)
    {
        var values = new Dictionary<string, string>
        {
            {"Point", point.ToString() }
        };

        return values.ToJson();
    }

    /*
    /// <summary>
    /// ��Ī�� �� �÷��̾ �������� �Լ�
    /// </summary>
    /// <param name="spawn"> true�� �� �÷��̾ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ�</returns>
    public static string SpawnPlayer(bool spawn)
    {
        var values = new Dictionary<string, string>
        {
            { "Spawn", spawn.ToString() },
        };

        return values.ToJson();
    }
    */

    /// <summary>
    /// ��Ī�� �÷��� �ð��� �� �Ǿ ���� ���Ḧ �˸��� ������
    /// </summary>
    /// <param name="timeDone"> true�� ���� �ð��� �� ���������� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string TimeDone(bool timeDone)
    {
        var values = new Dictionary<string, string>
        {
            {"TimeDone", timeDone.ToString() },
        };

        return values.ToJson();
    }
}
