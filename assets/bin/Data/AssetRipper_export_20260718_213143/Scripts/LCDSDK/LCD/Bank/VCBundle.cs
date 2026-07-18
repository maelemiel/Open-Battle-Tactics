using System.Collections.Generic;
using LCD.Internal.Impl;
using LCD.Internal.Web;

namespace LCD.Bank
{
	public class VCBundle
	{
		public delegate void VCBundleCallback(List<VCBundle> bundles, LCDError error);

		public readonly string sku;

		public readonly string title;

		public readonly double price;

		public readonly string priceCode;

		public readonly string displayPrice;

		public readonly string detail;

		public readonly double usdPrice;

		public readonly string currency;

		public readonly int value;

		internal VCBundle(string sku, string title, double price, string priceCode, string displayPrice, string detail, double usdPrice, string currency, int value)
		{
			this.sku = sku;
			this.title = title;
			this.price = price;
			this.priceCode = priceCode;
			this.displayPrice = displayPrice;
			this.detail = detail;
			this.usdPrice = usdPrice;
			this.currency = currency;
			this.value = value;
		}

		public static void GetAsList(VCBundleCallback callback)
		{
			SDKWebUtil.Execute("GET", "/bank/inventory", null, null, new VCBundleWebCallbackImpl(callback));
		}

		public void Purchase(Wallet.WalletCallback callback)
		{
			Dictionary<string, object> param = ToJson();
			SDKWebUtil.openSDKWebView("purchase", param, new WalletCallbackImpl(callback), false);
		}

		private Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("sku", sku);
			dictionary.Add("title", title);
			dictionary.Add("price", price);
			dictionary.Add("priceCode", priceCode);
			dictionary.Add("detail", detail);
			dictionary.Add("usdPrice", usdPrice);
			dictionary.Add("currency", currency);
			dictionary.Add("value", value);
			return dictionary;
		}
	}
}
