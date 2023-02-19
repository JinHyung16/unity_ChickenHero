using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using HughGeneric;
using Nakama.TinyJson;
using Cysharp.Threading.Tasks;
using System.Linq;
using Cysharp.Threading.Tasks.CompilerServices;
using System;

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

        matchCanvasObj.SetActive(true);

        playerDictionary = new Dictionary<string, GameObject>();
        InitMatchManager();
    }
    #endregion
    private string ticket;

    UnityMainThreadDispatcher mainThread;

    private IMatch currentMatch;
    private IUserPresence localUser;
    [HideInInspector] public string localUserSessionID;

    private IDictionary<string, GameObject> playerDictionary;

    [SerializeField] private GameObject LocalPlayer;
    [SerializeField] private GameObject RemotePlayer;

    [SerializeField] private Transform LocalSpawnPoint;
    [SerializeField] private Transform RemoteSpawnPoint;

    //Match Making UI Canvas
    [SerializeField] private GameObject matchCanvasObj;
    private Canvas matchCanvas;

    private int startTime = 0;

    private void OnDestroy()
    {
        GameServer.GetInstance.GetSocket().ReceivedMatchmakerMatched -= T => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchPresence -= T => mainThread.Enqueue(() => OnReceivedMatchPresence(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchState -= T => mainThread.Enqueue(async () => await OnReceivedMatchState(T));

        playerDictionary.Clear();
    }
    private void Start()
    {
        matchCanvas = matchCanvasObj.GetComponent<Canvas>();
        matchCanvas.enabled = true;
    }
    /// <summary>
    /// 매칭 시작시 Match관련 SetUp을 진행하고 매칭을 시작하도록 한다.
    /// </summary>
    public void InitMatchManager()
    {
        mainThread = UnityMainThreadDispatcher.Instance();
        GameServer.GetInstance.GetSocket().ReceivedMatchmakerMatched += T => mainThread.Enqueue(() => OnRecivedMatchMakerMatched(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchPresence += T => mainThread.Enqueue(() => OnReceivedMatchPresence(T));
        GameServer.GetInstance.GetSocket().ReceivedMatchState += T => mainThread.Enqueue(async () => await OnReceivedMatchState(T));
    }

    #region Match 찾기, 취소, 나가기 함수 & Button 연결 함수
    /// <summary>
    /// MatchCanvas의 FindMatch Button의 연결할 함수
    /// </summary>
    public async void FindMatch()
    {
        await FindMatchmaking();
    }

    /// <summary>
    /// MatchCanvas의 CancelMatch의 연결할 함수
    /// </summary>
    public async void CancelMatch()
    {
        await CancelMatchmaking();
    }


    private async UniTask FindMatchmaking(int minPlayer = 2)
    {
        var matchMakingTicket = await GameServer.GetInstance.GetSocket().AddMatchmakerAsync("*", minPlayer);
        ticket = matchMakingTicket.Ticket;
    }

    private async Task CancelMatchmaking()
    {
        if (ticket != null)
        {
            await GameServer.GetInstance.GetSocket().RemoveMatchmakerAsync(ticket);
        }
        matchCanvas.enabled = true;
        SceneController.GetInstance.GoToScene("Lobby").Forget();
    }

    public async UniTask QuickMatch()
    {
        localUser = null;

        foreach (var player in playerDictionary.Values)
        {
            Destroy(player);
        }

        playerDictionary.Clear();

        await GameServer.GetInstance.GetSocket().LeaveMatchAsync(currentMatch.Id);
        currentMatch = null;
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

        //spawn Point도 local인지 아닌지에 따라 다르게 주기
        var spawnPoint = playerPrefab == isLocalPlayer ? LocalSpawnPoint : RemoteSpawnPoint;

        //local과 remote의 스폰 위치는 다르다. remote의 경우는 화면에 보이지 않는 곳에 생성시킨다
        var player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        if (!isLocalPlayer)
        {
            playerPrefab.GetComponent<PlayerNetworkRemoteSync>().NetworkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
        }

        playerDictionary.Add(user.SessionId, player);

        //match UI Active false로 설정
        matchCanvas.enabled = false;
    }

    /// <summary>
    /// MatchmakerMatched event가 nakama 서버로부터 받아지면 호출된다.
    /// </summary>
    /// <param name="matched">The MatchmakerMatched data</param>
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

        //match canvas에 있는 ui들을 끈다.
        matchCanvas.enabled = false;

        //적을 생성시키라고 서버에 알려준다
        string jsonData = MatchDataJson.SetStartGame(2);
        SendMatchState(OpCodes.StartGame, jsonData);
    }

    /// <summary>
    /// match에 참여하거나 나갔을때 호출된다.
    /// </summary>
    /// <param name="matchPresenceEvent">The MatchPresenceEvent data</param>
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // 유저 참여시 생성 해주기
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // 유저가 떠날 때 삭제해주기
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
    /// 서버의 데이터를 수신할때 부른다.
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
                MultiplayManager.GetInstance.UpdateRemoteScoreInMultiplay(int.Parse(state["Score"]));
                Debug.Log("MatchManager에서 Score Update중");
                break;
            case OpCodes.StartGame:
                startTime = int.Parse(state["maxTasks"]);
                EnemySpawnCall().Forget();
                break;
        }
    }

    private async UniTaskVoid EnemySpawnCall()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(startTime), cancellationToken: this.GetCancellationTokenOnDestroy());
        EnemySpawnManager.GetInstance.StartEnemySpawnerPooling();
    }

    public async UniTask SendMatchStateAsync(long opCode, string state)
    {
        await GameServer.GetInstance.GetSocket().SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        GameServer.GetInstance.GetSocket().SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

}
