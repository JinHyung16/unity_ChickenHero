    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using UnityEngine.UI;
using HughGeneric;
using Nakama.TinyJson;
using Packet.GameServer;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MatchManager : Singleton<MatchManager>
{
    //about match
    private IMatch currentMatch;
    private IUserPresence localUser;
    private string ticket;
    
    private GameObject localPlayer;
    private IDictionary<string, GameObject> playerDictionary;

    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private GameObject RemotePlayer;

    [SerializeField] private GameObject SpawnPoint;


    /// <summary>
    /// ��Ī ���۽� Match���� SetUp�� �����ϰ� ��Ī�� �����ϵ��� �Ѵ�.
    /// </summary>
    public async Task MatchSetup()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            //about nakama match
            playerDictionary = new Dictionary<string, GameObject>();

            var mainThread = new UnityMainThreadDispatcher();
            GameServer.GetInstance.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(m));
            GameServer.GetInstance.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
            GameServer.GetInstance.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

            await JoinMatch();
        }
    }
    
    /// <summary>
    /// ������ ����Ǿ� �ִ� ���, DB ������ User�� ������ �����Ѵ�.
    /// </summary>
    /// <param name="level">������ ���� ����</param>
    /// <param name="name">������ �г��� ����</param>
    /// <param name="gold">������ ��ȭ�� ����</param>
    public async void SaveUserInfoServer(string name, int gold)
    {
        if (GameServer.GetInstance.IsLogin)
        {
            ReqSetUserPacket reqData = new ReqSetUserPacket
            {
                userId = GameServer.GetInstance.userid,
                userName = name,
                userGold = gold,
            };

            await GameServer.GetInstance.SetUserInfo(reqData);
        }
        else
        {
            return;
        }
    }

    public async void RemoveUserInfoServer(string _userId)
    {
        ReqUserInfoPacket reqData = new ReqUserInfoPacket
        {
            userId = _userId,
        };

        await GameServer.GetInstance.RemoveUserInfo(reqData);
    }

    /// <summary>
    /// Match�� �����Ѵ�
    /// </summary>
    /// <param name="minPlayer"> �ּ� �÷��̾� �ο��� �����Ѵ�. �⺻�� 2�� </param>
    private async Task JoinMatch(int minPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.Socket.AddMatchmakerAsync("*", minPlayer, 2);
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

    /// <summary>
    /// Player Spawn ���ִ� �Լ�
    /// LocalPlayer�� �������ְ�, RemotePlayer�� ������ų �ʿ� ����.
    /// �ֳĸ� ������ Update�ؼ� UI�� �����ٰű� �����̴�.
    /// </summary>
    /// <param name="matchId"> match�� id�� �޴´� </param>
    /// <param name="user"> ��ġ�� ���� user�� �޴´� </param>
    public void SpawnPlayer(string matchId, IUserPresence user)
    {
        if (playerDictionary.ContainsKey(user.SessionId))
        {
            return;
        }

        var isLocalPlayer = user.SessionId == localUser.SessionId;
        var playerPrefab = isLocalPlayer ? LocalPlayer : RemotePlayer;

        var player = Instantiate(LocalPlayer, SpawnPoint.transform.position, Quaternion.identity);

        if (!isLocalPlayer)
        {
            playerPrefab.GetComponent<PlayerNetworkRemoteSync>().networkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
            
        }

        playerDictionary.Add(user.SessionId, player);

        if (isLocalPlayer) 
        { 
            localPlayer = player;
            //local player�� die event �������ֱ�
            player.GetComponent<Player>().dieEvent.AddListener(OnLocalPlayerDied);
        }
    }

    /// <summary>
    /// Match�� ������ ���� IDictionary�� �ִ� player �����ִ� �Լ�
    /// </summary>
    /// <param name="player">local player�� �޴´�</param>
    private async void OnLocalPlayerDied(GameObject player)
    {
        // Send a network message telling everyone that we died.
        await SendMatchStateAsync(OpCodes.TimeDone, MatchDataJson.TimeDone(true));

        // Remove ourself from the players array and destroy our GameObject after 0.5 seconds.
        playerDictionary.Remove(localUser.SessionId);
        Destroy(player, 0.5f);
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
            SpawnPlayer(match.Id, user);
        }

        currentMatch = match;
    }
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // �� ���� ������ ���� ���� �����ϰ� ���̴� UI ����
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
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
                SpawnPlayer(currentMatch.Id, matchState.UserPresence);
                break;
            case OpCodes.TimeDone:
                var player = playerDictionary[userSessionId];
                Destroy(player, 0.5f);
                playerDictionary.Remove(userSessionId);
                break;
            default:
                break;
        }
    }
    #endregion
}
