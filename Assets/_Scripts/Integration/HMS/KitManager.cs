using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitManager : MonoBehaviour
{
    public GameObject accountButtons;
    public GameObject GameServiceButtons;    
    public GameObject IAPButtons;

    #region Singleton

    public static KitManager Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    private void Awake()
    {
        Singleton();
    }
    public void Start()
    {
        GameManager.Instance.OnGameStarted += StartGameAnalytics;
        StartCoroutine("FirstLoad");
    }
    private void FirstLoad()
    {
        accountButtons?.SetActive(true);
        GameServiceButtons?.SetActive(false);
        IAPButtons?.SetActive(false);
    }
    public void ShowAccountButtons()
    {
        accountButtons?.SetActive(true);
    }
    public void HideAccountButtons()
    {
        accountButtons?.SetActive(false);
    }
    public void ShowGameServicesButtons()
    {
        GameServiceButtons?.SetActive(true);
    }
    public void HideGameServicesButtons()
    {
        GameServiceButtons?.SetActive(false);
    }
    public void ShowIAPButtons()
    {
       IAPButtons?.SetActive(true);
    }
    public void HideIAPButtons()
    {
       IAPButtons?.SetActive(false);
    }
    public void StartGameAnalytics()
    {
        var analyticsManager = AnalyticsManager.Instance;
        if (analyticsManager == null)
        {
            return;
        }

        var bundle = new Dictionary<string, object>
        {
            ["_Result"] = "StartGame"
        };
        analyticsManager.SendEvent("$StartGame", bundle);
    }
    public void EndGameAnalytics(string result)
    {
        var analyticsManager = AnalyticsManager.Instance;
        if (analyticsManager == null)
        {
            return;
        }
        
        string duration = GameManager.Instance?.elapsedTime.ToString();
        var bundle = new Dictionary<string, object>
        {
            ["$Duration"] = duration,
            ["$Result"] = result
        };
        analyticsManager.SendEvent("$EndGame", bundle);
    }
    public void OpenServices ()
    {
        StartCoroutine("ShowAccountButtons");
        StartCoroutine("ShowGameServicesButtons");
        StartCoroutine("ShowIAPButtons");
    }
    public void CloseServices ()
    {
        HideGameServicesButtons();
        HideIAPButtons();
        
    }

}
