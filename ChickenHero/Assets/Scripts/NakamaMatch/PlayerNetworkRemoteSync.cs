using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    public RemotePlayerNetworkData networkData;

    public void SetRemoteScore(Dictionary<string, string> state)
    {
        //GameManager.GetInstance.UpdateRemoteScore(int.Parse(state["Score"]));
    }
}
