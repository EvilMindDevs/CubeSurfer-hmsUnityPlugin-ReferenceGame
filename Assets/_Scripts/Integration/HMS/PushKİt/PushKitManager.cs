using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using HmsPlugin;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
// 053543007As.

public class PushKitManager : Singleton<PushKitManager>
{

    private string pushToken;
    private Text remoteMessageText, tokenText;

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
        HMSPushKitManager.Instance.OnTokenSuccess = OnNewToken;
        HMSPushKitManager.Instance.OnTokenFailure = OnTokenError;
        HMSPushKitManager.Instance.OnTokenBundleSuccess = OnNewToken;
        HMSPushKitManager.Instance.OnTokenBundleFailure = OnTokenError;
        HMSPushKitManager.Instance.OnMessageSentSuccess = OnMessageSent;
        HMSPushKitManager.Instance.OnSendFailure = OnSendError;
        HMSPushKitManager.Instance.OnMessageDeliveredSuccess = OnMessageDelivered;
        HMSPushKitManager.Instance.OnMessageReceivedSuccess = OnMessageReceived;
        HMSPushKitManager.Instance.OnNotificationMessage = OnNotificationMessage;
        HMSPushKitManager.Instance.NotificationMessageOnStart = NotificationMessageOnStart;
        HMSPushKitManager.Instance.Init();
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

    public void OnNewToken(string token)
    {
        Debug.Log($"[HMS] Push token from OnNewToken is {token}");
        if (string.IsNullOrWhiteSpace(token))
        {
            pushToken = token;
            tokenText.text = "Push Token: " + pushToken;
        }
    }

    public void OnTokenError(Exception e)
    {
        Debug.Log("Error asking for Push token");
        Debug.Log(e.StackTrace);
    }

    public void OnMessageReceived(RemoteMessage remoteMessage)
    {
        var id = remoteMessage.MessageId;
        var from = remoteMessage.From;
        var to = remoteMessage.To;
        var data = remoteMessage.Data;
        remoteMessageText.text = $"ID: {id}\nFrom: {from}\nTo: {to}\nData: {data}";
    }

    public void OnNewToken(string token, Bundle bundle)
    {
        Debug.Log($"[HMS] Push token from OnNewToken is {token}");
        if (string.IsNullOrWhiteSpace(token))
        {
            pushToken = token;
            tokenText.text = "Push Token: " + pushToken;
        }
    }

    public void OnTokenError(Exception exception, Bundle bundle)
    {
        Debug.Log("Error asking for Push token");
        Debug.Log(exception.StackTrace);
    }

    public void OnMessageSent(string msgId)
    {
        Debug.Log(msgId);
    }

    public void OnMessageDelivered(string msgId, Exception exception)
    {
        Debug.Log("Message Delivered");
        Debug.Log(exception.StackTrace + " , Message ID:" + msgId);
    }

    public void OnSendError(string msgId, Exception exception)
    {
        Debug.Log(exception.StackTrace + " , Message ID:" + msgId);
    }
}
