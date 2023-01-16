using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HmsPlugin;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using System.Linq;
using HuaweiMobileServices.Base;

public class IAPManager : Singleton<IAPManager>
{
    
    // Start is called before the first frame update
    [SerializeField]
    private Button NoAdsButton;

    [SerializeField]
    private Button DoubleScoreButton;

    [SerializeField]
    private Button PremiumButton;

    [SerializeField]
    private Text DoubleScoreTextCount;

    private bool CheckSuccess =>  PlayerPrefs.GetInt("CheckSuccess", 0) == 1;

    private const string publicKey = "Your Public Key Here";

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
        if(CheckSuccess){
            HMSIAPManager.Instance.CheckIapAvailability();
         }
        NoAdsButton?.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void DeliveryControl()
    {
        // https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/redelivering-consumables-0000001051356573
        // Check whether the purchase token is in the purchase token list of the delivered products.

        IIapClient iapClient = Iap.GetIapClient();

        // Construct an OwnedPurchasesReq object.
        OwnedPurchasesReq ownedPurchasesReq = new OwnedPurchasesReq();
        // priceType: 0: consumable; 1: non-consumable; 2: subscription
        ownedPurchasesReq.PriceType = PriceType.IN_APP_CONSUMABLE;

        /*  ObtainOwnedPurchases
            Queries purchase data of the products that a user has bought.

            For consumables, this method returns purchase data of those already bought but not consumed.

            For non-consumables, this method returns purchase data of all the products that have been bought.

            For subscriptions, this method returns only the currently active subscription relationships.
        */
         ITask<OwnedPurchasesResult> task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);
         task.AddOnSuccessListener((result) => {
             if (result != null && result.InAppPurchaseDataList != null) {
                 for (int i = 0; i < result.InAppPurchaseDataList.Count; i++) {
                        InAppPurchaseData inAppPurchaseData = result.InAppPurchaseDataList[i];
                        string inAppPurchaseSignature = result.InAppSignature[i];
                        string inAppPurchaseDataRawJSON = result.InAppPurchaseDataListRawJSON[i];
                        
                        // Use the IAP public key to verify the signature of inAppPurchaseData.
                        bool isSignatureValid = Helpers.VerifyPurchase(inAppPurchaseDataRawJSON,inAppPurchaseSignature,publicKey);

                        if (isSignatureValid) {
                            // Check whether the purchase token is in the purchase token list of the delivered products.
                            Debug.Log("[IAPManager]: isSignatureValid. " + inAppPurchaseDataRawJSON);
                            // Change the applicationId value to a string                                                       
                            var data = JsonUtility.FromJson<InAppPurchaseDataRawJSON>(inAppPurchaseDataRawJSON);
                            if(data?.purchaseState == 0){
                                // If the purchase token is not in the purchase token list of the delivered products, deliver the product.
                                Debug.Log("[IAPManager]: PurchaseState == 0");
                                // If the purchase token is in the purchase token list of the delivered products, do not deliver the product.
                                // Call the consumeOwnedPurchase API to consume the product.
                                ConsumeOwnedPurchaseReq consumeOwnedPurchaseReq = new ConsumeOwnedPurchaseReq
                                {
                                    PurchaseToken = inAppPurchaseData.PurchaseToken
                                };
                                ITask<ConsumeOwnedPurchaseResult> consumeTask = iapClient.ConsumeOwnedPurchase(consumeOwnedPurchaseReq);
                                consumeTask.AddOnSuccessListener((consumeResult) => {
                                    // Consume the product successfully.
                                    Debug.Log("[IAPManager]: Consume the product successfully.");
                                }).AddOnFailureListener((exception) => {
                                    // Consume the product failed.
                                    Debug.Log("[IAPManager]: Consume the product failed.");
                                });
        
                            }
                            else{
                                Debug.Log("[IAPManager]: PurchaseState != 0");
                            }
                        } 
                        else {
                            // The signature is invalid.
                            Debug.Log("[IAPManager]: The signature is invalid.");
                        }       
                 }
             }
         }).AddOnFailureListener((exception) => {
             // Handle the exception.
             Debug.Log("[IAPManager]: Handle the exception.");
         });
         
    }
    public void BuyProduct(string productID)
    {
        Debug.Log("BuyProduct");

        HMSIAPManager.Instance.BuyProduct(productID);

    }
    IEnumerator AfterBuyProductSuccess(PurchaseResultInfo obj)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("[IAPManager]: AfterBuyProductSuccess");
        switch(obj.InAppPurchaseData.ProductId)
        {
            case "NoAdsProduct":
                NoAdsProductProcessAfterBuySuccess();
                break;
            case "DoubleScore":
                DoubleScoreProcessAfterBuySuccess();
                break;
            case "premium":
                PremiumProcessAfterBuySuccess();
                break;
        }
    }
    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        //After back to unity activity 
        StartCoroutine(AfterBuyProductSuccess(obj));
    }
    // For sandbox testing
    private void OnBuyProductFailure(int code)
    {
        //https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/client-error-code-0000001050746111
        // this command is the solution of the error of product has already been purchased.(ORDER_PRODUCT_OWNED)
        Debug.Log("[IAPManager]: OnBuyProductFailure code : " + code);
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
                Debug.Log("[IAPManager]: OnObtainOwnedPurchasesSuccess : " + obj.ProductId);
                HMSIAPManager.Instance.ConsumePurchaseWithToken(obj.PurchaseToken);
            }
        }
    }

    private void OnObtainOwnedPurchasesFailure(HMSException obj)
    {
        Debug.Log("[IAPManager]: OnObtainOwnedPurchasesFailure : " + obj.Message);
    }

    public void OnObtainProductInfoSuccess(IList<ProductInfoResult> result)
    {
        Debug.Log("[IAPManager]: OnObtainProductInfoSuccess");
    }

    private void OnCheckIapAvailabilityFailure(HMSException obj)
    {
        Debug.Log("[IAPManager]: IAP is not ready. " + obj.Message);
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        Debug.Log("[IAPManager]: IAP is ready.");
        PlayerPrefs.SetInt("CheckSuccess", 1);
        DeliveryControl();
        ControlIAP();
        
    }

    void ControlIAP()
    {
        bool hasDoubleScore = false;
        HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            hasDoubleScore = restoredProducts.ItemList?.Any(t=> t == "DoubleScore") ?? false;
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
                        NoAdsButton?.gameObject?.SetActive(false);
                    }
                    if (item.ProductId == "DoubleScore")
                    {
                        // Give your player coins here.Resources.FindObjectsOfTypeAll()
                        Debug.Log("[IAPManager]: DoubleScore Purchased"); 
                        CheckDoubleScore();            
                    }
                    if (item.ProductId == "premium")
                    {
                        // Grant your player premium feature.
                        Debug.Log("[IAPManager]: premium Purchased");
                        Debug.Log("[IAPManager]: buttonhas been " + PremiumButton?.gameObject != null ? "Yes" : "No");
                        Debug.Log("[IAPManager]: buttonhas active " + (PremiumButton?.gameObject?.activeSelf == true ? "Yes" : "No"));
                        PremiumButton.gameObject.SetActive(true);
                        PlayerPrefs.SetInt("premium", 1);
                        if(AdsManager.Instance != null)
                            AdsManager.Instance.HideAds();
                        NoAdsButton?.gameObject?.SetActive(false);
                    }
            }
        });

        if(!hasDoubleScore && PlayerPrefs.GetInt("DoubleScore",0) > 0){
            Debug.Log("[IAPManager]: DoubleScore Purchased with PlayerPrefs");
            var getDoubleScourCount = PlayerPrefs.GetInt("DoubleScore",0);    
            if(DoubleScoreButton != null) {
                DoubleScoreButton?.gameObject?.SetActive(true);   
                DoubleScoreButton.interactable = true;         
            } 
            else 
            {
                GameObject doubleScoreButton = GameObject.Find("DoubleScoreButton");
                if(doubleScoreButton != null) {
                    doubleScoreButton.SetActive(false);
                    doubleScoreButton.GetComponent<Button>().interactable = true;
                } else {
                    Debug.LogError("Can't find DoubleScoreButton");
                }
            }
            var doubleScoreTextCount = GameObject.Find("DoubleScoreTextCount");
            if (doubleScoreTextCount != null){
                doubleScoreTextCount.GetComponent<Text>().text = getDoubleScourCount.ToString();
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
        PlayerPrefs.SetInt("UseDoubleScore",0);
        var getDoubleScoreCount = PlayerPrefs.GetInt("DoubleScore",0);
        if(getDoubleScoreCount > 0){
            var newDoubleScoreCount = getDoubleScoreCount - 1;
            PlayerPrefs.SetInt("DoubleScore",newDoubleScoreCount );
            PlayerPrefs.SetInt("UseDoubleScore",1);
            var doubleScoreTextCount = GameObject.Find("DoubleScoreTextCount");

            if (doubleScoreTextCount != null){
                doubleScoreTextCount.GetComponent<Text>().text = newDoubleScoreCount.ToString();
            }
        } 
        DoubleScoreButton.interactable = false;
    }
    void NoAdsProductProcessAfterBuySuccess(){
        PlayerPrefs.SetInt("NoAdsProduct", 1);
        if(AdsManager.Instance != null)
            AdsManager.Instance.HideAds();
    }
    void DoubleScoreProcessAfterBuySuccess(){
        Debug.Log("[IAPManager]: DoubleScore Purchased");
        
        if(DoubleScoreButton != null) {
            DoubleScoreButton?.gameObject?.SetActive(true);   
            DoubleScoreButton.interactable = true;         
        } 
        else 
        {
            GameObject doubleScoreButton = GameObject.Find("DoubleScoreButton");
            if(doubleScoreButton != null) {
                doubleScoreButton.SetActive(false);
                doubleScoreButton.GetComponent<Button>().interactable = true;
            } else {
                Debug.LogError("Can't find DoubleScoreButton");
            }
        }
        var getDoubleScoreCount = PlayerPrefs.GetInt("DoubleScore",0);  
        getDoubleScoreCount++;  
        PlayerPrefs.SetInt("DoubleScore", getDoubleScoreCount);
        var doubleScoureText = GameObject.Find("DoubleScoreTextCount");
        Debug.Log("[IAPManager]: DoubleScore Count: " + (doubleScoureText == null ? "null" : doubleScoureText.GetComponent<Text>().text));
        if(doubleScoureText != null)
            doubleScoureText.GetComponent<Text>().text = getDoubleScoreCount.ToString();
    }
    void PremiumProcessAfterBuySuccess(){
        Debug.Log("[IAPManager]: premium Purchased");  
        PlayerPrefs.SetInt("premium", 1);
        if(AdsManager.Instance != null)
            AdsManager.Instance.HideAds();
        NoAdsButton.interactable = false;
        NoAdsButton?.gameObject?.SetActive(false);
        PremiumButton?.gameObject?.SetActive(true);
    }
    void CheckDoubleScore(){
        if(PlayerPrefs.GetInt("DoubleScore",0) > 0){
            Debug.Log("[IAPManager]: DoubleScore Purchased with PlayerPrefs");
            var getDoubleScourCount = PlayerPrefs.GetInt("DoubleScore",0);    
            if(DoubleScoreButton != null) {
                DoubleScoreButton?.gameObject?.SetActive(true);   
                DoubleScoreButton.interactable = true;         
            } 
            else 
            {
                GameObject doubleScoreButton = GameObject.Find("DoubleScoreButton");
                if(doubleScoreButton != null) {
                    doubleScoreButton.SetActive(false);
                    doubleScoreButton.GetComponent<Button>().interactable = true;
                } else {
                    Debug.LogError("Can't find DoubleScoreButton");
                }
            }
            var doubleScoreTextCount = GameObject.Find("DoubleScoreTextCount");
            if (doubleScoreTextCount != null){
                doubleScoreTextCount.GetComponent<Text>().text = getDoubleScourCount.ToString();
            }
        }

        
    }

    internal class InAppPurchaseDataRawJSON
    {
        public bool autoRenewing { get; set; }
        public string orderId { get; set; }
        public string packageName { get; set; }
        public int applicationId { get; set; }
        public string applicationIdString { get; set; }
        public int kind { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public long purchaseTime { get; set; }
        public long purchaseTimeMillis { get; set; }
        public int purchaseState { get; set; }
        public string developerPayload { get; set; }
        public string purchaseToken { get; set; }
        public int consumptionState { get; set; }
        public int confirmed { get; set; }
        public string currency { get; set; }
        public int price { get; set; }
        public string country { get; set; }
        public string payOrderId { get; set; }
        public string payType { get; set; }
        public string sdkChannel { get; set; }
    }



}
