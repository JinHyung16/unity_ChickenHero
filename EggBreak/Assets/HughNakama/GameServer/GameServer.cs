using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using System.Threading.Tasks;

public partial class GameServer : HughServer<GameServer>
{
    protected new string host = "35.197.17.99"; // boss scene 이동시 HughServer에 있는 host로 연결되므로 당장 nakama 접속시엔 필요 없다.
    //protected new string host = "localhost"; // @LocalHost
    //protected new int port = 7350; // HughServer에 있는 port와 동일

    protected new string sessionPrefName = "gameserver.session";
    public string userid;
    public string userNickName;

    protected const string deviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";
    public async Task<ApiResponseException> DeviceLogin()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> ConnectToLoginServer(DeviceLogin) : Host : {0}, Port : {1} </color>", this.host, this.port);
#endif
        ConnectToServer(this.host, this.port);

        // If we weren't able to restore an existing session, authenticate to create a new user session.
        if (session == null)
        {
            try
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
                session = await client.AuthenticateDeviceAsync(deviceId, null, true);
                userid = session.UserId;
                userNickName = session.Username;

#if UNITY_EDITOR
                Debug.LogFormat("session.AuthToken : {0}", session.AuthToken);
                Debug.LogFormat("session.CreateTime : {0}", session.CreateTime);
                Debug.LogFormat("session.ExpireTime : {0}", session.ExpireTime);
                Debug.LogFormat("session.RefreshToken : {0}", session.RefreshToken);
                Debug.LogFormat("session.RefreshExpireTime : {0}", session.RefreshExpireTime);
#endif

                // Store the auth token that comes back so that we can restore the session later if necessary.
                PlayerPrefs.SetString(sessionPrefName, session.AuthToken);
            }
            catch (ApiResponseException e)
            {
                Debug.Log(e);
                return e;
            }
        }

        // Open a new Socket for realtime communication.
        await SocketConnect();
        return null;
    }

    public override Task Disconnect()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Game-Server]</b> Disconnect : Host : {0}, Port : {1} </color>", this.host, this.port);
#endif
        // Delete the auth token
        // If u need login cache, please remove this code
        PlayerPrefs.DeleteKey(sessionPrefName);
        return base.Disconnect();
    }

    public async Task GetAccoount()
    {
        var response = await client.GetAccountAsync(session);
        userNickName = response.User.Username;
        Debug.LogFormat("username : {0}", response.User.Username);
        Debug.LogFormat("userid : {0}", response.User.Id);
    }

    public async Task<ApiResponseException> UpdateAccount(string username)
    {
        try
        {
            await client.UpdateAccountAsync(session, username);
            userNickName = username;
            Debug.LogFormat("username : {0}", session.Username);
            Debug.LogFormat("userid : {0}", session.UserId);
            return null;
        }
        catch (ApiResponseException e)
        {
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return e;
        }
    }



}
