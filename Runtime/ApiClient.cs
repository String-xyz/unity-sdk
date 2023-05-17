using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StringSDK
{
	/// <summary>
	/// Provides an interface to an http rest api
	///
	/// Handles authentication, signing requests and base url
	/// </summary>
	public class ApiClient
	{
		/// <summary>
		/// Create a json api client for a REST api
		/// </summary>
		/// <param name="baseUrl">base url for requests, e.g. https://api.example.com</param>
		/// <param name="basicAuth">plaintext string to be base64 encoded as HTTP basic auth</param>
		public ApiClient(string basicAuth = null)
		{
			this.BasicAuth = basicAuth;
		}

		/// <summary>
		/// accessToken authenticates us with a server using x-access-token
		/// </summary>
		public string accessToken;

		/// <summary>
		/// basicAuth authenticates us with a server using base64 encoded basic auth string
		/// </summary>
		public string BasicAuth
		{
			get => basicAuth;
			set => basicAuth = Base64Encode(value);
		}
		string basicAuth;


		/// <summary>
		/// headers set here are sent with every request
		/// </summary>
		/// <typeparam name="string">http header name, e.g. "Content-Type"</typeparam>
		/// <typeparam name="string">http header value, e.g. "application/json"</typeparam>
		/// <returns></returns>
		public Dictionary<string, string> headers = new Dictionary<string, string>();

		/// <summary>
		/// baseUrl defines the base url to use with each path
		/// </summary>
		internal string BaseUrl
		{
			get => baseUrl;
			set => baseUrl = value;
		}
		string baseUrl;

		/// <summary>
		/// Sends an HTTP GET request to the given path
		/// </summary>
		/// <param name="path">path to api resource, e.g. "/v1/tweets"</param>
		/// <param name="headers">dictionary of http headers</param>
		/// <param name="token">cancellation token</param>
		/// <typeparam name="T">A <see cref="System.Serializable" /> class to populate with data. </typeparam>
		/// <returns>T</returns>
		public async UniTask<HttpResponseObject<T>> Get<T>(string path, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request<T>(HttpMethod.Get, path, headers, token: token);
		}

		/// <summary>
		/// Sends an HTTP PUT request to the given path
		/// </summary>
		/// <param name="path">path to api resource, e.g. "/v1/tweets"</param>
		/// <param name="headers">dictionary of http headers</param>
		/// <param name="token">cancellation token</param>
		/// <typeparam name="T">A <see cref="System.Serializable" /> class to populate with data. </typeparam>
		/// <returns>T</returns>
		public async UniTask<HttpResponseObject<T>> Put<T>(string path, object body = null, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request<T>(HttpMethod.Put, path, headers, body, token: token);
		}

		/// <summary>
		/// Sends an HTTP POST request to the given path
		/// </summary>
		/// <param name="path">path to api resource, e.g. "/v1/tweets"</param>
		/// <param name="headers">dictionary of http headers</param>
		/// <param name="token">cancellation token</param>
		/// <typeparam name="T">A <see cref="System.Serializable" /> class to populate with data. </typeparam>
		/// <returns>T</returns>
		public async UniTask<HttpResponseObject<T>> Post<T>(string path, object body = null, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request<T>(HttpMethod.Post, path, headers, body, token: token);
		}

		/// <summary>
		/// Sends an HTTP DELETE request to the given path
		/// </summary>
		/// <param name="path">path to api resource, e.g. "/v1/tweets"</param>
		/// <param name="headers">dictionary of http headers</param>
		/// <param name="token">cancellation token</param>
		/// <typeparam name="T">A <see cref="System.Serializable" /> class to populate with data. </typeparam>
		/// <returns>T</returns>
		public async UniTask<HttpResponseObject<T>> Delete<T>(string path, object body = null, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request<T>(HttpMethod.Delete, path, headers, body, token: token);
		}

		/// <summary>
		/// Sends an HTTP PATCH request to the given path
		/// </summary>
		/// <param name="path">path to api resource, e.g. "/v1/tweets"</param>
		/// <param name="headers">dictionary of http headers</param>
		/// <param name="token">cancellation token</param>
		/// <typeparam name="T">A <see cref="System.Serializable" /> class to populate with data. </typeparam>
		/// <returns>T</returns>
		public async UniTask<HttpResponseObject<T>> Patch<T>(string path, object body = null, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request<T>(HttpMethod.Patch, path, headers, body, token: token);
		}

		// public async UniTask<Dictionary<string, float>> GetDictionary(string path, Dictionary<string, string> headers = null, CancellationToken token = default)
		// {
		// 	return await RequestDictionary(HttpMethod.Get, path, headers, token: token);
		// }

		// public async UniTask<float> GetFloat(string path, Dictionary<string, string> headers = null, CancellationToken token = default)
		// {
		// 	return await RequestFloat(HttpMethod.Get, path, headers, token: token);
		// }

		// public async UniTask<int> GetInt(string path, Dictionary<string, string> headers = null, CancellationToken token = default)
		// {
		// 	return await RequestInt(HttpMethod.Get, path, headers, token: token);
		// }

		public async UniTask<HttpResponse> Get(string path, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Get, path, headers, token: token);
		}

		public async UniTask<HttpResponse> Post(string path, object body, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Post, path, headers, body, token: token);
		}

		public async UniTask<HttpResponse> Put(string path, object body, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Put, path, headers, body, token: token);
		}

		public async UniTask<HttpResponse> Delete(string path, object body = null, Dictionary<string, string> headers = null, CancellationToken token = default)
		{
			return await Request(HttpMethod.Delete, path, headers, body, token: token);
		}

		async UniTask<HttpResponseObject<T>> Request<T>(HttpMethod method, string path, Dictionary<string, string> headers = null, object body = null, CancellationToken token = default)
		{
			HttpResponseObject<T> result = new HttpResponseObject<T>();
			var response = await Request(method, path, headers, body, token);
			result.headers = response.headers;
			result.status = response.status;
			result.errorMsg = response.errorMsg;

			if (response.IsSuccess)
			{
				// this is a workaround for https://answers.unity.com/questions/1123326/jsonutility-array-not-supported.html
				if (response.body.TrimStart()[0] == '[')
				{
					string newJson = "{ \"data\": " + response.body + "}";
					Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
					result.body = wrapper.data;
					return result;
				}

				result.body = JsonUtility.FromJson<T>(response.body);
			}
			return result;
		}

		async UniTask<HttpResponse> Request(HttpMethod method, string path, Dictionary<string, string> headers = null, object body = null, CancellationToken token = default)
		{
			if (headers == null) { headers = new Dictionary<string, string>(); }
			headers["Accept"] = "application/json";
			foreach (var entry in this.headers)
			{
				headers[entry.Key] = entry.Value;
			}

			var url = baseUrl + PrependPath(path);
			return await HttpClient.Request(method, url, headers, body, token);
		}

		string Base64Encode(string plainText)
		{
			if (plainText == null) { return null; }
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		string PrependPath(string path)
		{
			return path[0] == '/' ? path : $"/{path}";
		}

		[Serializable]
		private class Wrapper<T>
		{
			public T data;
		}
	}
}