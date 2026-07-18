using System;
using System.Collections.Generic;
using LCD.Bank;
using LCD.Internal.Util;
using LCD.Internal.Web;

namespace LCD.Internal.Impl
{
	internal class VCBundleWebCallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "VCBundleWebCallbackImpl";

		private VCBundle.VCBundleCallback callback;

		internal VCBundleWebCallbackImpl(VCBundle.VCBundleCallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			LCDErrorImpl error = new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR.ToString(), 500, "Invalid json, json null");
			if (message == null)
			{
				if (callback != null)
				{
					callback(null, error);
				}
				return;
			}
			List<VCBundle> list = new List<VCBundle>();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary == null)
			{
				if (callback != null)
				{
					callback(null, error);
				}
				return;
			}
			List<object> list2 = (List<object>)dictionary["items"];
			foreach (Dictionary<string, object> item2 in list2)
			{
				string sku = (string)item2["sku"];
				string title = string.Empty;
				if (item2.ContainsKey("title"))
				{
					title = (string)item2["title"];
				}
				double price = -1.0;
				if (item2.ContainsKey("price"))
				{
					price = Convert.ToDouble(item2["price"]);
				}
				string priceCode = string.Empty;
				if (item2.ContainsKey("priceCode"))
				{
					priceCode = (string)item2["priceCode"];
				}
				double num = Convert.ToDouble(item2["usdPrice"]);
				string displayPrice = "$" + num;
				if (item2.ContainsKey("displayPrice"))
				{
					displayPrice = (string)item2["displayPrice"];
				}
				string detail = string.Empty;
				if (item2.ContainsKey("detail"))
				{
					detail = (string)item2["detail"];
				}
				string currency = (string)item2["currency"];
				int value = Convert.ToInt32(item2["value"]);
				VCBundle item = new VCBundle(sku, title, price, priceCode, displayPrice, detail, num, currency, value);
				list.Add(item);
			}
			if (callback != null)
			{
				callback(list, null);
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "onFailure : " + message);
			LCDError error = LCDErrorImpl.CreateLCDError(message);
			if (callback != null)
			{
				callback(null, error);
			}
		}
	}
}
