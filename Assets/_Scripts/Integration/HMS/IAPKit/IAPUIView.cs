using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPUIView : MonoBehaviour
{
    [SerializeField] private Button Btn_ItemRemoveAds;
    [SerializeField] private Button Btn_OpenIAPMenu;
    [SerializeField] private GameObject IAPMenu;
    private Text Txt_Log;



    void Awake(){

    }

    // Start is called before the first frame update
    void Start()
    {
        Btn_ItemRemoveAds.onClick.AddListener(ButtonClick_BuyItemRemoveAds);
        Btn_OpenIAPMenu.onClick.AddListener(ButtonClick_OpenIAPMenu);
        IapDemoManager.IAPLog += OnIAPLog;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnIAPLog(string log)
    {
        Txt_Log.text = log;
    }

    private void ButtonClick_BuyItemRemoveAds()
    {
        IAPManager.Instance.BuyProduct("NoAdsProduct");
    }
    public void ButtonClick_OpenIAPMenu()
    {
        IAPMenu.SetActive(true);
    }

    public void CloseIAPArea()
    {
        IAPMenu.SetActive(false);
    }

}
