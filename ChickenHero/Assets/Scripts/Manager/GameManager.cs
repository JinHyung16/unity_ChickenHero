using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using UnityEngine.UI;
using HughLibrary;
using Nakama.TinyJson;

public class GameManager : Singleton<GameManager>
{
    private IMatch currentMatch;
    private IUserPresence localUser;
    private string ticket;

    private GameObject localPlayer;
    private IDictionary<string, GameObject> playerDictionary;


    private void Start()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            //about nakama match
            playerDictionary = new Dictionary<string, GameObject>();

            var mainThread = new UnityMainThreadDispatcher();
            GameServer.GetInstance.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(m));
            GameServer.GetInstance.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
            GameServer.GetInstance.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        }
    }

    public async void MatchStart(int minPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.Socket.AddMatchmakerAsync("*", minPlayer, 8);
        ticket = matchMakingTicket.Ticket;
#if UNITY_EDITOR
        Debug.Log("<color=green><b> Find Match </b></color>");
#endif
    }

    public async Task QuickMatch()
    {
        await GameServer.GetInstance.Socket.LeaveMatchAsync(currentMatch);

        currentMatch = null;
        localUser = null;

        foreach (var player in playerDictionary.Values)
        {
            Destroy(player);
        }

        playerDictionary.Clear();

#if UNITY_EDITOR
        Debug.Log("<color=green><br> Quick Match </br></color>");
#endif
    }

    #region Nakama Match State Function
    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await GameServer.GetInstance.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        GameServer.GetInstance.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    private async void OnRecivedMatchMakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        // localuser ĳ��
        localUser = matchmakerMatched.Self.Presence;
        var match = await GameServer.GetInstance.Socket.JoinMatchAsync(matchmakerMatched);

#if UNITY_EDITOR
        Debug.Log("Our Session Id: " + match.Self.SessionId);
#endif

        foreach (var user in match.Presences)
        {
            Debug.Log("Connected User Session Id: " + user.SessionId);
            //SpawnPlayer(match.Id, user);
        }

        currentMatch = match;
    }
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // �� ���� ������ ���� ���� �����ϰ� ���̴� UI ����
        foreach (var user in matchPresenceEvent.Joins)
        {
            //SpawnPlayer(match.Id, user);
        }

        // �� ������ ���� �� �������ֱ�
        foreach (var user in matchPresenceEvent.Leaves)
        {
            Debug.Log("Leave User Session Id : " + user.SessionId);

            if (playerDictionary.ContainsKey(user.SessionId))
            {
                Destroy(playerDictionary[user.SessionId]);
                playerDictionary.Remove(user.SessionId);
            }
        }
    }

    // �Լ� ������ await ��� ���ؼ� �ߴ� ��� ��
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        // local ������ session id ��������
        var userSessionId = matchState.UserPresence.SessionId;

        // match state�� ���̰� �ִٸ� dictionary�� decode���ֱ�
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCodes�� ���� Match ���� ����
        switch (matchState.OpCode)
        {
            case OpCodes.SpawnPlayer:
                break;
            case OpCodes.TimeDone:
                break;
            default:
                break;
        }
    }
    #endregion
}
