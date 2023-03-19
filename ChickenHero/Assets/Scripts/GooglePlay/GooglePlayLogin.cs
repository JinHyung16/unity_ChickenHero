using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using TMPro;
using UnityEngine.UI;

public class GooglePlayLogin : MonoBehaviour
{
    [SerializeField] private GameObject googleLoginPanel;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginStateTxt;

    private bool waitingForAuth = false;
    private void Start()
    {
        googleLoginPanel.SetActive(false);
        loginButton.onClick.AddListener(GooglePlayGamesActive);
    }

    private void GooglePlayGamesActive()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        AutoLogin();
    }

    //시작하자마자 자동 로그인 진행
    private void AutoLogin()
    {
        if (waitingForAuth)
        {
            return;
        }

        googleLoginPanel.SetActive(true);
        if (!Social.localUser.authenticated)
        {
            loginStateTxt.text = "Authenticating...";
            waitingForAuth = true;
            Social.localUser.Authenticate(AuthenticateCallback);
        }
    }

    //수동으로 로그인
    public void LoginToButton()
    {
        //이미 인증된 사용자는 바로 로그인 성공 
        if (Social.localUser.authenticated)
        {
            Debug.Log(Social.localUser.userName);
            loginStateTxt.text = "name : " + Social.localUser.userName + "\n";
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
        }

        googleLoginPanel.SetActive(false);
    }


    private void AuthenticateCallback(bool success)
    {
        loginStateTxt.text = "Loading";
        if (success)
        {
            loginStateTxt.text = "Welcome" + Social.localUser.userName + "\n";
        }
        else
        {
            loginStateTxt.text = "Login Fail\n";
        }
    }
}
