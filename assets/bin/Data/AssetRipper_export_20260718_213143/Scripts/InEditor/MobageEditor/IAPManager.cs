using System;

namespace MobageEditor
{
	public class IAPManager
	{
		public static void PurchaseCoin(string sku, string price, string value, string currency, Action<JsonData> callback)
		{
			callback(JsonMapper.ToObject("[\"401\", {}]"));
		}
	}
}
