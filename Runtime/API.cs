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
        public static ApiClient apiClient;

        // Environment
        public static string Env
        {
            get => apiClient.BaseUrl;
            set => apiClient.BaseUrl = value;
        }

        // Developer Headers
        public static string ApiKey
        {
            get => apiClient.headers.ContainsKey("X-Api-Key") ? apiClient.headers["X-Api-Key"] : null;
            set => apiClient.headers["X-Api-Key"] = value;
        }

        // User Headers
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

        // Request Login Payload from server
        public static async UniTask<LoginPayload> RequestLogin(string walletAddr, CancellationToken token = default)
        {
            var result = await apiClient.Get<LoginPayload>($"/login?walletAddress={walletAddr}");
            return result.body;
        }

        // Login or create new user using signed payload
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
                OnError?.Invoke($"Login returned error {result.errorMsg}");
                return result.body;
            }
        }

        // Create new user using signed payload
        public static async UniTask<LoginResponse> CreateUser(LoginRequest loginRequest, CancellationToken token = default)
        {
            var result = await apiClient.Post<LoginResponse>($"/users", loginRequest);
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"CreateUser returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Send an email to the user to verify them with String
        public static async UniTask<HttpResponse> RequestEmailAuth(string emailAddr, string userId, CancellationToken token = default)
        {
            var result = await apiClient.Get($"/users/{userId}/verify-email?email={emailAddr}");
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"RequestEmailAuth returned error {result.errorMsg}");
            }
            return result;
        }

        // Log the user out of the String service
        public static async UniTask<HttpResponse> Logout(CancellationToken token = default)
        {
            HttpResponse result = await apiClient.Post(path: $"/login/logout", body: null);
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"Logout returned error {result.errorMsg}");
            }
            return result;
        }

        // Update the user's name with the String service
        public static async UniTask<User> SetUserName(UserNameRequest userNameRequest, string userId, CancellationToken token = default)
        {
            var result = await apiClient.Patch<User>($"/users/{userId}", userNameRequest);
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"SetUserName returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Check the user's verification status with the String service
        public static async UniTask<UserStatusResponse> GetUserStatus(string userId, CancellationToken token = default)
        {
            var result = await apiClient.Get<UserStatusResponse>($"/users/{userId}/status");
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"GetUserStatus returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Retrieve a real-time quote for a desired transaction
        public static async UniTask<Quote> Quote(TransactionRequest quoteRequest, CancellationToken token = default)
        {
            var result = await apiClient.Post<Quote>($"/quotes", quoteRequest);
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"Quote returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Execute the quote payload
        public static async UniTask<TransactionResponse> Transact(ExecutionRequest transactionRequest, CancellationToken token = default)
        {
            // If using a new payment method, check if card info is valid
            if (transactionRequest.paymentInfo.cardId == "" && 
            (!WebEventManager.CardValid || WebEventManager.CardToken == ""))
            {
                OnWarning?.Invoke("WARNING: Card Info is invalid or not provided yet");
            }
            var result = await apiClient.Post<TransactionResponse>($"/transactions", transactionRequest);
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"Transact returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Get users saved card info
        public static async UniTask<CardInstrument[]> GetCards(CancellationToken token = default)
        {
            var result = await apiClient.Get<CardInstrument[]>($"/cards");
            if (!result.IsSuccess)
            {
                OnError?.Invoke($"GetCards returned error {result.errorMsg}");
            }
            return result.body;
        }

        // Check if payment info is valid
        public static bool ReadyForPayment()
        {
            return (WebEventManager.CardToken != "" && WebEventManager.CardValid);
        }

        // Get secure payment token
        public static string GetPaymentToken()
        {
            return WebEventManager.CardToken;
        }

        // Error handling
        public static event Action<string> OnError;
        public static event Action<string> OnWarning;

    }

}

