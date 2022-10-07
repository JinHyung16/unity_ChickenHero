using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Net.Http;
using System.Threading.Tasks;

public partial class GameServer : HughServer<GameServer>
{
    protected new string sessionPrefName = "gameserver.session";

    protected new string Host = "35.197.17.99"; // @GCP hugh-server VM ¿ÜºÎ ip
    protected new int Port = 7350;

    public async Task ConnectToGameServer(string userid)
    {
        this.Host = Host;
        this.Port = Port;

#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server] Connect To Server : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif

        if(Session == null)
        {
            Session = await Client.AuthenticateCustomAsync(userid, "", false);
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
}