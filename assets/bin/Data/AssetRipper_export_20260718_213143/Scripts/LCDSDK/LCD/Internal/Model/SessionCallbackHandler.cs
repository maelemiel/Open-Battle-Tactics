using System.Collections.Generic;
using LCD.Internal.Util;
using LCD.Internal.Web;

namespace LCD.Internal.Model
{
	internal class SessionCallbackHandler : SDKWebCallbackHandler
	{
		private static string TAG = "SessionCallback";

		private SDKWebCallbackHandler callback;

		internal SessionCallbackHandler(SDKWebCallbackHandler callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "SDKWebViewCallbackImpl onSuccess - message:'" + message + "'");
			if (message != null && message.Length > 0)
			{
				Credentials.setSessionResponse(message);
				SDKWebViewManagerInternal.sharedManager.eventHandler.OnSessionUpdate(message);
			}
			if (callback != null)
			{
				callback.onSuccess(message);
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "SDKWebViewCallbackImpl onFailure - message:'" + message + "'");
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			long num = (long)dictionary["code"];
			if (num == 401)
			{
				Credentials.accessToken = null;
			}
			SDKWebViewManagerInternal.sharedManager.eventHandler.OnSessionError(message);
			if (callback != null)
			{
				callback.onFailure(message);
			}
		}
	}
}
