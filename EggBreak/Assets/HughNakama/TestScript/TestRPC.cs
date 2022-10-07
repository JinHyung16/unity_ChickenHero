using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packet.GameServer;

public class TestRPC : MonoBehaviour
{
    public KeyCode setGoodsKey = KeyCode.A;
    public KeyCode getGoodsKey = KeyCode.S;

    private void Update()
    {
        if(Input.GetKeyDown(setGoodsKey))
        {
            SetUserGoods();
        }
        if(Input.GetKeyDown(getGoodsKey))
        {
            GetUserGoods();
        }
    }

    private async void SetUserGoods()
    {
        ReqSetUserPacket reqSetUserPacket = new ReqSetUserPacket
        {
            userId = LoginServer.GetInstance.userid,
            userName = "Hugh",
            userLevel = 1,
            userGold = 1000
        };

        if(reqSetUserPacket != null)
        {
            Debug.Log(reqSetUserPacket.userId);
            Debug.Log(reqSetUserPacket.userName);
            Debug.Log(reqSetUserPacket.userLevel);
            Debug.Log(reqSetUserPacket.userGold);
        }

        var res = await GameServer.GetInstance.SetUserRetainGoods(reqSetUserPacket);

        Debug.Log(res.userId);
        Debug.Log(res.userName);
        Debug.Log(res.userLevel);
        Debug.Log(res.userGold);
    }

    public async void GetUserGoods()
    {
        var res = await GameServer.GetInstance.GetUserRetainGoodsTest();

        Debug.Log(res.userId); 
        Debug.Log(res.userName);
        Debug.Log(res.userLevel);
        Debug.Log(res.userGold);
    }
}
