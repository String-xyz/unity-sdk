using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace StringSDK
{
    public static class StringXYZ
    {
        // API Client
        static ApiClient apiClient;

        // Environment
        public static string Env
        {
            get => apiClient.BaseUrl;
            set => apiClient.BaseUrl = value;
        }

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
            apiClient = new ApiClient();
            Env = Config.ENV_DEFAULT;
        }

        // Methods
        public static async UniTask<LoginPayload> RequestLogin(string walletAddr, CancellationToken token = default)
        {
            return await apiClient.Get<LoginPayload>($"/login?walletAddress={walletAddr}");
        }

        public static async UniTask<LoginResponse> Login(LoginRequest loginRequest, bool bypassDeviceCheck = false, CancellationToken token = default)
        {
            if (!WebEventManager.FingerprintAvailable())
            {
                Debug.Log("WARNING: Fingerprint Data Not Yet Available");
            }
            try
            {
                string bypass = "false";
                if (bypassDeviceCheck) bypass = "true";
                return await apiClient.Post<LoginResponse>($"/login/sign?bypassDevice={bypass}", loginRequest);
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
            return await apiClient.Patch<User>($"/users/{userId}", userNameRequest);
        }

        public static async UniTask<UserStatusResponse> GetUserStatus(string userId, CancellationToken token = default)
        {
            return await apiClient.Get<UserStatusResponse>($"/users/{userId}/status");
        }

        public static async UniTask<Quote> Quote(TransactionRequest quoteRequest, CancellationToken token = default)
        {
            return await apiClient.Post<Quote>($"/quotes", quoteRequest);
        }

        public static async UniTask<TransactionResponse> Transact(ExecutionRequest transactionRequest, CancellationToken token = default)
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

