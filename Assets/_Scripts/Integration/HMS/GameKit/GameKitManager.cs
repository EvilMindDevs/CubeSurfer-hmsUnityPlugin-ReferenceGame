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
    #region Constants and Variables
    private readonly string TAG = "[HMS] GameKitManager ";
    private bool customUnit;
    public const string SuccessGame = "B49147C59E3791BEBBE43C5A41A277303BE10AAEF8229213EC34F97D6E55B11B";
    public const string AllGemsCollect = "5F44ED67AFEFC23C7E7C6908415641CB3518BBEF3BC67E885531C26B90464B64";
    public const string FailedGameCollectAnyGems = "676F2DC9BEEBB9D0C79486CDD11CF9C619B2CF6BF5A32A4CF16FAA91A3CE6050";
    public const string WeeklyWinner = "8455FCF24132E39E24EC491C8261205B88BAA1298181F07E27DF83126B42015E";
    
    Dictionary<string, string> achievementDictionary = new() { 
        { SuccessGame, nameof(SuccessGame)}, 
        { AllGemsCollect, nameof(AllGemsCollect)}, 
        { FailedGameCollectAnyGems, nameof(FailedGameCollectAnyGems) }
    };

    public TableUI achievementTable;
    public TableUI leaderBoardTable;

    #endregion
    private void OnEnable()
    {
        AccountManager.AccountKitIsActive += OnAccountKitIsActive;
    }

    private void OnDisable()
    {
        AccountManager.AccountKitIsActive -= OnAccountKitIsActive;
    }

    private void OnAccountKitIsActive()
    {
        HMSAchievementsManager.Instance.OnShowAchievementsSuccess = OnShowAchievementsSuccess;
        HMSAchievementsManager.Instance.OnShowAchievementsFailure = OnShowAchievementsFailure;
        HMSAchievementsManager.Instance.OnUnlockAchievementSuccess = OnUnlockAchievementSuccess;
        HMSAchievementsManager.Instance.OnUnlockAchievementFailure = OnUnlockAchievementFailure;
        HMSGameServiceManager.Instance.Init();
        Debug.Log(TAG + "HMSAchievementManager is initialized");
    }

    #region Achievements
    private void OnGetAchievemenListSuccess(IList<Achievement> achievementList)
    {
        Debug.Log("HMS Games: GetAchievementsList SUCCESS ");
        //HMSAchievementsManager.Instance.UnlockAchievement(achievementDictionary.GetValueOrDefault(SuccessGame));
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

    public void GetAchievementsAndLeaderBoardList()
    {
        HMSAchievementsManager.Instance.OnGetAchievementsListSuccess = OnGetAchievemenListSuccess;
        HMSAchievementsManager.Instance.OnGetAchievementsListFailure = OnGetAchievementListFailure;
        HMSAchievementsManager.Instance.GetAchievementsList();
        GetLeaderboardData(WeeklyWinner);
    }

    private void OnChangeAchievementTextValue(int index, Achievement achievement)
    {
        var achievementName = achievementDictionary.GetValueOrDefault(achievement.Id);
        Debug.Log("GamePlayer Level: " + achievement.GamePlayer?.Level);
        if(!string.IsNullOrWhiteSpace(achievementName))
        {
            achievementTable.GetCell(index+1, 0).text = achievementName;
            achievementTable.GetCell(index+1, 1).text = achievement.State == 3 ? "Achieved" : "Not Achieved";
        }
                    
    }
    public void UnlockAchievement(string achievementId)
    {
        Debug.Log(TAG + " UnlockAchievement");

        HMSAchievementsManager.Instance.UnlockAchievement(achievementId);
    }
    private void OnSubmitScoreSuccess(ScoreSubmissionInfo scoreSubmission)
    {
        Debug.Log("HMS Games: SubmitScore SUCCESS ");
    }

    private void OnSubmitScoreFailure(HMSException exception)
    {
        Debug.Log("HMS Games: SubmitScore ERROR ");

    }
    public void SubmitScore(string leaderboardId, long score)
    {
        HMSLeaderboardManager.Instance.OnSubmitScoreSuccess = OnSubmitScoreSuccess;
        HMSLeaderboardManager.Instance.OnSubmitScoreFailure = OnSubmitScoreFailure;
        HMSLeaderboardManager.Instance.SubmitScore(leaderboardId, score);
        
    }
    #endregion

    #region Leaderboard

    public void OnGetLeaderboardDataSuccess(Ranking ranking)
    {
        
        Debug.Log("HMS Games: GetLeaderboardsData SUCCESS ");

        if(ranking == null)
        {
            Debug.Log("HMS Games: GetLeaderboardsData SUCCESS but ranking is null");
            return;
        }
        OnChangeLeaderBoardTextValue(0,ranking);

        foreach(var item in ranking?.RankingVariants){
            item.GetType().GetProperties().ToList().ForEach(x => Debug.Log(x.Name + " : " + x.GetValue(item, null)));
        }

    }
    public void OnGetLeaderboardDataFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetLeaderboardsData ERROR ");

    }
    public void OnGetLeaderboardsDataSuccess(IList<Ranking> rankingList)
    {
        
        Debug.Log("HMS Games: GetLeaderboardsData SUCCESS ");
        if(rankingList == null || rankingList?.Count == 0)
        {
            Debug.Log("HMS Games: GetLeaderboardsData SUCCESS but rankingList is null");
            return;
        }
        foreach(var item in rankingList){
            item.GetType().GetProperties().ToList().ForEach(x => Debug.Log(x.Name + " : " + x.GetValue(item, null)));
        }

    }
    public void OnGetLeaderboardsDataFailure(HMSException exception)
    {
        Debug.Log("HMS Games: GetLeaderboardsData ERROR ");

    }
    private void OnChangeLeaderBoardTextValue(int index, Ranking ranking)
    {
        var rankingName = ranking?.RankingDisplayName;
        Debug.Log("GamePlayer Level: " + rankingName);
        if(!string.IsNullOrWhiteSpace(rankingName))
        {
            leaderBoardTable.GetCell(index+1, 0).text = rankingName;
            leaderBoardTable.GetCell(index+1, 1).text = ranking.RankingVariants[0].PlayerDisplayScore;
        }
                    
    }
    public void GetLeaderboardsData(){
        HMSLeaderboardManager.Instance.GetLeaderboardsData();
    }
    public void GetLeaderboardData(string leaderboardId)
    {
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataSuccess = OnGetLeaderboardDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardDataFailure = OnGetLeaderboardDataFailure;
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataSuccess = OnGetLeaderboardsDataSuccess;
        HMSLeaderboardManager.Instance.OnGetLeaderboardsDataFailure = OnGetLeaderboardsDataFailure;
        HMSLeaderboardManager.Instance.GetLeaderboardData(leaderboardId);
    }

    #endregion

}
