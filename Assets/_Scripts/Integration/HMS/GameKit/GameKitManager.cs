using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Utils;
using System;
using HmsPlugin;
using UnityEngine.UI;

public class GameKitManager : Singleton<GameKitManager>
{
    private readonly string TAG = "[HMS] GameKitManager ";
    private bool customUnit = false;
    private const string SUCCESS_GAME = "DA527C62B67A8D8B34CAFC4107F7E712705690152363BEAE1DFE6D48AFDE3B04";
    private const string ALL_GEMS_COLLECT = "9E60B31BB82590B71E8682E4C6405C51D1BAFA30E7A57FD87AA331670791F2F4";
    private const string FAILED_GAME_COLLECT_ANY_GEMS = "18CD69D86915F09665DC5DCDEF343A86295E98652983CBB9EB3AA0D6B998E049";

    void Start()
    {
        //check HMSGameServiceManager  is initialized use time interval
        InvokeRepeating("CheckHMSGameServiceManager", 0.5f, 0.5f);
    }

    
    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log("HMS Games: GetAchievementsList SUCCESS ");
        foreach(var item in achievementList){
            Debug.Log("Achievement Id: " + item.Id);
            Debug.Log("Achievement Desc: " + item.DescInfo);
            Debug.Log("Achievement State: " + item.State);
            Debug.Log("Achievement GamePlayerName: " + item.GamePlayer?.DisplayName);
        }
    }

    private void OnGetAchievementListFailure(HMSException error)
    {
        Debug.Log("HMS Games: GetAchievementsList ERROR ");
    }

    private void OnShowAchievementsSuccess()
    {
        Debug.Log("HMS Games: ShowAchievements SUCCESS ");
    }

    private void OnShowAchievementsFailure(HMSException exception)
    {
        Debug.Log("HMS Games: ShowAchievements ERROR ");
    }

    private void CheckHMSGameServiceManager()
    {
        Debug.Log("Kardeşim burası game manager");
        if (HMSGameServiceManager.Instance != null)
        {
            CancelInvoke("CheckHMSGameServiceManager");
            Debug.Log(TAG + "HMSGameServiceManager is initialized");
            HMSAccountKitManager.Instance.SignIn();
            InvokeRepeating("CheckHMSAchievementManager", 0.5f, 0.5f);
        }
    }

    private void CheckHMSAchievementManager()
    {
        if (HMSAchievementsManager.Instance != null)
        {
            CancelInvoke("CheckHMSAchievementManager");
            Debug.Log(TAG + "HMSAchievementManager is initialized");
            HMSAchievementsManager.Instance.OnShowAchievementsSuccess = OnShowAchievementsSuccess;
            HMSAchievementsManager.Instance.OnShowAchievementsFailure = OnShowAchievementsFailure;
            HMSAchievementsManager.Instance.ShowAchievements();
            //GetAchievementsList();
        }
    }

    public void GetAchievementsList()
    {
        HMSAchievementsManager.Instance.OnGetAchievementsListSuccess = OnGetAchievemenListSuccess;
        HMSAchievementsManager.Instance.OnGetAchievementsListFailure = OnGetAchievementListFailure;
        HMSAchievementsManager.Instance.GetAchievementsList();
    }


}
