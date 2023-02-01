using Nakama.TinyJson;
using System.Collections.Generic;

public class MatchDataJson
{
    /// <summary>
    /// ������ ������ Update���� ������
    /// </summary>
    /// <param name="score"> point�� ���� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string Score(int score)
    {
        var values = new Dictionary<string, string>
        {
            {"Score", score.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// ��Ī�� �÷��� �ð��� �� �Ǿ ���� ���Ḧ �˸��� ������
    /// </summary>
    /// <param name="isDied"> true�� ���� �ð��� �� ���������� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string Died(string userSessionID)
    {
        var values = new Dictionary<string, string>
        {
            {"DieUser", userSessionID.ToString() },
        };

        return values.ToJson();
    }

    /// <summary>
    /// ���� ��Ī �� maxTasks��ŭ ��ٸ� �� ������ �����ϵ��� �Ѵ�.
    /// </summary>
    /// <param name="maxTasks"></param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ�</returns>
    public static string SetStartGame(int maxTasks)
    {
        var values = new Dictionary<string, string>
        {
            { "maxTasks", maxTasks.ToString() },
        };

        return values.ToJson();
    }
}
