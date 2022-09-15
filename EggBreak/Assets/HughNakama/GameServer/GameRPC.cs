using Nakama;
using System.Threading.Tasks;
using Packet.GameServer;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using UnityEngine;
using System.Collections.Generic;

public partial class GameServer
{
    // ������ ����� RPC ����� ���� �����ϴ� Ŭ����

    #region User
    public async Task<UserData> SetUserRetainGoods(ReqSetUserPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await client.RpcAsync(session, "set_user_goods", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }

    public async Task<UserData> GetUserRetainGoods(ReqUserInfoPacket req)
    {
        string json = JsonConvert.SerializeObject(req);
        var res = await client.RpcAsync(session, "get_user_goods", json);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }

    public async Task<UserData> GetUserRetainGoodsTest()
    {
        var res = await client.RpcAsync(session, "get_user_goods_test");
#if UNITY_EDITOR
        UnityEngine.Debug.Log(res.Payload);
#endif 
        return JsonConvert.DeserializeObject<UserData>(res.Payload);
    }
    #endregion
}