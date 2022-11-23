using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packet.GameServer
{
    public enum MessageCode
    {
        Success = 200,
        BadRequest = 400,
        NotFound = 404,
        Conflict = 409,
        ServerError = 500,
    }

    public class ResData
    {
        public int messageCode;
        public string message;
    }

    #region User
    public class UserData : ResData
    {
        public string userId;
        public string userName;
        public int userGold;
    }

    public class ReqSetUserPacket
    {
        public string userId;
        public string userName;
        public int userGold;
    }

    public class ReqUserInfoPacket
    {
        public string userId;
    }
    #endregion
}