using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AccountUIView : MonoBehaviour
{

    [SerializeField] private Button Btn_Login;
    [SerializeField] private Button Btn_SilentLogin;
    [SerializeField] private Button Btn_Logout;
    [SerializeField] private Text AccountKitState;

    #region Monobehaviour

    private void OnEnable()
    {
        Btn_Login.onClick.AddListener(ButtonClick_Login);
        Btn_SilentLogin.onClick.AddListener(ButtonClick_SilentLogin);
        Btn_Logout.onClick.AddListener(ButtonClick_Logout);

        AccountManager.AccountKitLog += OnAccountKitLog;
    }

    private void OnDisable()
    {
        Btn_Login.onClick.RemoveListener(ButtonClick_Login);
        Btn_SilentLogin.onClick.RemoveListener(ButtonClick_SilentLogin);
        Btn_Logout.onClick.RemoveListener(ButtonClick_Logout);

        AccountManager.AccountKitLog -= OnAccountKitLog;
    }

    #endregion

    #region Callbacks

    private void OnAccountKitLog(string log)
    {
        AccountKitState.text = log;
    }

    #endregion

    #region Button Events

    private void ButtonClick_Login()
    {
        AccountManager.Instance.LogIn();
    }

    private void ButtonClick_SilentLogin()
    {
        AccountManager.Instance.SilentSignIn();
    }

    private void ButtonClick_Logout()
    {
        AccountManager.Instance.LogOut();
    }

    #endregion

}
