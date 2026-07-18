using System;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class BankInventory
	{
		public delegate void getItemForId_onCompleteCallback(SimpleAPIStatus status, Error error, ItemData itemData);

		public delegate void getASCItems_onCompleteCallback(SimpleAPIStatus status, Error error, List<ItemData> items);

		public static void getItemForId(string itemId, getItemForId_onCompleteCallback onComplete)
		{
			GetItemForStringIds(itemId, delegate(SimpleAPIStatus status, Error error, List<ItemData> items)
			{
				onComplete(status, error, (items == null || items.Count <= 0) ? null : items[0]);
			});
		}

		public static void getASCItems(getASCItems_onCompleteCallback onComplete)
		{
			GetASCItems(delegate(SimpleAPIStatus status, Error error, List<ItemData> items)
			{
				onComplete(status, error, items);
			});
		}

		public static void GetItemForStringIds(string itemId, Action<SimpleAPIStatus, Error, List<ItemData>> completeCB)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			mobageRequest.APIMethod = "bank/inventory/@app/" + itemId;
			mobageRequest.HTTPMethod = "GET";
			mobageRequest.QueryString = new Dictionary<string, object> { { "fields", "id,name,value,description,imageUrl,iapEnabled" } };
			mobageRequest.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (data != null && err == null)
				{
					JsonData jsonData = data["entry"];
					if (jsonData != null && jsonData.Count > 0)
					{
						List<ItemData> list = new List<ItemData>();
						List<string> list2 = new List<string>();
						for (int i = 0; i < jsonData.Count; i++)
						{
							JsonData jsonData2 = jsonData[i];
							ItemData itemData = JsonMapper.ToObject<ItemData>(jsonData2.ToJson());
							if (itemData.itemForCash)
							{
								list2.Add(itemData.itemId);
							}
							list.Add(itemData);
						}
						if (list2.Count > 0)
						{
							throw new NotImplementedException();
						}
						completeCB(SimpleAPIStatus.Success, null, list);
					}
				}
				else
				{
					completeCB(SimpleAPIStatus.Error, err, null);
				}
			});
		}

		public static void GetASCItems(Action<SimpleAPIStatus, Error, List<ItemData>> completeCB)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			mobageRequest.APIMethod = "bank/items";
			mobageRequest.HTTPMethod = "GET";
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					mobageRequest.QueryString = new Dictionary<string, object>
					{
						{ "os", "Android" },
						{ "type", "credit" }
					};
				}
				else
				{
					mobageRequest.QueryString = new Dictionary<string, object>
					{
						{ "os", "ios" },
						{ "type", "credit" }
					};
				}
			}
			mobageRequest.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (data != null && err == null)
				{
					JsonData jsonData = data["items"];
					if (jsonData != null && jsonData.Count > 0)
					{
						List<ItemData> list = new List<ItemData>();
						List<string> list2 = new List<string>();
						for (int i = 0; i < jsonData.Count; i++)
						{
							JsonData jsonData2 = jsonData[i];
							ItemData itemData = new ItemData
							{
								itemId = (string)jsonData2["sku"],
								price = int.Parse((string)jsonData2["value"]),
								currency = (string)jsonData2["currency"],
								name = (string)jsonData2["display_name"],
								description = (string)jsonData2["display_message"],
								originPrice = double.Parse((string)jsonData2["origin_price"]),
								originCurrencyLabel = (string)jsonData2["origin_currency"],
								originPriceLabel = (string)jsonData2["display_origin_price"]
							};
							string text = (string)jsonData2["image_url"];
							itemData.imageUrl = ((text == null || text.Length <= 0) ? string.Empty : text);
							if (itemData.itemForCash)
							{
								list2.Add(itemData.itemId);
							}
							list.Add(itemData);
						}
						if (list2.Count > 0)
						{
							throw new NotImplementedException();
						}
						completeCB(SimpleAPIStatus.Success, null, list);
					}
				}
				else
				{
					completeCB(SimpleAPIStatus.Error, err, null);
				}
			});
		}
	}
}
