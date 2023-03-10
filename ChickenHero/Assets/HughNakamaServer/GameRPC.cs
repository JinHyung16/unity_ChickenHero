using UnityEngine;
using System.Threading.Tasks;
using Packet.GameServer;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public partial class GameServer
{
    /// <summary>
    /// Game Server RPC 관리하는 곳
    /// </summary>

    #region User
    public async UniTask SetUserInfo(ReqSetUserPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await Client.RpcAsync(Session, "login_user", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
    }

    public async UniTask<UserData> GetUserInfo(ReqUserInfoPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await Client.RpcAsync(Session, "get_user", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }

    public async UniTask RemoveUserInfo(ReqUserInfoPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await Client.RpcAsync(Session, "remove_user", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log("Success Remove" + res.Payload);
#endif 
    }
    #endregion
}