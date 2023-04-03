using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace StringSDK
{
    public static class Constants
    {
        public const string api_base_url = "http://localhost:5555";
    }

    public static class StringXYZ
    {
        // API Client
        static ApiClient apiClient;

        // Headers
        public static string ApiKey
        {
            get => apiClient.headers.ContainsKey("X-Api-Key") ? apiClient.headers["X-Api-Key"] : null;
            set => apiClient.headers["X-Api-Key"] = value;
        }

        public static string Authorization
        {
            get => apiClient.headers.ContainsKey("Authorization") ? apiClient.headers["Authorization"] : null;
            set => apiClient.headers["Authorization"] = value;
        }

        // Constructor
        static StringXYZ()
        {
            apiClient = new ApiClient(Constants.api_base_url);
        }

        // Methods
        public static async UniTask<LoginPayload> RequestLogin(string walletAddr, CancellationToken token = default)
        {
            return await apiClient.Get<LoginPayload>($"/login?walletAddress={walletAddr}");
        }

        public static async UniTask<LoginResponse> Login(LoginRequest loginRequest, LoginOptions loginOptions = default, CancellationToken token = default)
        {
            if (!WebEventManager.FingerprintAvailable())
            {
                Debug.Log("WARNING: Fingerprint Data Not Yet Available");
            }
            try
            {
                var bypassDevice = loginOptions?.bypassDeviceCheck || false ? "?bypassDevice=true" : "";
                return await apiClient.Post<LoginResponse>($"/login/sign{bypassDevice}", loginRequest);
            }
            catch // (Exception e)
            {
                // Unsure how to check for 400 Bad response without parsing Exception string
                // Skipping for now
                // TODO: Parse exception for status code 400 before Creating user
                // If the status is not 400, throw error up the stack
                return await CreateUser(loginRequest);
            }
        }

        public static async UniTask<LoginResponse> CreateUser(LoginRequest loginRequest, CancellationToken token = default)
        {
            return await apiClient.Post<LoginResponse>($"/users", loginRequest);
        }

        public static async UniTask<HttpResponse> RequestEmailAuth(string emailAddr, string userId, CancellationToken token = default)
        {
            return await apiClient.Get($"/users/{userId}/verify-email?email={emailAddr}");
        }

        public static async UniTask<HttpResponse> Logout(CancellationToken token = default)
        {
            return await apiClient.Post<HttpResponse>($"/login/logout");
        }

        public static async UniTask<User> SetUserName(UserNameRequest userNameRequest, string userId, CancellationToken token = default)
        {
            return await apiClient.Put<User>($"/users/{userId}", userNameRequest);
        }

        public static async UniTask<UserStatusResponse> GetUserStatus(string userId, CancellationToken token = default)
        {
            return await apiClient.Put<UserStatusResponse>($"/users/{userId}/status");
        }

        public static async UniTask<TransactionRequest> Quote(QuoteRequest quoteRequest, CancellationToken token = default)
        {
            return await apiClient.Post<TransactionRequest>($"/quotes", quoteRequest);
        }

        public static async UniTask<TransactionResponse> Transact(TransactionRequest transactionRequest, CancellationToken token = default)
        {
            if (!WebEventManager.CardValid || WebEventManager.CardToken == "")
            {
                Debug.Log("WARNING: Card Info is invalid or not provided yet");
            }
            return await apiClient.Post<TransactionResponse>($"/transactions", transactionRequest);
        }

        // Card stuff
        public static bool ReadyForPayment()
        {
            return (WebEventManager.CardToken != "" && WebEventManager.CardValid);
        }

        public static string GetPaymentToken()
        {
            return WebEventManager.CardToken;
        }
    }

}

