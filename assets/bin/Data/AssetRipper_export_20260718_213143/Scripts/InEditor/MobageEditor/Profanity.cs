using System;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class Profanity
	{
		public delegate void checkProfanity_onCompleteCallback(SimpleAPIStatus status, Error error, bool textIsValid);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		public static void checkProfanity(string text, checkProfanity_onCompleteCallback completeCb)
		{
			Action<Error, bool> finalCallback = delegate(Error error, bool isValid)
			{
				completeCb(SimpleAPIStatus.Success, error, isValid);
			};
			if (string.IsNullOrEmpty(text))
			{
				finalCallback(new Error
				{
					domain = "com.mobage.error.api",
					code = 20002
				}, false);
				return;
			}
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "opensocial/ngword";
			request.HTTPMethod = "GET";
			request.QueryString = new Dictionary<string, object> { { "text", text } };
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				bool arg = false;
				if (error == null)
				{
					arg = (bool)data["valid"];
				}
				finalCallback(error, arg);
			});
		}
	}
}
