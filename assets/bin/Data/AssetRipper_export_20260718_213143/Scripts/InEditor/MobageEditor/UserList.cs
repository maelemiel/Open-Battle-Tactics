using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class UserList
	{
		private User user;

		private string game;

		private string relation;

		private string uid;

		public UserList(User aUser, string userRelation)
		{
			user = aUser;
			relation = userRelation;
			uid = user.uid + ":" + relation;
		}

		public UserList(User aUser, string appId, string userRelation)
		{
			user = aUser;
			game = appId;
			relation = userRelation;
			uid = string.Format("{0}:{1}:{2}", user.uid, appId, relation);
		}

		public void Get(int count, uint offset, Action<SimpleAPIStatus, Error, List<User>, int, int> cb)
		{
			if (string.IsNullOrEmpty(relation) || (!(relation == "mutual") && !(relation == "enemies")))
			{
				return;
			}
			MobageRequestCallbackBlock callback = delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (err == null && data != null)
				{
					int num = (int)data["startIndex"];
					int num2 = (int)data["totalResults"];
					List<User> list = new List<User>();
					JsonData jsonData = data["entry"];
					if (jsonData != null && num >= 0 && num2 >= 0)
					{
						for (int i = 0; i < jsonData.Count; i++)
						{
							JsonData jsonData2 = jsonData[i];
							list.Add(JsonMapper.ToObject<User>(jsonData2.ToJson()));
						}
						cb(SimpleAPIStatus.Success, null, list, num, num2);
					}
				}
			};
			MobageRequest request = MobageRequest.Request;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("startIndex", offset.ToString());
			dictionary.Add("count", count.ToString());
			Dictionary<string, object> dictionary2 = dictionary;
			if (relation == "mutual")
			{
				request.APIMethod = string.Format("opensocial/people/{0}/@friends", user.uid);
				if (!string.IsNullOrEmpty(game))
				{
					dictionary2["hasApp"] = "filterBy";
					dictionary2["equals"] = "filterOp";
					dictionary2["true"] = "filterValue";
				}
				request.QueryString = dictionary2;
				request.Send(callback);
			}
			else if (relation == "enemies")
			{
				request.APIMethod = string.Format("opensocial/blacklist/{0}/@all", user.uid);
				request.QueryString = dictionary2;
				request.Send(callback);
			}
			else
			{
				Debug.Log("MB_WW_UserList: Invalid relation (mutual or enemies expected).");
			}
		}
	}
}
