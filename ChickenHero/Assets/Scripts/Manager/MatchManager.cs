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

    //Winning Display ���� UI��
    [SerializeField] private GameObject displayWinPanel;
    [SerializeField] private TMP_Text winningPlayerText;
    private string localDisplayName;

    private void Start()
    {
        displayWinPanel.SetActive(false);
    }

    /// <summary>
    /// ��Ī ���۽� Match���� SetUp�� �����ϰ� ��Ī�� �����ϵ��� �Ѵ�.
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
    /// ������ ����Ǿ� �ִ� ���, DB ������ User�� ������ �����Ѵ�.
    /// </summary>
    /// <param name="level">������ ���� ����</param>
    /// <param name="name">������ �г��� ����</param>
    /// <param name="gold">������ ��ȭ�� ����</param>
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
    /// Match�� �����Ѵ�
    /// </summary>
    /// <param name="minPlayer"> �ּ� �÷��̾� �ο��� �����Ѵ�. �⺻�� 2�� </param>
    /// <param name="maxPlayer"> �ִ� �÷��̾� �ο��� �����Ѵ�. �⺻�� 2�� </param>
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

        //�׳� ���� ĳ���� �������Ѽ� �����ϸ� �Ǵϱ�, ������ ĳ���ʹ� ȭ�鿡 �����ٰ� �ƴϹǷ�
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
    /// �̱� ����� �̸��� ����ش�.
    /// </summary>
    /// <param name="winningPlayerName">The name of the winning player.</param>
    private async UniTask AnnounceWinner(string winningPlayerName)
    {
        displayWinPanel.SetActive(true);
        // Set the winning player text label.
        winningPlayerText.text = string.Format("{0} �̰��", winningPlayerName);

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

    /// <summary>
    /// ��ġ ��ü ���� ����(�帧)�� ���Ѱ� �����Ѵ�
    /// ex)���� ����, ��밡 �׾����� �ƴ��� ���
    /// </summary>
    /// <param name="matchState"> ��ġ ���¸� ���� </param>
    /// <returns></returns>
    private async UniTask OnReceivedMatchState(IMatchState matchState)
    {
        // local ������ session id ��������
        var userSessionId = matchState.UserPresence.SessionId;

        // match state�� ���̰� �ִٸ� dictionary�� decode���ֱ�
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // OpCodes�� ���� Match ���� ����
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
