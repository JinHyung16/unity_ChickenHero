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
    /// 매칭 시작시 Match관련 SetUp을 진행하고 매칭을 시작하도록 한다.
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
    /// 서버의 연결되어 있는 경우, DB 서버에 User의 정보를 전달한다.
    /// </summary>
    /// <param name="level">유저의 레벨 전달</param>
    /// <param name="name">유저의 닉네임 전달</param>
    /// <param name="gold">유저의 재화량 전달</param>
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
    /// Match를 진행한다
    /// </summary>
    /// <param name="minPlayer"> 최소 플레이어 인원을 설정한다. 기본은 2명 </param>
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
    /// Player Spawn 해주는 함수
    /// LocalPlayer만 생성해주고, RemotePlayer는 생성시킬 필요 없다.
    /// 왜냐면 점수만 Update해서 UI로 보여줄거기 때문이다.
    /// </summary>
    /// <param name="matchId"> match의 id를 받는다 </param>
    /// <param name="user"> 매치에 들어온 user를 받는다 </param>
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
            //local player의 die event 연결해주기
            player.GetComponent<Player>().dieEvent.AddListener(OnLocalPlayerDied);
        }
    }

    /// <summary>
    /// Match가 끝나고 나면 IDictionary에 있는 player 지워주는 함수
    /// </summary>
    /// <param name="player">local player를 받는다</param>
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
        // localuser 캐싱
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
        // 각 유저 참여시 상대방 정보 간단하게 보이는 UI 띄우기
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // 각 유저가 떠날 때 삭제해주기
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

    // 함수 내에서 await 사용 안해서 뜨는 노란 줄
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        // local 유저의 session id 가져오기
        var userSessionId = matchState.UserPresence.SessionId;

        // match state의 길이가 있다면 dictionary에 decode해주기
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCodes에 따라 Match 상태 변경
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
