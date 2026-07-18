using System;
using System.Collections.Generic;
using LCD.Internal.Util;
using LCD.Internal.Web;
using LCD.User;

namespace LCD.Internal.Impl
{
	internal class LoadAccountCallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "LoadAccountCallbackImpl";

		private LCD.User.User.LoadAccountCallback callback;

		internal LoadAccountCallbackImpl(LCD.User.User.LoadAccountCallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			if (message != null)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
				long newUserId = Convert.ToInt64(dictionary["newUserId"]);
				long oldUserId = Convert.ToInt64(dictionary["oldUserId"]);
				if (callback != null)
				{
					callback(newUserId, oldUserId, null);
				}
			}
			else
			{
				LCDErrorImpl error = new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR, -1, "Invalid JSON");
				if (callback != null)
				{
					callback(-1L, -1L, error);
				}
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "onFailure : " + message);
			LCDErrorImpl error = LCDErrorImpl.CreateLCDError(message);
			if (callback != null)
			{
				callback(-1L, -1L, error);
			}
		}
	}
}
