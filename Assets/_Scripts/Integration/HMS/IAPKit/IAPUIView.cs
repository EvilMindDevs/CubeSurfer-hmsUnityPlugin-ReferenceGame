using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HuaweiMobileServices.IAP;
using HmsPlugin;
using System.Linq;

public class IAPUIView : MonoBehaviour
{
    [SerializeField] private Button Btn_ItemRemoveAds;
    [SerializeField] private Button Btn_ManageSubscriptions;
    [SerializeField] private Button Btn_EditSubscriptions;
    [SerializeField] private Button Btn_OpenIAPMenu;
    [SerializeField] private GameObject IAPMenu;
    [SerializeField] private GameObject IAPButtons;
    private Text Txt_Log;

    // Start is called before the first frame update

    void Awake()
    {
    }
    void Start()
    {
        Btn_ItemRemoveAds.onClick.AddListener(ButtonClick_BuyItemRemoveAds);
        Btn_OpenIAPMenu.onClick.AddListener(ButtonClick_OpenIAPMenu);
        Btn_ManageSubscriptions.onClick.AddListener(OpenSubscriptionManagementScreen);
        Btn_EditSubscriptions.onClick.AddListener(OpenSubscriptionEditingScreen);
        IapDemoManager.IAPLog += OnIAPLog;
        IAPButtons?.SetActive(true);
        Debug.Log("[IAPUIView]: IAPUIView Started");

    }

    private void OnIAPLog(string log)
    {
        Txt_Log.text = log;
    }

    void ButtonClick_BuyItemRemoveAds()
    {
        IAPManager.Instance.BuyProduct("NoAdsProduct");
    }
    public void ButtonClick_OpenIAPMenu()
    {
        IAPMenu.SetActive(true);
        GameObject lastGameObj = null;
        var productList = HMSIAPManager.Instance.GetProductInfoList();
        var distinctProductList = productList.GroupBy(x => x.ProductId.Trim()).Select(y => y.First()).ToList();
        GameObject original = GameObject.FindGameObjectWithTag("BasePurchaseProductCart");
        int index = 0;
        foreach (var product in distinctProductList)
        {
            Debug.Log("[IAPUIView]: ProductName: " + product.ProductName);
            var newGameObject = CreateObjCopy(original);
            InitializeProductUI(newGameObject, product, index);
            lastGameObj = newGameObject;
            index++;
        }


    }
    private void OpenSubscriptionEditingScreen()
    {
        HMSIAPManager.Instance.RedirectingtoSubscriptionEditingScreen("premium");
    }

    private void OpenSubscriptionManagementScreen()
    {
        HMSIAPManager.Instance.RedirectingtoSubscriptionManagementScreen("premium");
    }
    public void CloseIAPArea()
    {
        IAPMenu.SetActive(false);
    }
    private void InitializeProductUI(GameObject productObj, ProductInfo product, int index)
    {
        productObj.gameObject.SetActive(true);
        var itemName = productObj.transform.Find("ItemName").gameObject.GetComponent<Text>();
        itemName.text = product.ProductName;
        var itemCost = productObj.transform.Find("ItemCost").gameObject.GetComponent<Text>();
        itemCost.text = product.Price;
        var itemDesc = productObj.transform.Find("ItemDesc").gameObject.GetComponent<Text>();
        itemDesc.text = product.ProductDesc;
        var itemImage = productObj.transform.Find("ItemImage").gameObject.GetComponent<Image>();
        Debug.Log("[IAPUIView]: ProductId: " + product.ProductId);
        itemImage.sprite = Resources.Load<Sprite>("ProductImages/" + product.ProductId) ?? Resources.Load<Sprite>("ProductImages/DefaultProductImage");
        var itemBuyButton = productObj.transform.Find("ItemBuyButton").gameObject.GetComponent<Button>();
        itemBuyButton.onClick.AddListener(() => IAPManager.Instance.BuyProduct(product.ProductId));

        if(index != 0 && index % 2 == 0)
            productObj.transform.position  = new Vector3(productObj.transform.position.x, productObj.transform.position.y - (200 * (index + 1)), productObj.transform.position.z);

        if(index != 0 && index % 2 != 0)
            productObj.transform.position  = new Vector3(productObj.transform.position.x+(250 * (index + 1)) , productObj.transform.position.y, productObj.transform.position.z);
    }
    private GameObject CreateObjCopy(GameObject original)
    {
        // Create a copy of the original object
        GameObject copy = Instantiate(original);

        // Set the parent of the copy to be the same as the original's parent
        copy.transform.SetParent(original.transform.parent);

        // Set the position, rotation and scale of the copy to be the same as the original
        copy.transform.localPosition = original.transform.localPosition;
        copy.transform.localRotation = original.transform.localRotation;
        copy.transform.localScale = original.transform.localScale;

        // Copy the components from the original to the copy
        foreach (Component component in original.GetComponents<Component>())
        {
            if (component == null || component is Transform)
            {
                continue;
            }
            Component newComponent = Object.Instantiate(component);
            newComponent.transform.SetParent(copy.transform);
            newComponent.transform.localPosition = Vector3.zero;
            newComponent.transform.localRotation = Quaternion.identity;
            newComponent.transform.localScale = Vector3.one;
        }
       // original.gameObject.SetActive(false);
        return copy;

    }

}
