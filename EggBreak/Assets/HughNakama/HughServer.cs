using Nakama;
using System;
using System.Threading.Tasks;
using HughLibrary.Generics;

public abstract class HughServer<T> : LazySingleton<T> where T : HughServer<T>
{
    protected string scheme = "http";
    protected string host = "35.197.17.99"; // @GCP VM: hugh-server ¿ÜºÎ ip
    //protected string host = "localhost";

    protected int port = 7350;
    protected string serverKey = "defaultkey";

    protected string sessionPrefName;


    protected IClient client;
    protected ISession session;
    protected ISocket socket;

    protected UnityMainThreadDispatcher mainThread;
    protected virtual void ConnectToServer(string host, int port)
    {
        client = new Nakama.Client(scheme, host, port, serverKey);
    }

    protected async Task SocketConnect()
    {
        socket = client.NewSocket(false);        
        await socket.ConnectAsync(session, true);

        BindSocketEvents();
    }

    protected virtual void BindSocketEvents()
    {        
        if (mainThread == null) 
        {
            mainThread = UnityMainThreadDispatcher.Instance();            
        }
    }

    protected virtual void Socket_ReceivedError(Exception obj) { }

    protected virtual void Socket_ReceivedNotification(IApiNotification obj) { }

    protected virtual void Socket_Closed() { }

    protected virtual void Socket_Connected() { }

    public virtual async Task Disconnect()
    {
        if (socket != null)
        {            
            await socket.CloseAsync();
            socket = null;
        }

        if (session != null)
        {         
            //   await client.SessionLogoutAsync(session);
            session = null;
        }
    }

    /// <summary>
    /// if u not use custom rpc
    /// please using this.
    /// </summary>
    
    /*
    protected async Task<IApiRpc> GetRPC(string rpcId)
    {
        if (session == null) return null;

        return await client.RpcAsync(session, rpcId, "");
    }

    protected async Task<IApiRpc> PutRPC(string rpcId, string param)
    {
        if (session == null) return null;

        return await client.RpcAsync(session, rpcId, param);
    }
    */
}
