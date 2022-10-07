using UnityEngine;
using System.Threading.Tasks;
using Packet.GameServer;
using Newtonsoft.Json;

public partial class GameServer
{
    /// <summary>
    /// Game Server RPC �����ϴ� ��
    /// </summary>

    #region User
    public async Task<UserData> SetUserRetainGoods(ReqSetUserPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await Client.RpcAsync(Session, "set_user_goods", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }

    public async Task<UserData> GetUserRetainGoods(ReqUserInfoPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await Client.RpcAsync(Session, "get_user_goods", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }

    public async Task<UserData> GetUserRetainGoodsTest()
    {
        var res = await Client.RpcAsync(Session, "get_user_goods_test");
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }
    #endregion
}