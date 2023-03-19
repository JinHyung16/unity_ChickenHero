using Nakama.TinyJson;
using System.Collections.Generic;
using UnityEngine;

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
    public static string Died(UnityEngine.Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            {"DiePos_x", position.x.ToString() },
            {"DiePos_y", position.y.ToString() },
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
