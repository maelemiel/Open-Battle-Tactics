using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class Transaction
	{
		public string uid;

		public string transactionId;

		public List<BillingItem> items;

		public string comment;

		public TransactionState transactionState;

		public string published;

		public string updated;

		public string id
		{
			get
			{
				return transactionId;
			}
			set
			{
				transactionId = value;
			}
		}

		public string state
		{
			get
			{
				return transactionState.ToString();
			}
			set
			{
				transactionState = (TransactionState)(int)Enum.Parse(typeof(TransactionState), value, true);
			}
		}

		public static void Create(BillingItem billingItem, string comment, Action<SimpleAPIStatus, Error, string> completeCB)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			mobageRequest.APIMethod = "bank/debit/@app";
			mobageRequest.HTTPMethod = "POST";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["sku"] = billingItem.item.itemId;
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["item"] = dictionary;
			dictionary2["quantity"] = billingItem.quantity;
			List<object> list = new List<object>();
			list.Add(dictionary2);
			List<object> value = list;
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3["items"] = value;
			dictionary3["comment"] = comment;
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					dictionary3["os"] = "android";
				}
				else
				{
					dictionary3["os"] = "ios";
				}
			}
			mobageRequest.RequestType = RequestType.Json;
			mobageRequest.PostBody = dictionary3;
			mobageRequest.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				Debug.Log("MBBankDebit create: err=" + err);
				err = getErrorObj(err, data, (int)status);
				if (err == null)
				{
					Transaction transaction = JsonMapper.ToObject<Transaction>(data.ToJson());
					completeCB(SimpleAPIStatus.Success, null, transaction.transactionId);
				}
				else
				{
					completeCB(SimpleAPIStatus.Error, err, null);
				}
			});
		}

		public static void Cancel(string transactionId, ItemType type, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			putTransaction(transactionId, TransactionState.Canceled, type, successCB, errorCB);
		}

		public static void Authorize(string transactionId, ItemType type, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			putTransaction(transactionId, TransactionState.Authorized, type, successCB, errorCB);
		}

		private static Error getErrorObj(Error error, JsonData data, int status)
		{
			if (error != null)
			{
				return error;
			}
			return null;
		}

		private static void putTransaction(string transactionId, TransactionState state, ItemType type, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			string text = stringForState(state);
			if (string.IsNullOrEmpty(text) || state == TransactionState.New)
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 40001;
				Error error2 = error;
				errorCB(error2);
			}
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			string aPIMethod = ((type != ItemType.Debit) ? string.Format("bank/purchase/@app/{0}?state={1}", transactionId, text) : string.Format("bank/debit/@app/{0}?state={1}", transactionId, text));
			mobageRequest.APIMethod = aPIMethod;
			mobageRequest.HTTPMethod = "PUT";
			sendDebit(mobageRequest, state, successCB, errorCB);
		}

		private static string stringForState(TransactionState state)
		{
			switch (state)
			{
			case TransactionState.Authorized:
				return "authorized";
			case TransactionState.Canceled:
				return "canceled";
			case TransactionState.Closed:
				return "closed";
			case TransactionState.New:
				return "new";
			case TransactionState.Open:
				return "open";
			default:
				return string.Empty;
			}
		}

		private static void sendDebit(MobageRequest request, TransactionState state, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				err = getErrorObj(err, data, (int)status);
				if (err == null)
				{
					string text = (string)data["id"];
					string text2 = (string)data["state"];
					TransactionState transactionState = stateFromString(text2);
					successCB(text, transactionState);
					return;
				}
				throw new NotImplementedException();
			});
		}

		private static TransactionState stateFromString(string state)
		{
			switch (state)
			{
			case "authorized":
				return TransactionState.Authorized;
			case "canceled":
				return TransactionState.Canceled;
			case "closed":
				return TransactionState.Closed;
			case "new":
				return TransactionState.New;
			case "open":
				return TransactionState.Open;
			default:
				return TransactionState.Invalid;
			}
		}

		public static void Close(string transactionId, ItemType type, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			putTransaction(transactionId, TransactionState.Closed, type, successCB, errorCB);
		}

		public static void Open(string transactionId, ItemType type, TransactionStateBlock successCB, OnErrorBlock errorCB)
		{
			putTransaction(transactionId, TransactionState.Open, type, successCB, errorCB);
		}
	}
}
