using Packet.GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Test : MonoBehaviour
{
    public KeyCode poolingKey;
    public KeyCode saveInfoKey;
    public KeyCode loadInfoKey;

    [SerializeField] private TMP_Text LevelTxt;
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text GoldTxt;

    //UI 관련
    private void Start()
    {
        poolingKey = KeyCode.A;
        saveInfoKey = KeyCode.S;
        loadInfoKey = KeyCode.L;
    }
    private void Update()
    {
        if (Input.GetKeyDown(poolingKey))
        {
            PoolTest();
        }

        if (Input.GetKeyDown(saveInfoKey))
        {
            SaveUserInfo();
        }

        if (Input.GetKeyDown(loadInfoKey))
        {
            LoadUserInfo();
        }
        
    }

    private void PoolTest()
    {
        GameObject testEgg = null;
        testEgg.transform.position = new Vector2(0, 5);
        testEgg.SetActive(true);
    }

    private async void SaveUserInfo()
    {
        ReqSetUserPacket req = new ReqSetUserPacket
        {
            userId = GameServer.GetInstance.userid,
            userName = "영웅 치킨",
            userGold = 5000,
        };

        await GameServer.GetInstance.SetUserInfo(req);
    }

    private async void LoadUserInfo()
    {
        ReqUserInfoPacket req = new ReqUserInfoPacket
        {
            userId = GameServer.GetInstance.userid,
        };

        var res = await GameServer.GetInstance.GetUserInfo(req);
        NameTxt.text = res.userName.ToString();
        GoldTxt.text = res.userGold.ToString();
    }
}
