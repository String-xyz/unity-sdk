using System.Collections.Generic;
using UnityEngine;

namespace StringSDK
{
	public struct HttpResponse {
		public long status;
		public string body;
		public bool IsSuccess => 200 <= status && status <= 299;
		public Dictionary<string, string> headers;
		public string errorMsg;

		public override string ToString() => JsonUtility.ToJson(this);
	}

	public struct HttpResponseObject<T> {
		public long status;
		public T body;
		public bool IsSuccess => 200 <= status && status <= 299;
		public Dictionary<string, string> headers;
		public string errorMsg;

		public override string ToString() => JsonUtility.ToJson(this);
	}
}
