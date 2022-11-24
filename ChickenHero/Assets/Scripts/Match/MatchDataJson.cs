using Nakama.TinyJson;
using System.Collections.Generic;

public class MatchDataJson
{
    /// <summary>
    /// 유저의 점수를 Update해줄 데이터
    /// </summary>
    /// <param name="point"> point를 통해 점수를 갱신한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다 </returns>
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
    /// 매칭시 두 플레이어를 스폰해줄 함수
    /// </summary>
    /// <param name="spawn"> true면 두 플레이어를 생성한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다</returns>
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
    /// 매칭시 플레이 시간이 다 되어서 게임 종료를 알리는 데이터
    /// </summary>
    /// <param name="timeDone"> true면 게임 시간이 다 끝난것으로 게임을 종료한다 </param>
    /// <returns> Dictionary의 값을 json형태로 리턴한다 </returns>
    public static string TimeDone(bool timeDone)
    {
        var values = new Dictionary<string, string>
        {
            {"TimeDone", timeDone.ToString() },
        };

        return values.ToJson();
    }
}
