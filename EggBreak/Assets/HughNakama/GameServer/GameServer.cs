using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Net.Http;
using System.Threading.Tasks;

public partial class GameServer : HughServer<GameServer>
{
    protected new string sessionPrefName = "gameserver.session";

    //protected new string Host = "35.197.17.99"; // @GCP hugh-server VM ¿ÜºÎ ip
    //protected new int Port = 7450;

    public async Task ConnectToGameServer(string userid, string host, int port = 7350)
    {
        this.Host = host;
        this.Port = port;

#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> Connect To Server : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif

        if(Session == null)
        {
            Session = await Client.AuthenticateCustomAsync(userid, null, false, null);
            PlayerPrefs.SetString(sessionPrefName, Session.AuthToken);
        }

        await SocketConnect();
    }

    public override Task Disconnect()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> Disconnect : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
        return base.Disconnect();
    }

    protected override void BindSocketEvents()
    {
        base.BindSocketEvents();

        Socket.Connected += Socket_Connected;
        Socket.Closed += Socket_Closed;
        Socket.ReceivedError += Socket_ReceivedError;
    }
}