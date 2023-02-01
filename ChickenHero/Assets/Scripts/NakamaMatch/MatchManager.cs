using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using HughGeneric;
using Nakama.TinyJson;
using Cysharp.Threading.Tasks;
using System.Linq;

sealed class MatchManager : MonoBehaviour
{
    #region Singleton
    private static MatchManager instance;
    public static MatchManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion
    private string ticket;

    private IMatch currentMatch;
    private IUserPresence localUser;
    [SerializeField] public string localUserSessionID;

    private IDictionary<string, GameObject> playerDictionary;

    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private GameObject RemotePlayer;

    [SerializeField] private Transform LocalSpawnPoint;
    [SerializeField] private Transform RemoteSpawnPoint;

    //Match Making UI Canvas
    [SerializeField] private GameObject matchCanvas;

    private void Start()
    {
        matchCanvas.SetActive(true);

        playerDictionary = new Dictionary<string, GameObject>();
        InitMatchManager();
    }

    /// <summary>
    /// ��Ī ���۽� Match���� SetUp�� �����ϰ� ��Ī�� �����ϵ��� �Ѵ�.
    /// </summary>
    public void InitMatchManager()
    {
        UnityMainThreadDispatcher mainThread = UnityMainThreadDispatcher.Instance();
        GameServer.GetInstance.GetSocket().ReceivedMatchmakerMatched += T => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchPresence += T => mainThread.Enqueue(() => OnReceivedMatchPresence(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchState += T => mainThread.Enqueue(async () => await OnReceivedMatchState(T));

        Debug.Log("��ġ �����Ͽ� �Լ��� ���� �Ϸ�");
    }

    #region Match ã��, ���, ������ �Լ� & Button ���� �Լ�
    /// <summary>
    /// MatchCanvas�� FindMatch Button�� ������ �Լ�
    /// </summary>
    public async void FindMatch()
    {
        await FindMatchmaking();
        matchCanvas.SetActive(false);
    }

    /// <summary>
    /// MatchCanvas�� CancelMatch�� ������ �Լ�
    /// </summary>
    public async void CancelMatch()
    {
        await CancelMatchmaking();
        matchCanvas.SetActive(true);
    }


    private async UniTask FindMatchmaking(int minPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.GetSocket().AddMatchmakerAsync("*", minPlayer);
        ticket = matchMakingTicket.Ticket;
    }

    private async Task CancelMatchmaking()
    {
        await GameServer.GetInstance.GetSocket().RemoveMatchmakerAsync(ticket);
        matchCanvas.SetActive(true);
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

            string json = MatchDataJson.SetStartGame(5);
            await SendMatchStateAsync(OpCodes.StartGame, json);
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
            case OpCodes.StartGame:
                EnemySpawnManager.GetInstance.StartEnemySpawnerPooling();
                break;
        }
    }
}
