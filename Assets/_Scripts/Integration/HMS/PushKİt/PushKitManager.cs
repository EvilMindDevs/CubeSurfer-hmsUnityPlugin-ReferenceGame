using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using HmsPlugin;

public class PushKitManager : Singleton<PushKitManager>
{
    // Start is called before the first frame update
    void Start()
    {
         InvokeRepeating("CheckHMSPushKitManager", 0.5f, 0.5f);
    }


    private void CheckHMSPushKitManager()
    {
        Debug.Log("Kardeşim burası Kit manager");
        if (HMSPushKitManager.Instance != null)
        {
            CancelInvoke("CheckHMSPushKitManager");
            HMSPushKitManager.Instance.OnNotificationMessage = OnNotificationMessage;
            HMSPushKitManager.Instance.NotificationMessageOnStart = NotificationMessageOnStart;
            HMSPushKitManager.Instance.Init();
            Debug.Log("CheckHMSPushKitManager is initialized");

        }
    }

        private void OnNotificationMessage(NotificationData data)
    {
        Debug.Log("[HMSPushDemo] CmdType: " + data.CmdType);
        Debug.Log("[HMSPushDemo] MsgId: " + data.MsgId);
        Debug.Log("[HMSPushDemo] NotifyId: " + data.NotifyId);
        Debug.Log("[HMSPushDemo] KeyValueJSON: " + data.KeyValueJSON);
    }

    private void NotificationMessageOnStart(NotificationData data)
    {
        Debug.Log("[HMSPushDemo] CmdType: " + data.CmdType);
        Debug.Log("[HMSPushDemo] MsgId: " + data.MsgId);
        Debug.Log("[HMSPushDemo] NotifyId: " + data.NotifyId);
        Debug.Log("[HMSPushDemo] KeyValueJSON: " + data.KeyValueJSON);
        /* TODO: Make your own logic here
         * notificationDataOnStart = data;
         */
    }
}
