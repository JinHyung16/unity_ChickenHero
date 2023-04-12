using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.UI;

public class GooglePlayLogin : MonoBehaviour
{
    [SerializeField] private GameObject googleLoginPanel;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginStateTxt;

    
    private string token;
    private string error;

    private void Awake()
    {
        GooglePlayLoginAuto();
    }

    private void Start()
    {
        googleLoginPanel.SetActive(false);
        loginButton.onClick.AddListener(GooglePlayGamesActive);
    }

    //시작하자마자 자동 로그인 진행
    private void GooglePlayLoginAuto()
    {
        //구글 게임즈 플렛폼 활성화(초기화) 및 게임 정보 저장(EnableSavedGames)
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    //수동으로 로그인
    public void GooglePlayGamesActive()
    {
        googleLoginPanel.SetActive(true);
        //이미 인증된 사용자는 바로 로그인 성공 
        if (Social.localUser.authenticated)
        {
            Debug.Log(Social.localUser.userName);
            loginStateTxt.text = "name : " + Social.localUser.userName + "\n";
            googleLoginPanel.SetActive(false);
            return;
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    loginStateTxt.text = "name : " + Social.localUser.userName + "\n";
                }
                else
                {
                    loginStateTxt.text = "Login Fail\n";
                }
            });
            googleLoginPanel.SetActive(false);
        }
    }
    
}
