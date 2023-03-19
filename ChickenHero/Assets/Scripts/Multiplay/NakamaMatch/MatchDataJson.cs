using Nakama.TinyJson;
using System.Collections.Generic;
using UnityEngine;

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
            {"Score", score.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// 매칭시 플레이 시간이 다 되어서 게임 종료를 알리는 데이터
    /// </summary>
    /// <param name="isDied"> true면 게임 시간이 다 끝난것으로 게임을 종료한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다 </returns>
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
    /// 서버 매칭 후 maxTasks만큼 기다린 후 게임을 시작하도록 한다.
    /// </summary>
    /// <param name="maxTasks"></param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다</returns>
    public static string SetStartGame(int maxTasks)
    {
        var values = new Dictionary<string, string>
        {
            { "maxTasks", maxTasks.ToString() },
        };

        return values.ToJson();
    }
}
