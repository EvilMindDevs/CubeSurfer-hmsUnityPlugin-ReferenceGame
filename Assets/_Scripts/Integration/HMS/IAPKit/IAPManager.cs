using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HmsPlugin;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using System;
using HuaweiMobileServices.Id;
using UnityEngine.UI;

public class IAPManager : Singleton<IAPManager>
{
    
    // Start is called before the first frame update
    [SerializeField]
    private Button NoAdsButton;

    [SerializeField]
    private Button DoubleScoreButton;

    [SerializeField]
    private Button PremiumButton;

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
        HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;

        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

        HMSIAPManager.Instance.OnBuyProductFailure = OnBuyProductFailure;
        HMSIAPManager.Instance.OnObtainProductInfoSuccess += OnObtainProductInfoSuccess;
        
    }

    

    void Start()
    {
        Debug.Log("[IAPManager]: IapManager Started");
        HMSIAPManager.Instance.CheckIapAvailability();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void BuyProduct(string productID)
    {
        Debug.Log("BuyProduct");

        HMSIAPManager.Instance.BuyProduct(productID);
    }

    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        if (obj.InAppPurchaseData.ProductId == "NoAdsProduct")
        {
            //HMSAdsKitManager.Instance.HideBannerAd();
            PlayerPrefs.SetInt("NoAdsProduct", 1);
            AdsManager.Instance.HideAds();
        }
        if (obj.InAppPurchaseData.ProductId == "DoubleScore")
        {
            Debug.Log("[IAPManager]: DoubleScore Purchased");
            DoubleScoreButton.enabled = true;        
            PlayerPrefs.SetInt("DoubleScore", 1);
        }
        if (obj.InAppPurchaseData.ProductId == "premium")
        {
            Debug.Log("[IAPManager]: premium Purchased");  
                PlayerPrefs.SetInt("premium", 1);
                if(AdsManager.Instance != null)
                    AdsManager.Instance.HideAds();
                NoAdsButton.interactable = false;
                NoAdsButton.enabled = false;
                PremiumButton.enabled = true;
        }
    }

        // For sandbox testing
    private void OnBuyProductFailure(int code)
    {
        //https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/client-error-code-0000001050746111
        // this command is the solution of the error of product has already been purchased.(ORDER_PRODUCT_OWNED)

        if (code == OrderStatusCode.ORDER_PRODUCT_OWNED)
        {
            HMSIAPManager.Instance.OnObtainOwnedPurchasesSuccess = OnObtainOwnedPurchasesSuccess;
            HMSIAPManager.Instance.ObtainOwnedPurchases(PriceType.IN_APP_NONCONSUMABLE);
        }
    }

    private void OnObtainOwnedPurchasesSuccess(OwnedPurchasesResult result)
    {
        if (result != null)
        {
            foreach (var obj in result.InAppPurchaseDataList)
            {
                HMSIAPManager.Instance.ConsumePurchaseWithToken(obj.PurchaseToken);
            }
        }
    }



    private void OnCheckIapAvailabilityFailure(HMSException obj)
    {
        Debug.Log("IAP is not ready. " + obj.Message);
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        Debug.Log("IAP is ready.");
        ControlIAP();
        
    }

    void ControlIAP()
    {
        HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            var productPurchasedList = new List<InAppPurchaseData>(restoredProducts.InAppPurchaseDataList);
            Debug.Log("[IAPManager]: RestorePurchases: " + productPurchasedList.Count);
            foreach (var item in productPurchasedList)
            {
                Debug.Log("[IAPManager]: RestorePurchasesProduct: " + item.ProductId);
                    if (item.ProductId == "NoAdsProduct")
                    {
                        PlayerPrefs.SetInt("NoAdsProduct", 1);
                        if(AdsManager.Instance != null)
                            AdsManager.Instance.HideAds();
                        Debug.Log("[IAPManager]: NoAdsProduct Purchased");
                        NoAdsButton.interactable = false;
                        NoAdsButton.enabled = false;
                    }
                    if (item.ProductId == "DoubleScore")
                    {
                        // Give your player coins here.
                        Debug.Log("[IAPManager]: DoubleScore Purchased");
                        PlayerPrefs.SetInt("DoubleScore", 1);
                        DoubleScoreButton.enabled = true;
                    }
                    if (item.ProductId == "premium")
                    {
                        // Grant your player premium feature.
                        Debug.Log("[IAPManager]: premium Purchased");
                        PremiumButton.enabled = true;
                        PlayerPrefs.SetInt("premium", 1);
                        if(AdsManager.Instance != null)
                            AdsManager.Instance.HideAds();
                        NoAdsButton.interactable = false;
                        NoAdsButton.enabled = false;
                    }
            }
        });
    }

    public void OnObtainProductInfoSuccess(IList<ProductInfoResult> result)
    {
        Debug.Log("[IAPManager]: OnObtainProductInfoSuccess");
        foreach (var productInfo in result)
        {
            if(productInfo.ReturnCode == 0){

                foreach(var item in productInfo.ProductInfoList){
                    var props = item.GetType().GetProperties();
                    foreach(var prop in props){
                        //Debug.Log("[IAPManager]: " + prop.Name + " : " + prop.GetValue(item));
                    }
                }
            }
          
        }

    }
    void ChangeColor()
    {
        // Find the object you want to modify
        GameObject objectToModify = GameObject.Find("Boy01_Body_Geo");
        
        // Get a reference to the object's Renderer component
        SkinnedMeshRenderer renderer = objectToModify.GetComponent<SkinnedMeshRenderer>();
        
        // Get a reference to the material you want to modify
        Material material = renderer.material;
        
        // Modify the material's color
        material.SetColor("_Color", UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

        Debug.Log("[IAPManager]: ChangeColor");


        

    }

    public void ClickButtonPremium(){
        Debug.Log("[IAPManager]: ClickButtonPremium");
        var buttonText = GameObject.Find("PremiumButtonText").GetComponent<Text>();
        if(PlayerPrefs.GetInt("premiumButton",0) == 0){
            InvokeRepeating("ChangeColor", 0, 0.5f);
            PlayerPrefs.SetInt("premiumButton", 1);
            buttonText.text = "DISCO ON";
            return;
        }
        CancelInvoke("ChangeColor");
        PlayerPrefs.SetInt("premiumButton", 0);
        buttonText.text = "DISCO OFF";
    }

    public void ClickDoubleScore(){
        Debug.Log("[IAPManager]: ClickDoubleScore");
        PlayerPrefs.SetInt("DoubleScore", 1);
        DoubleScoreButton.interactable = false;
    }
}
