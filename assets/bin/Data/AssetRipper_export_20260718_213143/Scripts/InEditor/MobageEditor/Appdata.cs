using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class Appdata
	{
		public delegate void deleteEntriesForKeys_onCompleteCallback(SimpleAPIStatus status, Error error, List<string> deletedKeys);

		public delegate void getEntriesForKeys_onCompleteCallback(SimpleAPIStatus status, Error error, List<string> keys, List<string> values);

		public delegate void updateEntries_onCompleteCallback(SimpleAPIStatus status, Error error, List<string> updatedKeys);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		public static void deleteEntriesForKeys(List<string> theKeys, deleteEntriesForKeys_onCompleteCallback completeCb)
		{
			Debug.Log("App Data: Calling US Delete");
			Action<SimpleAPIStatus, Error, List<string>> finalCallback = delegate(SimpleAPIStatus status, Error error, List<string> result)
			{
				completeCb(status, error, result);
			};
			if (theKeys == null || theKeys.Count < 1)
			{
				Debug.Log("App Data (Delete): Called with no data...");
				finalCallback(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "AppData Called With No Data",
					domain = "com.mobage.error.api",
					code = 400
				}, null);
				return;
			}
			string value = string.Join(",", theKeys.ToArray());
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "opensocial/appdata/@me/@self/@app";
			request.HTTPMethod = "DELETE";
			request.QueryString = new Dictionary<string, object> { { "fields", value } };
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (error == null)
				{
					Debug.Log("App Data (Delete): Successfully deleted appdata from server");
					finalCallback(SimpleAPIStatus.Success, null, theKeys);
				}
				else
				{
					Debug.Log("App Data (Delete): Deleting failed! " + JsonMapper.ToJson(error));
					finalCallback(SimpleAPIStatus.Error, error, null);
				}
			});
		}

		public static void getEntriesForKeys(List<string> theKeys, getEntriesForKeys_onCompleteCallback completeCb)
		{
			Action<SimpleAPIStatus, Error, List<string>, List<string>> finalCallback = delegate(SimpleAPIStatus status, Error error, List<string> keys, List<string> values)
			{
				completeCb(status, error, keys, values);
			};
			if (!string.IsNullOrEmpty(NativeAPI.Instance.UserId))
			{
				string currentUserId = NativeAPI.Instance.UserId;
				string value = string.Join(",", theKeys.ToArray());
				MobageRequest request = MobageRequest.Request;
				request.APIMethod = "opensocial/appdata/@me/@self/@app";
				request.HTTPMethod = "GET";
				if (theKeys.Count > 0)
				{
					request.QueryString = new Dictionary<string, object> { { "fields", value } };
				}
				request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
				{
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					if (error == null)
					{
						IDictionary dictionary = data["entry"][currentUserId].EnsureDictionary();
						foreach (DictionaryEntry item in dictionary)
						{
							list.Add(item.Key.ToString());
							list2.Add(item.Value.ToString());
						}
						finalCallback(SimpleAPIStatus.Success, null, list, list2);
					}
					else
					{
						Debug.Log("App Data (GET): Failed: " + error);
						finalCallback(SimpleAPIStatus.Error, error, null, null);
					}
				});
			}
			else
			{
				finalCallback(SimpleAPIStatus.Error, new Error
				{
					domain = "com.mobage.error.api",
					code = 20001
				}, null, null);
			}
		}

		public static void updateEntries(List<string> theKeys, List<string> theValues, updateEntries_onCompleteCallback completeCb)
		{
			Action<SimpleAPIStatus, Error, List<string>> finalCallback = delegate(SimpleAPIStatus status, Error error, List<string> result)
			{
				completeCb(status, error, result);
			};
			if (theKeys.Count < 1 || theValues.Count < 1)
			{
				Debug.Log("App Data (Persist): Called with no data...");
				finalCallback(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "AppData updateEntries Called With No Data",
					domain = "com.mobage.error.api",
					code = 400
				}, null);
				return;
			}
			if (theKeys.Count != theValues.Count)
			{
				Debug.Log("App Data (Persist): Called with mismatched amounts of data...");
				finalCallback(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "AppData updateEntries called with mismatched number of inputs.",
					domain = "com.mobage.error.api",
					code = 400
				}, null);
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int num = 0;
			foreach (string theKey in theKeys)
			{
				if (theKey.Contains(" "))
				{
					Debug.Log("Appdata Key may not contain spaces...");
					finalCallback(SimpleAPIStatus.Error, new Error
					{
						domain = "com.mobage.error.api",
						code = 20003
					}, null);
					return;
				}
				string value = theValues[num];
				num++;
				dictionary[theKey] = value;
			}
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "opensocial/appdata/@me/@self/@app";
			request.HTTPMethod = "POST";
			request.QueryString = new Dictionary<string, object> { 
			{
				"fields",
				string.Join(",", theKeys.ToArray())
			} };
			request.RequestType = RequestType.Json;
			request.PostBody = dictionary;
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (error == null)
				{
					finalCallback(SimpleAPIStatus.Success, null, theKeys);
				}
				else
				{
					Debug.Log("App Data (Persist): Persisting failed: " + error);
					finalCallback(SimpleAPIStatus.Error, error, null);
				}
			});
		}
	}
}
