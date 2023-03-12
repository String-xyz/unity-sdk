using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

namespace StringSDK
{
public class WebEventManager:MonoBehaviour  {
    private const string CHANNEL = "STRING_PAY";
    private const string INIT_IFRAME = "init_iframe";
    private const string SUBMIT_CARD = "submit_card"; 

    public static string CardToken  { private set; get; }
    public static bool CardValid    { private set; get; }
    public static string CardVendor { private set; get; }
    public static string FingerprintVisitorId { private set; get; }
    public static string FingerprintRequestId { private set; get; }

    public static event Action<string> EventReceived;
    public static event Action<StringEvent<TokenizationPayload>> CardTokenized;
    public static event Action<StringEvent<ValidationPayload>> CardValidationChanged;
    public static event Action<StringEvent<VendorChangedPayload>> CardVendorChanged;
    
    public static CanvasWebViewPrefab Webview { private set; get; }

    public static void RegisterForEvent(CanvasWebViewPrefab web)
    {  
        Webview = web;
        Webview.WebView.MessageEmitted += handleEvent;
        InitIframe();
    }
    
    static void handleEvent(object sender, EventArgs<string> e) 
    { 
        Debug.Log("event from iframe "+e.Value);
        EventReceived?.Invoke(e.Value);
        parseEventPayload(e.Value);
    }

    public static string CreateEvent<T>(string name, T data)
    {
        var eventData = new PayloadData<T>(name, data);
        return new StringEvent<T>(CHANNEL, eventData).ToJSON();      
    }
    public static void InitIframe() 
    {
        sendEvent(new EmptyPayloadData(CHANNEL, INIT_IFRAME).ToJSON());
    }

     public static void SubmitCard() 
    {
        sendEvent(new EmptyPayloadData(CHANNEL, SUBMIT_CARD).ToJSON());
    }

    async private static void sendEvent(string payload)
    {
        Debug.Log("sending event "+ payload);
        await Webview.WebView.WaitForNextPageLoadToFinish();
        Webview.WebView.PostMessage(payload);
    }

    private static void parseEventPayload(string strPayload) 
    { 
        var  payload = EmptyPayloadData.FromJSON(strPayload);
        switch(payload.data.eventName) 
        { 
            case "card_tokenized":
               var tokenization = StringEvent<TokenizationPayload>.FromJSON(strPayload);
               if (tokenization == null) 
                 { 
                    break;
                 }
               CardToken = tokenization.data.data.token;
               CardValid = true;

               CardTokenized?.Invoke(tokenization);
                break;
            case "card_tokenization_failed":
                CardToken = "";
                CardValid = false;
                CardVendor = "";
                break;
            case "card_vendor_changed":
                var vendorChanged = StringEvent<VendorChangedPayload>.FromJSON(strPayload);
                if (vendorChanged == null) 
                 { 
                    break;
                 }

                CardVendor = vendorChanged.data.data.cardVendor;
                CardVendorChanged?.Invoke(vendorChanged);
                 break;
            case "card_validation_changed":
                var validation = StringEvent<ValidationPayload>.FromJSON(strPayload);
                 if (validation == null) 
                 { 
                    break;
                 }
                CardValid = validation.data.data.valid;
                CardValidationChanged?.Invoke(validation);
                break;
            case "fingerprint":
                var fingerprint = StringEvent<FingerprintPayload>.FromJSON(strPayload);
                 if (fingerprint == null) 
                 { 
                    break;
                 }
                FingerprintVisitorId = fingerprint.data.data.visitorId;
                FingerprintRequestId = fingerprint.data.data.requestId;
                break;
            default:
                break;
        }
    }
}
}