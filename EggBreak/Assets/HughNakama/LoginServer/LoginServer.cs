using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;

public partial class LoginServer : HughServer<LoginServer>
{
    protected new string Host = "35.247.19.228"; // @GCP hugh-login-server VM ¿ÜºÎ ip
    protected new string sessionPrefName = "nakama.session";

    public string userid;
    public string userNickName;

    protected const string deviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";
    public async Task<ApiResponseException> DeviceLogin()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=orange><b>[Login-Server]</b> DeviceLogin : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
        ConnectToServer(this.Host, this.Port);

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
                Debug.LogFormat("<color=orange><b>[Login-Server]</b> deviceId : {0} </color>", deviceId);
#endif

                // Use Nakama Device authentication to create a new session using the device identifier.
                Session = await Client.AuthenticateDeviceAsync(deviceId, null, true);
                userid = Session.UserId;
                userNickName = Session.Username;

#if UNITY_EDITOR
                Debug.LogFormat("<color=orange><b>[Login-Server]</b> userid : {0} </color>", userid);
                Debug.LogFormat("session.AuthToken : {0}", Session.AuthToken);
                Debug.LogFormat("session.CreateTime : {0}", Session.CreateTime);
                Debug.LogFormat("session.ExpireTime : {0}", Session.ExpireTime);
                Debug.LogFormat("session.RefreshToken : {0}", Session.RefreshToken);
                Debug.LogFormat("session.RefreshExpireTime : {0}", Session.RefreshExpireTime);
#endif

                // Store the auth token that comes back so that we can restore the session later if necessary.
                PlayerPrefs.SetString(sessionPrefName, Session.AuthToken);
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
        Debug.LogFormat("<color=orange><b>[Login-Server]</b> Disconnect : Host : {0}, Port : {1} </color>", this.Host, this.Port);
#endif
        return base.Disconnect();
    }

    public async Task GetAccoount()
    {
        var response = await Client.GetAccountAsync(Session);
        userNickName = response.User.Username;
        Debug.LogFormat("username : {0}", response.User.Username);
        Debug.LogFormat("userid : {0}", response.User.Id);
    }

    public async Task<ApiResponseException> UpdateAccount(string username)
    {
        try
        {
            await Client.UpdateAccountAsync(Session, username);
            userNickName = username;
            Debug.LogFormat("username : {0}", Session.Username);
            Debug.LogFormat("userid : {0}", Session.UserId);
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
