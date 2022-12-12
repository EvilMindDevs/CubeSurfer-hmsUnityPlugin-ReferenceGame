using HmsPlugin;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;

public class GameKitUIView : MonoBehaviour
{

    [SerializeField] private Button Btn_ShowAchievements;
    [SerializeField] private Button Btn_UnlockAchievement;
    [SerializeField] private GameObject hmsGameArea;  


    #region Monobehaviour

    private void Awake()
    {
    }

    private void OnEnable()
    {
        Btn_ShowAchievements.onClick.AddListener(ButtonClick_ShowAchievements);
        Btn_UnlockAchievement.onClick.AddListener(ButtonClick_UnlockAchievement);
    }

    private void OnDisable()
    {
        Btn_ShowAchievements.onClick.RemoveListener(ButtonClick_ShowAchievements);
        Btn_UnlockAchievement.onClick.RemoveListener(ButtonClick_UnlockAchievement);

    }

    #endregion

    #region Button Events

    private void ButtonClick_ShowAchievements()
    {
        GameKitManager.Instance.GetAchievementsList();
        hmsGameArea.SetActive(true);
    }

    private void ButtonClick_UnlockAchievement()
    {
        //GameKitManager.Instance.UnlockAchievement("tutorial");
        // var t = new HMSPushKitManager();
        // HMSPushKitManager.Instance.Init();

    }

    public void CloseGameArea()
    {
        hmsGameArea.SetActive(false);
    }

    #endregion

}
