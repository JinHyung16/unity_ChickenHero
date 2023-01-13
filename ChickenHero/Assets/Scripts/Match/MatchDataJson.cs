using Nakama.TinyJson;
using System.Collections.Generic;

public class MatchDataJson
{
    /// <summary>
    /// 유저의 점수를 Update해줄 데이터
    /// </summary>
    /// <param name="score"> point를 통해 점수를 갱신한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다 </returns>
    public static string Score(int score)
    {
        var values = new Dictionary<string, string>
        {
            {"Point", score.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// 매칭시 플레이 시간이 다 되어서 게임 종료를 알리는 데이터
    /// </summary>
    /// <param name="isDied"> true면 게임 시간이 다 끝난것으로 게임을 종료한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다 </returns>
    public static string Died(bool isDied)
    {
        var values = new Dictionary<string, string>
        {
            {"Died", isDied.ToString() },
        };

        return values.ToJson();
    }

}
