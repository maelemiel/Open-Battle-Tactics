using System;
using System.Collections.Generic;
using LCD.Bank;
using LCD.Internal.Util;
using LCD.Internal.Web;

namespace LCD.Internal.Impl
{
	internal class WalletCallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "WalletCallbackImpl";

		private Wallet.WalletCallback callback;

		internal WalletCallbackImpl(Wallet.WalletCallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			if (callback == null)
			{
				LCDSDKLog.Error(TAG, "WalletCallbackImpl.onFailure has a null callback", new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR, -1, "Null Callback"));
				return;
			}
			LCDErrorImpl error = new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR.ToString(), 500, "Invalid json, json null");
			if (message == null)
			{
				callback(null, error);
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			Wallet wallet = new Wallet(Convert.ToInt32(dictionary["balance"]), (string)dictionary["currency"]);
			if (callback != null)
			{
				callback(wallet, null);
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "onFailure : " + message);
			LCDErrorImpl error = LCDErrorImpl.CreateLCDError(message);
			if (callback != null)
			{
				callback(null, error);
			}
		}
	}
}
