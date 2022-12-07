using HmsPlugin;
using UnityEngine;
using UnityEngine.UI;

public class GameKitUIView : MonoBehaviour
{

    [SerializeField] private Button Btn_ShowAchievements;
    [SerializeField] private Button Btn_ShowLeaderboards;
    [SerializeField] private Button Btn_UnlockAchievement;

    #region Monobehaviour

    private void Awake()
    {
    }

    private void OnEnable()
    {
        Btn_ShowAchievements.onClick.AddListener(ButtonClick_ShowAchievements);
        Btn_ShowLeaderboards.onClick.AddListener(ButtonClick_ShowLeaderboards);
        Btn_UnlockAchievement.onClick.AddListener(ButtonClick_UnlockAchievement);
    }

    private void OnDisable()
    {
        Btn_ShowAchievements.onClick.RemoveListener(ButtonClick_ShowAchievements);
        Btn_ShowLeaderboards.onClick.RemoveListener(ButtonClick_ShowLeaderboards);
        Btn_UnlockAchievement.onClick.RemoveListener(ButtonClick_UnlockAchievement);

    }

    #endregion

    #region Button Events

    private void ButtonClick_ShowAchievements()
    {
        GameKitManager.Instance.GetAchievementsList();
    }

    private void ButtonClick_ShowLeaderboards()
    {
        //GameKitManager.Instance.ShowLeaderboards();
    }

    private void ButtonClick_UnlockAchievement()
    {
        //GameKitManager.Instance.UnlockAchievement("tutorial");
        // var t = new HMSPushKitManager();
        // HMSPushKitManager.Instance.Init();

    }

    #endregion

}
