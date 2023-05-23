using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace StringSDK
{
    // JSON Serializations
    [Serializable]
    public class TransactionRequest
    {
        public string userAddress;
        public string assetName;
        public int chainId; // todo: make this unsigned in both the backend and this sdk
        public string contractAddress;
        public string contractFunction;
        public string contractReturn;
        public string[] contractParameters;
        public string txValue;
        public string gasLimit;

        public TransactionRequest(string userAddress, string assetName, int chainId, string contractAddress, string contractFunction, string contractReturn, string[] contractParameters, string txValue, string gasLimit)
        {
            this.userAddress = userAddress;
            this.assetName = assetName;
            this.chainId = chainId;
            this.contractAddress = contractAddress;
            this.contractFunction = contractFunction;
            this.contractReturn = contractReturn;
            this.contractParameters = contractParameters;
            this.txValue = txValue;
            this.gasLimit = gasLimit;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
    [Serializable]
    public class Estimate
    {
        public long timestamp; // todo: make this unsigned in both the backend and this sdk
        public string baseUSD;
        public string gasUSD;
        public string tokenUSD;
        public string serviceUSD;
        public string totalUSD;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
    [Serializable]
    public class Quote
    {
        public TransactionRequest request;
        public Estimate estimate;
        public string signature;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class PaymentInfo
    {
        public string cardToken;
        public string cardId;
        public string cvv;

        public bool saveCard;

        public PaymentInfo(string cardToken = "", string cardId = "", string cvv = "", bool saveCard = false)
        {
            this.cardToken = cardToken;
            this.cardId = cardId;
            this.cvv = cvv;
            this.saveCard = saveCard;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class ExecutionRequest
    {
        public Quote quote;
        public PaymentInfo paymentInfo;

        public ExecutionRequest(Quote quote, PaymentInfo paymentInfo)
        {
            this.quote = quote;
            this.paymentInfo = paymentInfo;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class TransactionResponse
    {
        public string txId;
        public string txUrl;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class LoginPayload
    {
        public string nonce;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class LoginRequest
    {
        public string nonce;
        public string signature;
        public Fingerprint fingerprint;

        public LoginRequest(string nonce, string signature)
        {
            this.nonce = nonce;
            this.signature = signature;
            fingerprint = new Fingerprint(WebEventManager.FingerprintVisitorId, WebEventManager.FingerprintRequestId);
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class Fingerprint
    {
        public string visitorId;
        public string requestId;

        public Fingerprint(string visitorId, string requestId)
        {
            this.visitorId = visitorId;
            this.requestId = requestId;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class LoginResponse
    {
        public AuthToken authToken;
        public User user;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class AuthToken
    {
        public string expAt;
        public string issuedAt;
        public string token;
        public RefreshToken refreshToken;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class RefreshToken
    {
        public string token;
        public string expAt;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class User
    {
        public string id;
        public string createdAt;
        public string updatedAt;
        public string type;
        public string status;
        public string[] tags;
        public string firstName;
        public string middleName;
        public string lastName;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class UserNameRequest
    {
        public string walletAddress;
        public string firstName;
        public string middleName;
        public string lastName;

        public UserNameRequest(string walletAddress, string firstName, string middleName, string lastName)
        {
            this.walletAddress = walletAddress;
            this.firstName = firstName;
            this.middleName = middleName;
            this.lastName = lastName;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class UserStatusResponse
    {
        public string status;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class CustomerInstrument
    {
        public string id;
        public string type;
        public UInt64 expiry_month;
        public UInt64 expiry_year;
        public string scheme;
        public string last4;
        public string fingerprint;
        public string bin;
        public string card_type;
        public string card_category;
        public string issuer;
        public string issuer_country;
        public string product_id;
        public string product_type;
        public BillingAddress billing_address;
        public Phone phone;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class BillingAddress
    {
        public string address_line1;
        public string address_line2;
        public string city;
        public string state;
        public string zip;
        public string country;
    }

    [Serializable]
    public class Phone
    {
        public string country_code;
        public string number;
    }
}