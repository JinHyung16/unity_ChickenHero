using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Net.Http;
using System.Threading.Tasks;
using Packet.GameServer;
using HughGeneric;
using Cysharp.Threading.Tasks;

public partial class GameServer : LazySingleton<GameServer>
{
    protected string Scheme = "http";
    //protected string Host = "35.199.169.87"; // @GCP hugh-server VM �ܺ� ip
    protected string Host = "localhost"; //Local Host
    protected int Port = 7350;
    protected string ServerKey = "defaultkey";

    protected string sessionPrefName = "nakama.session";
    protected const string deviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    protected IClient Client;
    protected ISession Session;
    protected ISocket Socket; //socket�� ��� GameManager���� Match���� �ڵ� �ۼ����̶� �ʿ��ؼ� public���� �������

    public ISocket GetSocket() { return this.Socket; }

    //protected UnityMainThreadDispatcher mainThread;

    public string userid;
    public string userNickName;

    public async UniTask LoginToDevice()
    {
        PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> DeviceLogin : Host : {0}, Port : {1} </color>", Host, Port);
#endif
        Client = new Nakama.Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

        var authToken = PlayerPrefs.GetString(sessionPrefName);

        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Nakama.Session.Restore(authToken);
            if (!session.IsExpired)
            {
                Session = session;
            }
        }

        // If we weren't able to restore an existing session, authenticate to create a new user session.
        if (Session == null)
        {
            string deviceId;

            // If we've already stored a device identifier in PlayerPrefs then use that.
            if (PlayerPrefs.HasKey(deviceIdentifierPrefName))
            {
                deviceId = PlayerPrefs.GetString(deviceIdentifierPrefName);
            }
            else
            {
                // If we've reach this point, get the device's unique identifier or generate a unique one.
                deviceId = SystemInfo.deviceUniqueIdentifier;
                if (deviceId == SystemInfo.unsupportedIdentifier)
                {
                    deviceId = System.Guid.NewGuid().ToString();
                }

                // Store the device identifier to ensure we use the same one each time from now on.
                PlayerPrefs.SetString(deviceIdentifierPrefName, deviceId);
            }
#if UNITY_EDITOR
            Debug.LogFormat("<color=orange><b>[Game-Server]</b> deviceId : {0} </color>", deviceId);
#endif

            // Use Nakama Device authentication to create a new session using the device identifier.
            Session = await Client.AuthenticateDeviceAsync(deviceId, null, true);
            userid = Session.UserId;
            userNickName = Session.Username;

#if UNITY_EDITOR
            Debug.LogFormat("<color=green><b>[Game-Server]</b> userid : {0} </color>", userid);
            Debug.LogFormat("session.AuthToken : {0}", Session.AuthToken);
#endif

            // Store the auth token that comes back so that we can restore the session later if necessary.
            PlayerPrefs.SetString(sessionPrefName, Session.AuthToken);
        }

        Socket = Client.NewSocket();
        await Socket.ConnectAsync(Session, true, 10); // Socket connect timeout is 15

#if UNITY_EDITOR
        Debug.Log("<color=orange><b>[Game-Server]</b> Socekt Connect : {0} </color>");
#endif
    }

    public async UniTask Disconnect()
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
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Login-Server]</b> Disconnect : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
    }

    /// <summary>
    /// ������ ����Ǿ� �ִ� ���, DB ������ User�� ������ �����Ѵ�.
    /// </summary>
    /// <param name="name">������ �г��� ����</param>
    /// <param name="gold">������ ��ȭ�� ����</param>
    public async void SaveUserInfoServer(string name)
    {
        if (GetIsServerConnect())
        {
            ReqSetUserPacket reqData = new ReqSetUserPacket
            {
                userId = userid,
                userName = name,
            };

            await SetUserInfo(reqData);
        }
    }

    /// <summary>
    /// ������ ����Ǿ� �ִ� ���, DB ������ userId�� �˻��� �ش� ������ ������ �����.
    /// </summary>
    /// <param name="_userId"> userId�� �޴´� </param>
    public async void RemoveUserInfoServer(string _userId)
    {
        ReqUserInfoPacket reqData = new ReqUserInfoPacket
        {
            userId = _userId,
        };

        await RemoveUserInfo(reqData);
    }

    public bool GetIsServerConnect()
    {
        if (Session == null)
        {
            return false;
        }

        else
        {
            return true;
        }
    }
}