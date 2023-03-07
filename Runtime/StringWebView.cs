using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;
namespace StringSDK
{
public class StringWebView : MonoBehaviour 
{
    public CanvasWebViewPrefab vuplexWeb;

    async void Start() 
    {
        // setup webview(draw and instantiate)
        setup();
        // Load a URL once the weview has loaded
        await vuplexWeb.WaitUntilInitialized();
        vuplexWeb.WebView.LoadUrl("https://payment-iframe.dev.string-api.xyz/unity.html");
        // let the event manager register for events
        WebEventManager.RegisterForEvent(vuplexWeb);
    }

    private void setup() 
    { 
        vuplexWeb = CanvasWebViewPrefab.Instantiate();
        // Position canvas to take all available space
        var canvas = GameObject.Find("StringWebViewPrefab");
        vuplexWeb.transform.SetParent(canvas.transform, false); 
        var rectTransform = vuplexWeb.transform as RectTransform;     
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        vuplexWeb.transform.localScale = Vector3.one;
    }
}
}