using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    [HideInInspector] public RemotePlayerNetworkData networkData;

    private void Start()
    {
        GameServer.GetInstance.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;
    }

    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        mainThread.Enqueue(() => OnReceivedMatchState(matchState));
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        // If the incoming data is not related to this remote player, ignore it and return early.
        if (matchState.UserPresence.SessionId != networkData.User.SessionId)
        {
            return;
        }

        // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
        switch (matchState.OpCode)
        {
            case OpCodes.Point:
                UpdatePoint(matchState.State);
                break;
            default:
                break;
        }
    }

    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }

    private void UpdatePoint(byte[] state)
    {
    }

}
