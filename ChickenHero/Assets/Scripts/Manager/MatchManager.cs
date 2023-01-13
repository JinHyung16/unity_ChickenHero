using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using HughGeneric;
using Nakama.TinyJson;
using Packet.GameServer;
using Cysharp.Threading.Tasks;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Linq;

sealed class MatchManager : Singleton<MatchManager>
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

    //Winning Display 관련 UI들
    [SerializeField] private GameObject displayWinPanel;
    [SerializeField] private TMP_Text winningPlayerText;
    private string localDisplayName;

    private void Start()
    {
        displayWinPanel.SetActive(false);
    }

    /// <summary>
    /// 매칭 시작시 Match관련 SetUp을 진행하고 매칭을 시작하도록 한다.
    /// </summary>
    public async UniTask InitMatchManager()
    {
        if (GameServer.GetInstance.GetIsServerConnect())
        {
            //about nakama match
            playerDictionary = new Dictionary<string, GameObject>();

            var mainThread = UnityMainThreadDispatcher.Instance();
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
        if (GameServer.GetInstance.GetIsServerConnect())
        {
            ReqSetUserPacket reqData = new ReqSetUserPacket
            {
                userId = GameServer.GetInstance.userid,
                userName = name,
                userGold = gold,
            };

            await GameServer.GetInstance.SetUserInfo(reqData);
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
    /// <param name="maxPlayer"> 최대 플레이어 인원을 설정한다. 기본은 2명 </param>
    private async UniTask JoinMatch(int minPlayer = 2, int maxPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.Socket.AddMatchmakerAsync("*", minPlayer, maxPlayer);
        ticket = matchMakingTicket.Ticket;
#if UNITY_EDITOR
        Debug.Log("<color=green><b> Find Match </b></color>");
#endif
    }

    public async UniTask QuickMatch()
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

        //그냥 본인 캐릭만 생성시켜서 게임하면 되니깐, 상대방의 캐릭터는 화면에 보여줄게 아니므로
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
        }
    }

    /// <summary>
    /// Sends a network message that indicates a player has won and a new round is being started.
    /// </summary>
    /// <returns></returns>
    public async void AnnounceWinner()
    {
        var winningPlayerName = localDisplayName;

        await AnnounceWinner(winningPlayerName);
    }

    /// <summary>
    /// 이긴 사람의 이름을 띄어준다.
    /// </summary>
    /// <param name="winningPlayerName">The name of the winning player.</param>
    private async UniTask AnnounceWinner(string winningPlayerName)
    {
        displayWinPanel.SetActive(true);
        // Set the winning player text label.
        winningPlayerText.text = string.Format("{0} 이겼다", winningPlayerName);

        // Wait for 2 seconds.
        await UniTask.Delay(2000);

        // Reset the winner player text label.
        winningPlayerText.text = "";
        displayWinPanel.SetActive(false);

        // Remove ourself from the players array and destroy our player.
        playerDictionary.Remove(localUser.SessionId);
        Destroy(localPlayer);
    }

    /// <summary>
    /// Sets the local user's display name.
    /// </summary>
    /// <param name="displayName">The new display name for the local user</param>
    public void SetDisplayName(string displayName)
    {
        localDisplayName = displayName;
    }

    #region Nakama Match State Function
    public async UniTask SendMatchStateAsync(long opCode, string state)
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

    /// <summary>
    /// 매치 전체 게임 상태(흐름)에 대한걸 관리한다
    /// ex)라운드 변경, 상대가 죽었는지 아닌지 등등
    /// </summary>
    /// <param name="matchState"> 매치 상태를 전달 </param>
    /// <returns></returns>
    private async UniTask OnReceivedMatchState(IMatchState matchState)
    {
        // local 유저의 session id 가져오기
        var userSessionId = matchState.UserPresence.SessionId;

        // match state의 길이가 있다면 dictionary에 decode해주기
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCodes에 따라 Match 상태 변경
        switch (matchState.OpCode)
        {
            case OpCodes.Spawn:
                SpawnPlayer(currentMatch.Id, matchState.UserPresence);
                break;
            case OpCodes.IsDie:
                var player = playerDictionary[userSessionId];
                Destroy(player, 0.5f);
                playerDictionary.Remove(userSessionId);
                if (playerDictionary.Count == 1 && playerDictionary.First().Key == localUser.SessionId)
                {
                    AnnounceWinner();
                    await QuickMatch();
                }
                break;
            default:
                break;
        }
    }
    #endregion
}
