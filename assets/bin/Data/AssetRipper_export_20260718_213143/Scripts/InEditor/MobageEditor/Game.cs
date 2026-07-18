using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class Game : ISerializableItem
	{
		public string uid;

		public string name;

		public string longDescription;

		public string publisherName;

		public string appStoreURL;

		public string appKey;

		public string iconUrl;

		public string largeIconUrl;

		public bool installed;

		public bool featured;

		public string promotionImageUrl;

		public string description
		{
			get
			{
				return longDescription;
			}
			set
			{
				longDescription = value;
			}
		}

		public string icon_url
		{
			get
			{
				return iconUrl;
			}
			set
			{
				iconUrl = value;
			}
		}

		public string ngcore_url { get; set; }

		public bool Installed { get; set; }

		public static void GetCurrentGame(Action<SimpleAPIStatus, Error, Game> completeCb)
		{
			Debug.Log("GetCurrentGame");
			GetGame(Mobage.sharedInstance.AppId, completeCb);
		}

		public static void GetGame(string appkey, Action<SimpleAPIStatus, Error, Game> completeCb)
		{
			Debug.Log("GetGame " + appkey);
			Action<Error, Game> finalCallback = delegate(Error error, Game game)
			{
				completeCb((error != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, error, game);
			};
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "games/" + appkey;
			request.HTTPMethod = "GET";
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				Error arg = null;
				Game game = null;
				if (error != null)
				{
					arg = error;
				}
				else
				{
					try
					{
						JsonData jsonData = data["game"];
						game = JsonMapper.ToObject<Game>(jsonData.ToJson());
						game.Installed = true;
					}
					catch
					{
						Debug.Log("getGame got no game");
						arg = error;
					}
				}
				finalCallback(arg, game);
			});
		}

		public static void GetGames(int howMany, int startOffset, Action<SimpleAPIStatus, Error, List<Game>, int, int> completeCb)
		{
			requestGameList(delegate(SimpleAPIStatus status, Error error, List<Game> games)
			{
				int num = games.Count - 1;
				int num2 = 0;
				List<Game> list = new List<Game>();
				foreach (Game game in games)
				{
					if (howMany > 0 && (num2 < startOffset || num2 > startOffset + howMany))
					{
						num2++;
					}
					else
					{
						if (game.appKey != Mobage.sharedInstance.AppId)
						{
							list.Add(game);
						}
						num2++;
					}
				}
				completeCb(status, error, list, startOffset, list.Count);
			});
		}

		private static void requestGameList(Action<SimpleAPIStatus, Error, List<Game>> completeCb)
		{
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "games";
			request.HTTPMethod = "GET";
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				Error error = null;
				List<Game> list = null;
				if (err != null)
				{
					error = err;
				}
				else if (data.Contains("games"))
				{
					Debug.Log("total games count: " + data["games"].Count);
					list = new List<Game>();
					JsonData jsonData = data["games"];
					for (int i = 0; i < jsonData.Count; i++)
					{
						JsonData jsonData2 = jsonData[i];
						Debug.Log("game:" + jsonData2.ToJson());
						Game game = JsonMapper.ToObject<Game>(jsonData2.ToJson());
						game.installed = !string.IsNullOrEmpty(game.appKey) && GameUtilities.CanLaunch(game.appKey);
						list.Add(game);
					}
				}
				else
				{
					Debug.LogError("requestGameList got no games");
					error = new Error
					{
						localizedDescription = "Invalid response from server.",
						domain = "com.mobage.error.api",
						code = 10005
					};
				}
				completeCb((error != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, error, list);
			});
		}

		public Dictionary<string, object> PackForEnvironment(ModelSerializationEnvironment env)
		{
			return new Dictionary<string, object>();
		}
	}
}
