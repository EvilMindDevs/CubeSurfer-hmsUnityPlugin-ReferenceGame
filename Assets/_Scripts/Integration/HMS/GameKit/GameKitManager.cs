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
using UnityEngine.UI.TableUI;
using System.Linq;

public class GameKitManager : Singleton<GameKitManager>
{
    private readonly string TAG = "[HMS] GameKitManager ";
    private bool customUnit = false;
    private const string Success_Game = "DA527C62B67A8D8B34CAFC4107F7E712705690152363BEAE1DFE6D48AFDE3B04";
    private const string All_Gems_Collect = "9E60B31BB82590B71E8682E4C6405C51D1BAFA30E7A57FD87AA331670791F2F4";
    private const string Failed_Game_Collect_Any_Gems = "18CD69D86915F09665DC5DCDEF343A86295E98652983CBB9EB3AA0D6B998E049";
    private const string Weekly_Winner = "479696182309C1212897E5080D731C8C3B12E239EE4042DB09A7CA9BC4A9411A";
    
    Dictionary<string, string> achievementDictionary = new Dictionary<string, string>() { 
        { Success_Game, nameof(Success_Game).Replace("_", " ") }, 
        { All_Gems_Collect, nameof(All_Gems_Collect).Replace("_", " ") }, 
        { Failed_Game_Collect_Any_Gems, nameof(Failed_Game_Collect_Any_Gems).Replace("_", " ") }
    };

    public TableUI achievementTable;
    public TableUI leaderBoardTable;


    void Start()
    {
        //check HMSGameServiceManager  is initialized use time interval
        InvokeRepeating("CheckHMSGameServiceManager", 0.5f, 0.5f);
    }

    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log("HMS Games: GetAchievementsList SUCCESS ");
        //HMSAchievementsManager.Instance.UnlockAchievement(achievementDictionary.GetValueOrDefault(SUCCESS_GAME));
        foreach(var item in achievementList){
            Debug.Log("Achievement Id: " + item.Id);
            Debug.Log("Achievement Desc: " + item.DescInfo);
            Debug.Log("Achievement State: " + item.State);
            Debug.Log("Achievement GamePlayerName: " + item.GamePlayer?.DisplayName);
            OnChangeAchievementTextValue(achievementList.IndexOf(item), item);
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

    private void OnUnlockAchievementSuccess()
    {
        Debug.Log("HMS Games: UnlockAchievement SUCCESS ");
    }

    private void OnUnlockAchievementFailure(HMSException error)
    {
        Debug.Log("HMS Games: UnlockAchievement ERROR ");
    }

    private void CheckHMSGameServiceManager()
    {
        Debug.Log("Kardeşim burası game manager");
        if (HMSGameServiceManager.Instance != null)
        {
            CancelInvoke("CheckHMSGameServiceManager");
            Debug.Log(TAG + "HMSGameServiceManager is initialized");
            //HMSAccountKitManager.Instance.SignIn();
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
        GetLeaderboardData(Weekly_Winner);
    }

    private void OnChangeAchievementTextValue(int index, Achievement achievement)
    {
        var achievementName = achievementDictionary.GetValueOrDefault(achievement.Id);
        Debug.Log("GamePlayer Level: " + achievement.GamePlayer?.Level);
        if(!string.IsNullOrWhiteSpace(achievementName))
        {
            achievementTable.GetCell(index+1, 0).text = achievementName;
            achievementTable.GetCell(index+1, 1).text = achievement.State.ToString();
        }
                    
    }
   

    #region Leaderboard

    private void OnGetLeaderboardDataSuccess(Ranking ranking)
    {
        
        Debug.Log("HMS Games: GetLeaderboardsData SUCCESS ");
        Debug.Log("LeaderBoards "+ ranking?.RankingDisplayName);
        OnChangeLeaderBoardTextValue(0,ranking);

        foreach(var item in ranking?.RankingVariants){
            item.GetType().GetProperties().ToList().ForEach(x => Debug.Log(x.Name + " : " + x.GetValue(item, null)));
        }

    }
    private void OnGetLeaderboardDataFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetLeaderboardsData ERROR ");

    }


    private void OnGetLeaderboardsDataSuccess(IList<Ranking> rankingList)
    {
        
        Debug.Log("HMS Games: GetLeaderboardsData SUCCESS ");

        foreach(var item in rankingList){
            item.GetType().GetProperties().ToList().ForEach(x => Debug.Log(x.Name + " : " + x.GetValue(item, null)));
        }

    }
    private void OnGetLeaderboardsDataFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetLeaderboardsData ERROR ");

    }

    private void OnChangeLeaderBoardTextValue(int index, Ranking ranking)
    {
        var rankingName = ranking?.RankingDisplayName;
        if(!string.IsNullOrWhiteSpace(rankingName))
        {
            leaderBoardTable.GetCell(index+1, 0).text = rankingName;
            leaderBoardTable.GetCell(index+1, 1).text = ranking.RankingScoreOrder.ToString();
        }
                    
    }

    public void GetLeaderboardsData(){
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataSuccess = OnGetLeaderboardsDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataFailure = OnGetLeaderboardsDataFailure;
        HMSLeaderboardManager.Instance.GetLeaderboardsData();

    }

    public void GetLeaderboardData(string leaderboardId)
    {
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataSuccess = OnGetLeaderboardDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataFailure = OnGetLeaderboardDataFailure;
        HMSLeaderboardManager.Instance.GetLeaderboardData(leaderboardId);
    }

    #endregion

}
