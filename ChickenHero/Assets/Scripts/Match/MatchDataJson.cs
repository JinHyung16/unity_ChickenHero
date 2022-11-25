using Nakama.TinyJson;
using System.Collections.Generic;

public class MatchDataJson
{
    /// <summary>
    /// ������ ������ Update���� ������
    /// </summary>
    /// <param name="_score"> point�� ���� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string Score(int _score)
    {
        var values = new Dictionary<string, string>
        {
            {"Point", _score.ToString() }
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
    /// <param name="_timeDone"> true�� ���� �ð��� �� ���������� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string TimeDone(bool _timeDone)
    {
        var values = new Dictionary<string, string>
        {
            {"TimeDone", _timeDone.ToString() },
        };

        return values.ToJson();
    }
}
