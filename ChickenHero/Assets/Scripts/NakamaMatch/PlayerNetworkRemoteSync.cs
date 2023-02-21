using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using UnityEngine.SocialPlatforms.Impl;
using System;
using Cysharp.Threading.Tasks;

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    public RemotePlayerNetworkData NetworkData;

    private int startTime;

    private void Start()
    {
        //startTime = 0;
        //GameServer.GetInstance.GetSocket().ReceivedMatchState += EnqueueOnReceivedMatchState;
    }


    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        //mainThread.Enqueue(() => OnReceivedMatchState(matchState));
        UnityMainThreadDispatcher.GetInstance.Enqueue(() => OnReceivedMatchState(matchState));
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        // If the incoming data is not related to this remote player, ignore it and return early.
        if (matchState.UserPresence.SessionId != NetworkData.User.SessionId)
        {
            return;
        }

        // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
        switch (matchState.OpCode)
        {
            case OpCodes.StartGame:
                EnemySpawnWhenTaskDone(matchState.State);
                break;
            default:
                break;
        }
    }

    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }

    public void EnemySpawnWhenTaskDone(byte[] state)
    {
        var myState = GetStateAsDictionary(state);
        startTime = int.Parse(myState["maxTasks"]);
        EnemySpawnCall().Forget();
    }

    private async UniTaskVoid EnemySpawnCall()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(startTime), cancellationToken: this.GetCancellationTokenOnDestroy());
        EnemySpawnManager.GetInstance.StartEnemySpawnerPooling();
    }
}

public class RemotePlayerNetworkData
{
    public string MatchId;
    public IUserPresence User;
}