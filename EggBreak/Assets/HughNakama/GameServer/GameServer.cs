using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Net.Http;
using System.Threading.Tasks;

public partial class GameServer : HughServer<GameServer>
{
    protected new string sessionPrefName = "gameserver.session";

    public async Task ConnectToGameServer(string userid, string host, int port = 7350)
    {
        //접속할 게임채널의 host, port
        this.Host = host;
        this.Port = port;
#if UNITY_EDITOR
        Debug.LogFormat("<color=yellow><b>[Game-Server]</b> ConnectToGameServer : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
        ConnectToServer(this.Host, this.Port);

        if (Session == null)
        {
            Session = await Client.AuthenticateCustomAsync(userid, null, false, null);

            PlayerPrefs.SetString(sessionPrefName, Session.AuthToken);
        }

        await SocketConnect();

    }

    //public async Task DisconnectToGameServer()
    public override Task Disconnect()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=yellow><b>[Game-Server]</b> Disconnect : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
        return base.Disconnect();
    }


}
