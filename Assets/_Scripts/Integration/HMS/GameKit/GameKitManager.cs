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
    private const string SUCCESS_GAME = "E6179393760057F2321023A85B88CFDFB96CEBD1BF529F729010C67B8EB28BE7";
    private const string ALL_GEMS_COLLECT = "484C3EF0037D6C7EA82D20C943E28758C937AC5B373ED008BDC24974B9264F8F";
    private const string FAILED_GAME_COLLECT_ANY_GEMS = "38FED07F53B8BCB56C51275D2C1E6229A9FA2AD9C4DF85BC15F597EDC4CA4AF3";

    void Start()
    {
        //check HMSGameServiceManager  is initialized use time interval
        InvokeRepeating("CheckHMSGameServiceManager", 0.5f, 0.5f);
    }

    
    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log("HMS Games: GetAchievementsList SUCCESS ");
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
