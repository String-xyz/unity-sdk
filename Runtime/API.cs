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
            var result = await apiClient.Get<LoginPayload>($"/login?walletAddress={walletAddr}");
            return result.body;
        }

        public static async UniTask<LoginResponse> Login(LoginRequest loginRequest, bool bypassDeviceCheck = false, CancellationToken token = default)
        {
            string bypass = "false";
            if (bypassDeviceCheck) 
            {
                bypass = "true";
            }
            else
            {
                // Update fingerprint data
                await Util.WaitUntil(() => WebEventManager.FingerprintAvailable());
                loginRequest.fingerprint.visitorId = WebEventManager.FingerprintVisitorId;
                loginRequest.fingerprint.requestId = WebEventManager.FingerprintRequestId;
            }
            var result = await apiClient.Post<LoginResponse>($"/login/sign?bypassDevice={bypass}", loginRequest);
            if (result.status == 200)
            {
                return result.body;
            } 
            else if (result.status == 400)
            {
                var createResultBody = await CreateUser(loginRequest);
                return createResultBody;
            }
            else
            {
                Debug.Log($"Login returned error {result.errorMsg}");
                return result.body;
            }
        }

        public static async UniTask<LoginResponse> CreateUser(LoginRequest loginRequest, CancellationToken token = default)
        {
            var result = await apiClient.Post<LoginResponse>($"/users", loginRequest);
            if (!result.IsSuccess)
            {
                Debug.Log($"CreateUser returned error {result.errorMsg}");
            }
            return result.body;
        }

        public static async UniTask<HttpResponse> RequestEmailAuth(string emailAddr, string userId, CancellationToken token = default)
        {
            var result = await apiClient.Get($"/users/{userId}/verify-email?email={emailAddr}");
            if (!result.IsSuccess)
            {
                Debug.Log($"RequestEmailAuth returned error {result.errorMsg}");
            }
            return result;
        }

        public static async UniTask<HttpResponse> Logout(CancellationToken token = default)
        {
            var result = await apiClient.Post($"/login/logout");
            if (!result.IsSuccess)
            {
                Debug.Log($"Logout returned error {result.errorMsg}");
            }
            return result;
        }

        public static async UniTask<User> SetUserName(UserNameRequest userNameRequest, string userId, CancellationToken token = default)
        {
            var result = await apiClient.Patch<User>($"/users/{userId}", userNameRequest);
            if (!result.IsSuccess)
            {
                Debug.Log($"SetUserName returned error {result.errorMsg}");
            }
            return result.body;
        }

        public static async UniTask<UserStatusResponse> GetUserStatus(string userId, CancellationToken token = default)
        {
            var result = await apiClient.Get<UserStatusResponse>($"/users/{userId}/status");
            if (!result.IsSuccess)
            {
                Debug.Log($"GetUserStatus returned error {result.errorMsg}");
            }
            return result.body;
        }

        public static async UniTask<Quote> Quote(TransactionRequest quoteRequest, CancellationToken token = default)
        {
            var result = await apiClient.Post<Quote>($"/quotes", quoteRequest);
            if (!result.IsSuccess)
            {
                Debug.Log($"Quote returned error {result.errorMsg}");
            }
            return result.body;
        }

        public static async UniTask<TransactionResponse> Transact(ExecutionRequest transactionRequest, CancellationToken token = default)
        {
            // If using a new payment method, check if card info is valid
            if (transactionRequest.paymentInfo.cardId == "" && 
            (!WebEventManager.CardValid || WebEventManager.CardToken == ""))
            {
                Debug.Log("WARNING: Card Info is invalid or not provided yet");
            }
            var result = await apiClient.Post<TransactionResponse>($"/transactions", transactionRequest);
            if (!result.IsSuccess)
            {
                Debug.Log($"Transact returned error {result.errorMsg}");
            }
            return result.body;
        }

        public static async UniTask<CustomerInstrument[]> GetCards(CancellationToken token = default)
        {
            var result = await apiClient.Get<CustomerInstrument[]>($"/cards");
            if (!result.IsSuccess)
            {
                Debug.Log($"GetCards returned error {result.errorMsg}");
            }
            return result.body;
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

