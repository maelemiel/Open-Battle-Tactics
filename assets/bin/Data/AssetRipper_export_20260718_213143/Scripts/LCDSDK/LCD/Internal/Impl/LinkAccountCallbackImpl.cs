using System.Collections.Generic;
using LCD.Internal.Util;
using LCD.Internal.Web;
using LCD.User;

namespace LCD.Internal.Impl
{
	internal class LinkAccountCallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "LinkAccountCallbackImpl";

		private LCD.User.User.LinkAccountCallback callback;

		internal LinkAccountCallbackImpl(LCD.User.User.LinkAccountCallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			object value = null;
			dictionary.TryGetValue("requestId", out value);
			if (callback != null)
			{
				callback(null);
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "onFailure : " + message);
			LCDErrorImpl error = LCDErrorImpl.CreateLCDError(message);
			if (callback != null)
			{
				callback(error);
			}
		}
	}
}
