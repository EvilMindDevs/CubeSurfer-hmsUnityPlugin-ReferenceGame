using System;
using System.Collections;
using System.Collections.Generic;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.Ads.NativeAd;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NativeAdManager : MonoBehaviour
{
    // Start is called before the first frame update

    public string adUnitID = "testy63txaom86";
    NativeAd nativeAd;
    public RawImage ad_media;


    void Start()
    {
        Debug.Log("[HMS]LargeImageNative Start");
        ad_media.texture =  new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        if(AdsManager.Instance != null)
            LoadNativeAd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnNativeAdLoaded(NativeAd nativeAd)
    {
        Debug.Log("[HMS] OnNativeAdLoaded");
        this.nativeAd = nativeAd;

        foreach (var image in nativeAd.Images)
            StartCoroutine(DownloadImage(image.Uri.ToString()));
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        Debug.Log("[HMS] DownloadImage: " + MediaUrl);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            ad_media.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

        public void LoadNativeAd()
    {
        NativeAdLoader.Builder builder = new NativeAdLoader.Builder(adUnitID);


        builder.SetNativeAdLoadedListener(new NativeAdLoadedListener(new LargeImageNativeAdLoadedListener(OnNativeAdLoaded)))
            .SetAdListener(new AdStatusListener());
        NativeAdLoader nativeAdLoader = builder.Build();
       
        nativeAdLoader.LoadAd(new AdParam.Builder().Build());
    }

    private class LargeImageNativeAdLoadedListener : INativeAdLoadedListener
    {
        Action<NativeAd> OnNativeAdLoaded;
        public LargeImageNativeAdLoadedListener(Action<NativeAd> OnNativeAdLoaded) 
        {
            this.OnNativeAdLoaded = OnNativeAdLoaded;
        }
        public void onNativeAdLoaded(NativeAd nativeAd)
        {
            Debug.Log("[HMS] onNativeAdLoaded");
            OnNativeAdLoaded.Invoke(nativeAd);
        }
    }

    private class AdStatusListener : IAdListener
    {
        public AdStatusListener(){}

        public void OnAdClicked()
        {
            Debug.Log("[HMS] OnNativeAdClicked");
        }

        public void OnAdClosed()
        {
            Debug.Log("[HMS] OnNativeAdClosed");
        }

        public void OnAdFailed(int reason)
        {
            Debug.Log("[HMS] OnNativeAdFailed reason:" + reason);
        }

        public void OnAdImpression()
        {
            Debug.Log("[HMS] OnNativeAdImpression");
        }

        public void OnAdLeave()
        {
            Debug.Log("[HMS] OnNativeAdLeave");
        }

        public void OnAdLoaded()
        {
            Debug.Log("[HMS] OnNativeAdAdAdAdAdLoaded");
        }

        public void OnAdOpened()
        {
            Debug.Log("[HMS] OnNativeAdOpened");
        }
    }
}
