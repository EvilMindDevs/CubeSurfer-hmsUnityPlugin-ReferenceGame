using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitManager : MonoBehaviour
{
    public GameObject hmsAccount;
    public GameObject hmsGameServices;    

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
        hmsAccount.SetActive(true);
        hmsGameServices.SetActive(false);
    }

    public void OpenAccountKit()
    {
        hmsAccount.SetActive(true);
    }
    public void CloseAccountKit()
    {
        hmsAccount.SetActive(false);
    }
    public void OpenGameServices()
    {
        hmsGameServices.SetActive(true);
    }
    public void CloseGameServices()
    {
        hmsGameServices.SetActive(false);
    }
    public void StartGameAnalytics(){
        Dictionary<string, object> bundle = new();
        bundle.Add("_Result", "StartGame");
        AnalyticsManager.Instance.SendEvent("$StartGame",bundle);
    }
    public void EndGameAnalytics(float duration, string result){
        Dictionary<string, object> bundle = new();
        bundle.Add("$Duration", duration);
        bundle.Add("$Result", result);

        AnalyticsManager.Instance.SendEvent("$EndGame", bundle);
    }
}
