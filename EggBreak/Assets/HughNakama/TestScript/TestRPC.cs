using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packet.GameServer;

public class TestRPC : MonoBehaviour
{
    public KeyCode setGoodsKey = KeyCode.A;

    private void Update()
    {
        if(Input.GetKeyDown(setGoodsKey))
        {
            SetUserGoods();
        }
    }

    private async void SetUserGoods()
    {
        ReqSetUserPacket reqSetUserPacket = new ReqSetUserPacket
        {
            userId = GameServer.GetInstance.userid,
            userName = "Hugh",
            userLevel = 1,
            userGold = 1000,
        };

        var res = await GameServer.GetInstance.SetUserGoods(reqSetUserPacket);

        Debug.Log(res.userId);
        Debug.Log(res.userName);
        Debug.Log(res.userLevel);
        Debug.Log(res.userGold);
    }
}
