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
    public class QuoteRequest
    {
        public string userAddress;
        public long chainID; // todo: make this unsigned in both the backend and this sdk
        public string contractAddress;
        public string contractFunction;
        public string contractReturn;
        public string[] contractParameters;
        public string txValue;
        public string gasLimit;

        public QuoteRequest(string userAddress, long chainID, string contractAddress, string contractFunction, string contractReturn, string[] contractParameters, string txValue, string gasLimit)
        {
            this.userAddress = userAddress;
            this.chainID = chainID;
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
    public class TransactionRequest
    {
        public string userAddress;
        public long chainID; // todo: make this unsigned in both the backend and this sdk
        public string contractAddress;
        public string contractFunction;
        public string contractReturn;
        public string[] contractParameters;
        public string txValue;
        public string gasLimit;
        public long timestamp; // todo: make this unsigned in both the backend and this sdk
        public double baseUSD;
        public double gasUSD;
        public double tokenUSD;
        public double serviceUSD;
        public double totalUSD;
        public string signature;
        public string cardToken;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class TransactionResponse
    {
        public string txID;
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

        public LoginRequest(string nonce, string signature, string visitorId, string requestId)
        {
            this.nonce = nonce;
            this.signature = signature;
            fingerprint = new Fingerprint(visitorId, requestId);
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
}