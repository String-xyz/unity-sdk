using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace StringSDK
{
	public class HttpClient
	{
		public static async UniTask<HttpResponse> Get(string url, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Get, url, headers, token: token);
		}

		public static async UniTask<HttpResponse> Put(string url, object body, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Put, url, headers, body, token: token);
		}

		public static async UniTask<HttpResponse> Post(string url, object body, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Post, url, headers, body, token: token);
		}

		public static async UniTask<HttpResponse> Delete(string url, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Delete, url, headers, token: token);
		}

		public static async UniTask<HttpResponse> Request(HttpMethod method, string url, Dictionary<string, string> headers = null, object body = null, CancellationToken token = default)
		{
			HttpResponse response = new HttpResponse();

			var requestJson = body == null ? string.Empty : JsonUtility.ToJson(body).Replace("[]", "null");
			using (var uwr =
				method == HttpMethod.Get ? UnityWebRequest.Get(url) :
				method == HttpMethod.Delete ? UnityWebRequest.Delete(url) :
				UnityWebRequest.Put(url, requestJson)
			)
			{
				if (method == HttpMethod.Post)
				{
					uwr.method = "POST";
				}
				else if (method == HttpMethod.Patch)
				{
					uwr.method = "PATCH";
				}
				uwr.SetRequestHeader("Content-Type", "application/json");
				uwr.SetRequestHeader("Accept", "application/json");

				if (headers != null)
				{
					foreach (var entry in headers)
					{
						uwr.SetRequestHeader(entry.Key, entry.Value);
					}
				}

				try
				{
					// OP will only have data if response is 200
					var op = await uwr.SendWebRequest().WithCancellation(token);
					token.ThrowIfCancellationRequested();
					response.status = op.responseCode;
					response.body = op.downloadHandler?.text;
					response.headers = op.GetResponseHeaders();
				}
				catch (OperationCanceledException)
				{
					UnityEngine.Debug.LogWarning($"Http Request canceled. method={method} url={url}");
				}
				catch (UnityWebRequestException e)
				{
					// Continue if we received a response other than 200
					response.status = e.ResponseCode;
					response.errorMsg = e.Message;
				}

			}
			return response;
		}
	}
}
