using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.UI;
using System;

public class GooglePlayLogin : MonoBehaviour
{
    [SerializeField] private TMP_Text loginText;

    private bool isWaitingForAuth = false;
    private void Awake()
    {
        InitGooglePlayLogin();
    }

    //시작하자마자 자동 GPGS 인증 진행
    private void InitGooglePlayLogin()
    {
        loginText.text = "안녕하세요!!!";

        //구글 게임즈 플렛폼 활성화(초기화) 및 게임 정보 저장(EnableSavedGames)
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    //로그인 버튼에 연결할 수동 로그인
    public void GooglePlayGamesLogin()
    {
        //이미 인증된 사용자는 바로 로그인 성공된다. 
        if (Social.localUser.authenticated)
        {
            Debug.Log(Social.localUser.userName);
            loginText.text = "name : " + Social.localUser.userName + "\n";
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    loginText.text = "성공! \n " + "안녕하세요 " + Social.localUser.userName;
                }
                else
                {
                    loginText.text = "인증 실패! \r\n(로그인 없이 게임시작은 가능합니다)";
                }
            });
        }
    }
}
