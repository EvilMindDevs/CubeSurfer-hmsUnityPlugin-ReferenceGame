using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;
using System;

public class AccountManager : Singleton<AccountManager>
{
    private readonly string TAG = "[HMS] AccountManager ";
    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";
    [SerializeField] private TMPro.TMP_Text DisplayName;


    public static Action<string> AccountKitLog;

    void Start()
    {
        HMSAccountKitManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnLoginFailure;

        AccountKitLog?.Invoke(NOT_LOGGED_IN);

        if(HMSAccountKitManager.Instance.IsSignedIn) 
            HMSAccountKitManager.Instance.SilentSignIn();

    }

    public void LogIn()
    {
        Debug.Log(TAG+"LogIn");
        HMSAccountKitManager.Instance.SignIn();
    }

    public void SilentSignIn()
    {
        Debug.Log(TAG+"SilentSignIn");

        HMSAccountKitManager.Instance.SilentSignIn();
    }

    public void LogOut()
    {
        Debug.Log(TAG+"LogOut");

        HMSAccountKitManager.Instance.SignOut();
        AccountKitLog?.Invoke(NOT_LOGGED_IN);
        DisplayName.SetText("LOGIN", true);
        KitManager.Instance.CloseGameServices();
    }

    public void OnLoginSuccess(AuthAccount authHuaweiId)
    {
        AccountKitLog?.Invoke(string.Format(LOGGED_IN, authHuaweiId.DisplayName));
        DisplayName.SetText(authHuaweiId.DisplayName.ToUpper(),false);
        KitManager.Instance.OpenGameServices();
    }

    public void OnLoginFailure(HMSException error)
    {
        AccountKitLog?.Invoke(LOGIN_ERROR);
        DisplayName.SetText("LOGIN",true);
    }

}
