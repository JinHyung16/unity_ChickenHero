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
            {"Point", score.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// ��Ī�� �÷��� �ð��� �� �Ǿ ���� ���Ḧ �˸��� ������
    /// </summary>
    /// <param name="isDied"> true�� ���� �ð��� �� ���������� ������ �����Ѵ� </param>
    /// <returns> Dictionary�� ���� json���·� �����Ѵ� </returns>
    public static string Died(bool isDied)
    {
        var values = new Dictionary<string, string>
        {
            {"Died", isDied.ToString() },
        };

        return values.ToJson();
    }

}
