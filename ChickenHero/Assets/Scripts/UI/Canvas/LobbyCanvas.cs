using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Packet.GameServer;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text LevelTxt;
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text GoldTxt;

    private void Start()
    {
        if (GameServer.GetInstance.IsLogin)
        {
            LoadUserInfo();
        }
    }

    private async void LoadUserInfo()
    {
        ReqUserInfoPacket reqData = new ReqUserInfoPacket
        {
            userId = GameServer.GetInstance.userid,
        };

        var resData = await GameServer.GetInstance.GetUserInfo(reqData);
        LevelTxt.text = resData.userLevel.ToString();
        NameTxt.text = resData.userName;
        GoldTxt.text = resData.userGold.ToString();
    }
}
