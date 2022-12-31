using System.Collections.Generic;
using HmsPlugin;
using UnityEngine;
using HuaweiMobileServices.Ads;
using System;

public class AdsManager : Singleton<AdsManager>
{
    // Start is called before the first frame update

    public event Action RewardAdCompleted;
    [NonSerialized] public bool isAdRewarded = false;
    

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
        HMSAdsKitManager.Instance.OnRewarded = OnRewarded;
        HMSAdsKitManager.Instance.OnRewardAdCompleted = OnRewardAdCompleted;
        HMSAdsKitManager.Instance.OnRewardAdClosed+= OnRewardAdClosed;
        HMSAdsKitManager.Instance.OnInterstitialAdClosed = OnInterstitialAdClosed;

        HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;
        HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
        HMSAdsKitManager.Instance.RequestConsentUpdate();

        #region SetNonPersonalizedAd , SetRequestLocation

        var builder = HwAds.RequestOptions.ToBuilder();

        builder
            .SetConsent("tcfString")
            .SetNonPersonalizedAd((int)NonPersonalizedAd.ALLOW_ALL)
            .Build();

        bool requestLocation = true;
        var requestOptions = builder.SetConsent("testConsent").SetRequestLocation(requestLocation).Build();

        Debug.Log($"RequestOptions NonPersonalizedAds:  {requestOptions.NonPersonalizedAd}");
        Debug.Log($"Consent: {requestOptions.Consent}");


        #endregion
    }

    private void OnRewarded(Reward reward)
    {
        Debug.Log($"OnRewarded: {reward.Name} {reward.Amount}");
    }

    private void OnInterstitialAdClosed()
    {
        Debug.Log("OnInterstitialAdClosed");
    }

    private void OnConsentFail(string error)
    {
        Debug.Log($"OnConsentFail: {error}");
    }

    private void OnConsentSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
    {
        Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess consentStatus:{consentStatus} isNeedConsent:{isNeedConsent}");
        foreach (var AdProvider in adProviders)
        {
            Debug.Log($"[HMS] AdsDemoManager OnConsentSuccess adproviders: Id:{AdProvider.Id} Name:{AdProvider.Name} PrivacyPolicyUrl:{AdProvider.PrivacyPolicyUrl} ServiceArea:{AdProvider.ServiceArea}");
        }
    }

    public void ShowInstertitialAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowInstertitialAd");
        HMSAdsKitManager.Instance.ShowInterstitialAd();
    }

    
    public void ShowRewardedAd()
    {
        Debug.Log("[HMS] AdsDemoManager ShowRewardedAd");
        HMSAdsKitManager.Instance.ShowRewardedAd();
        //complatedtestforpc
        //OnRewardAdCompleted();
    }

    public void OnRewardAdClosed(){
        Debug.Log("[HMS] HMSAdsKitManager OnRewardAdClosed!");
    }

    public void OnRewardAdCompleted()
    {
        Debug.Log("[HMS] HMSAdsKitManager OnRewardAdCompleted!");
        isAdRewarded = true;
    }

    public void SubscribeToContinueGameAfterReward()
    {
        RewardAdCompleted += GameManager.Instance.ContinueGameAfterReward;
    }

    public void SubscribeToDoubleScoreAfterReward()
    {
        RewardAdCompleted += GameManager.Instance.DoubleScoreAfterReward;
    }
    public void OnApplicationFocus(bool focusStatus) {
        if (focusStatus && isAdRewarded) {
            Debug.Log("App has gained focus");
            RewardAdCompleted?.Invoke();
        } 
    }





}
