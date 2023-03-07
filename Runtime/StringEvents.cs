using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringSDK
{
[Serializable]
public class TokenizationPayload 
{
    public string token;
    public string scheme;
    public int last4;
}

[Serializable]
public class ValidationPayload 
{
    public bool valid;
}

[Serializable]
public class VendorChangedPayload 
{ 
    public string cardVendor;
    public bool accepted;
}

[Serializable]
public class PayloadData <T>
{
    public string eventName;
    public T data;
    public PayloadData(string name, T strData) 
    { 
        eventName = name;
        data = strData;
    }
}

[Serializable]
public class StringEvent <T>
{ 
    public string channel;
    public PayloadData<T> data;
    public StringEvent(string chan, PayloadData<T> strData)
    {
        channel = chan;
        data = strData;
    }
    public static StringEvent<T> FromJSON(string jsonStr)
    {
     return JsonUtility.FromJson<StringEvent<T>>(jsonStr);
    }
    
    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

// TODO: Make generic data fields nullable to avoid this class
[Serializable]
public class EmptyDataEvent { 
    public string eventName;
    public EmptyDataEvent(string name)
    {
        eventName = name;
    }
}

// TODO: Make generic data fields nullable to avoid this class
[Serializable]
public class EmptyPayloadData { 
    public string channel;
    public EmptyDataEvent data;
    public static EmptyPayloadData FromJSON(string jsonStr)
    {
        return JsonUtility.FromJson<EmptyPayloadData>(jsonStr); 
    }
    public EmptyPayloadData(string chann, EmptyDataEvent eventData) 
    {
        channel = chann;
        data = eventData;
    }
    public EmptyPayloadData(string chann, string eventName) 
    {
        channel = chann;
        data = new EmptyDataEvent(eventName);
    }
    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
}