using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiMobileServices.Analystics;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using System.Net.Mail;
using System.Linq;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private readonly string TAG = "[HMS] AnalyticsManager ";

    public void SendEvent(string eventIdFieldText)
    {
        Debug.Log(TAG+": Not Fields event");
        HMSAnalyticsKitManager.Instance.SendEventWithBundle(eventIdFieldText, string.Empty, string.Empty);
    }

    public void SendEvent(string eventIdFieldText, string keyFieldtext, string valueFieldText)
    {
        if (string.IsNullOrEmpty(eventIdFieldText) && string.IsNullOrEmpty(keyFieldtext) && string.IsNullOrEmpty(valueFieldText))
        {
            Debug.Log(TAG+": Fill Fields");
        }
        else
        {
            Debug.Log(TAG+eventIdFieldText + " " + keyFieldtext + " " + valueFieldText);
            HMSAnalyticsKitManager.Instance.SendEventWithBundle(eventIdFieldText, keyFieldtext, valueFieldText);
        }
    }

    public void SendEvent(string eventIdFieldText, Dictionary<string,object> fields)
    {
        if (string.IsNullOrEmpty(eventIdFieldText) && fields.Any())
        {
            Debug.Log(TAG+": Fill Fields");
        }
        else
        {
            Debug.Log(TAG+eventIdFieldText + " " + fields.Count);
            HMSAnalyticsKitManager.Instance.SendEventWithBundle(eventIdFieldText,fields);
        }
    }

}


