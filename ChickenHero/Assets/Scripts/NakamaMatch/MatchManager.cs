using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using HughGeneric;
using Nakama.TinyJson;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Net.Sockets;

sealed class MatchManager : Singleton<MatchManager>
{
    private string ticket;

    private IMatch currentMatch;
    private IUserPresence localUser;
    [SerializeField] public string localUserSessionID;

    private IDictionary<string, GameObject> playerDictionary;

    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private GameObject RemotePlayer;

    [SerializeField] private Transform LocalSpawnPoint;
    [SerializeField] private Transform RemoteSpawnPoint;


    /// <summary>
    /// ��Ī ���۽� Match���� SetUp�� �����ϰ� ��Ī�� �����ϵ��� �Ѵ�.
    /// </summary>
    public void InitMatchManager()
    {
        if (GameServer.GetInstance.GetIsServerConnect())
        {
            //about nakama match
            playerDictionary = new Dictionary<string, GameObject>();

            UnityMainThreadDispatcher mainThread = UnityMainThreadDispatcher.Instance();
            GameServer.GetInstance.GetSocket().ReceivedMatchmakerMatched += T => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(T));
            GameServer.GetInstance.GetSocket().ReceivedMatchPresence += T => mainThread.Enqueue(() => OnReceivedMatchPresence(T));
            GameServer.GetInstance.GetSocket().ReceivedMatchState += T => mainThread.Enqueue(async () => await OnReceivedMatchState(T));
        }
    }

    #region Match ã��, ���, ������ �Լ�
    public async UniTask FindMatch(int minPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.GetSocket().AddMatchmakerAsync("*", minPlayer);
        ticket = matchMakingTicket.Ticket;
    }

    public async Task CancelMatchmaking()
    {
        await GameServer.GetInstance.GetSocket().RemoveMatchmakerAsync(ticket);
    }

    public async UniTask QuickMatch()
    {
        await GameServer.GetInstance.GetSocket().LeaveMatchAsync(currentMatch.Id);

        currentMatch = null;
        localUser = null;

        foreach (var player in playerDictionary.Values)
        {
            Destroy(player);
        }

        playerDictionary.Clear();
    }
    #endregion


    public void SpawnPlayer(string matchId, IUserPresence user)
    {
        if (playerDictionary.ContainsKey(user.SessionId))
        {
            return;
        }
        var isLocalPlayer = user.SessionId == localUser.SessionId;
        var playerPrefab = isLocalPlayer ? LocalPlayer : RemotePlayer;

        //spawn Point�� local���� �ƴ����� ���� �ٸ��� �ֱ�
        var spawnPoint = playerPrefab == isLocalPlayer ? LocalSpawnPoint : RemoteSpawnPoint;

        //local�� remote�� ���� ��ġ�� �ٸ���. remote�� ���� ȭ�鿡 ������ �ʴ� ���� ������Ų��
        var player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        if (!isLocalPlayer)
        {
            playerPrefab.GetComponent<PlayerNetworkRemoteSync>().networkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
        }

        playerDictionary.Add(user.SessionId, player);
    }

    public async UniTask SendMatchStateAsync(long opCode, string state)
    {
        await GameServer.GetInstance.GetSocket().SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        GameServer.GetInstance.GetSocket().SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    private async void OnRecivedMatchMakerMatched(IMatchmakerMatched matched)
    {
        localUser = matched.Self.Presence;
        localUserSessionID = localUser.SessionId;

        IMatch match = await GameServer.GetInstance.GetSocket().JoinMatchAsync(matched);

        foreach (IUserPresence user in match.Presences)
        {
            SpawnPlayer(match.Id, user);
        }

        currentMatch = match;
    }
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // ���� ������ ���� ���ֱ�
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // ������ ���� �� �������ֱ�
        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (playerDictionary.ContainsKey(user.SessionId))
            {
                Destroy(playerDictionary[user.SessionId]);
                playerDictionary.Remove(user.SessionId);
            }
        }
    }

    /// <summary>
    /// ������ �����͸� �����Ҷ� �θ���.
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
                    await QuickMatch();
                }
                break;
            case OpCodes.Score:
                GameManager.GetInstance.UpdateRemoteScore(int.Parse(state["Score"]));
                break;
        }
    }
}
