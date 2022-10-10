using System;
using System.Threading.Tasks;
using HughLibrary.Generics;
using Nakama;

public abstract class HughServer<T> : LazySingleton<T> where T : HughServer<T>
{
    protected string Scheme = "http";
    protected string Host = "localhost";
    protected int Port = 8350;

    protected string ServerKey = "defaultkey";

    protected string sessionPrefName;

    protected IClient Client;
    protected ISession Session;
    protected ISocket Socket;

    protected UnityMainThreadDispatcher mainThread;
    protected virtual void ConnectToServer(string host, int port)
    {
        Client = new Nakama.Client(Scheme, host, port, ServerKey);
    }

    protected async Task SocketConnect()
    {
        Socket = Client.NewSocket(false);        
        await Socket.ConnectAsync(Session, true);

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
        if (Socket != null)
        {            
            await Socket.CloseAsync();
            Socket = null;
        }

        if (Session != null)
        {         
            //await Client.SessionLogoutAsync(Session);
            Session = null;
        }
    }


    protected async Task<IApiRpc> GetRPC(string rpcId)
    {
        if (Session == null) return null;

        return await Client.RpcAsync(Session, rpcId, "");
    }

    protected async Task<IApiRpc> PutRPC(string rpcId, string param)
    {
        if (Session == null) return null;

        return await Client.RpcAsync(Session, rpcId, param);
    }
}
