using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class GameLeaderboard
	{
		public delegate void getLeaderboardForId_onCompleteCallback(SimpleAPIStatus status, Error error, GameLeaderboard leaderboard);

		public delegate void getLeaderboardsForIds_onCompleteCallback(SimpleAPIStatus status, Error error, List<GameLeaderboard> leaderboards);

		public delegate void getAllLeaderboards_onCompleteCallback(SimpleAPIStatus status, Error error, List<GameLeaderboard> leaderboards);

		public delegate void getScoresForLeaderboard_onCompleteCallback(SimpleAPIStatus status, Error error, List<Score> scores);

		public delegate void getFriendsScoresForLeaderboard_onCompleteCallback(SimpleAPIStatus status, Error error, List<Score> scores);

		public delegate void getScoreForLeaderboard_onCompleteCallback(SimpleAPIStatus status, Error error, Score score);

		public delegate void updateCurrentUserScoreForLeaderboard_onCompleteCallback(SimpleAPIStatus status, Error error, Score score);

		public delegate void deleteCurrentUserScoreForLeaderboard_onCompleteCallback(SimpleAPIStatus status, Error error);

		public string uid;

		public string appId;

		public string title;

		public string scoreFormat;

		public int scorePrecision;

		public string iconUrl;

		public bool allowLowerScore;

		public bool reverse;

		public bool archived;

		public double defaultScore;

		public string published;

		public string updated;

		public int id
		{
			get
			{
				return int.Parse(uid);
			}
			set
			{
				uid = value.ToString();
			}
		}

		private static Dictionary<string, object> _allLeaderboardFields
		{
			get
			{
				string[] value = new string[13]
				{
					"id", "appid", "title", "unit", "scoreformat", "scoreprecision", "iconurl", "allowlowerscore", "reverse", "defaultscore",
					"archived", "published", "updated"
				};
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("fields", string.Join(",", value));
				return dictionary;
			}
		}

		public static Dictionary<string, object> AllScoreFields
		{
			get
			{
				string[] value = new string[5] { "userid", "value", "displayvalue", "rank", "updated" };
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("fields", string.Join(",", value));
				return dictionary;
			}
		}

		public static void getLeaderboardForId(string leaderboardId, getLeaderboardForId_onCompleteCallback onComplete)
		{
			string aPIMethod = "opensocial/leaderboards/@app/" + leaderboardId;
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = _allLeaderboardFields;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				GameLeaderboard leaderboard = null;
				if (err != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					leaderboard = JsonMapper.ToObject<GameLeaderboard>(data.ToJson());
				}
				onComplete(status2, err, leaderboard);
			});
		}

		public static void getLeaderboardsForIds(List<string> leaderboardIds, getLeaderboardsForIds_onCompleteCallback completeCb)
		{
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}", string.Join(",", leaderboardIds.ToArray()));
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = _allLeaderboardFields;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				List<GameLeaderboard> list = new List<GameLeaderboard>();
				if (err != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					GameLeaderboard item = JsonMapper.ToObject<GameLeaderboard>(data.ToJson());
					list.Add(item);
				}
				completeCb(status2, err, list);
			});
		}

		public static void getAllLeaderboards(getAllLeaderboards_onCompleteCallback completeCb)
		{
			Action<SimpleAPIStatus, Error, List<GameLeaderboard>> finalCallback = delegate(SimpleAPIStatus status, Error error, List<GameLeaderboard> leaderboards)
			{
				completeCb(status, error, leaderboards);
			};
			string aPIMethod = "opensocial/leaderboards/@app";
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = _allLeaderboardFields;
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				Error arg = null;
				SimpleAPIStatus arg2 = SimpleAPIStatus.Success;
				List<GameLeaderboard> list = new List<GameLeaderboard>();
				if (error != null)
				{
					arg = error;
					arg2 = SimpleAPIStatus.Error;
				}
				else
				{
					JsonData jsonData = data["entry"];
					for (int i = 0; i < jsonData.Count; i++)
					{
						GameLeaderboard item = JsonMapper.ToObject<GameLeaderboard>(jsonData[i].ToJson());
						list.Add(item);
					}
				}
				finalCallback(arg2, arg, list);
			});
		}

		public static void getScoresForLeaderboardId(string leaderboardId, int count, int startIndex, getScoresForLeaderboard_onCompleteCallback completeCb)
		{
			Dictionary<string, object> allScoreFields = AllScoreFields;
			allScoreFields["startIndex"] = startIndex;
			allScoreFields["count"] = count;
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}/@me/@all", leaderboardId);
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = allScoreFields;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				List<Score> list = new List<Score>();
				if (err != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					JsonData jsonData = data["entry"];
					for (int i = 0; i < jsonData.Count; i++)
					{
						Score item = JsonMapper.ToObject<Score>(jsonData[i].ToJson());
						list.Add(item);
					}
				}
				completeCb(status2, err, list);
			});
		}

		public static void getScoresForLeaderboard(GameLeaderboard leaderboard, int count, int startIndex, getScoresForLeaderboard_onCompleteCallback onComplete)
		{
			getScoresForLeaderboardId(leaderboard.uid, count, startIndex, onComplete);
		}

		public static void getFriendsScoresForLeaderboardId(string leaderboardId, int count, int startIndex, getFriendsScoresForLeaderboard_onCompleteCallback completeCb)
		{
			Dictionary<string, object> allScoreFields = AllScoreFields;
			allScoreFields["startIndex"] = startIndex;
			allScoreFields["count"] = count;
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}/@me/@friends", leaderboardId);
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = allScoreFields;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				List<Score> list = new List<Score>();
				if (err != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					JsonData jsonData = data["entry"];
					foreach (object item2 in (IDictionary)jsonData)
					{
						if (item2 is JsonData)
						{
							Score item = JsonMapper.ToObject<Score>(((JsonData)item2).ToJson());
							list.Add(item);
						}
					}
				}
				completeCb(status2, err, list);
			});
		}

		public static void getFriendsScoresForLeaderboard(GameLeaderboard leaderboard, int count, int startIndex, getFriendsScoresForLeaderboard_onCompleteCallback onComplete)
		{
			getFriendsScoresForLeaderboardId(leaderboard.uid, count, startIndex, onComplete);
		}

		public static void getScoreForLeaderboardId(string leaderboardId, string userId, getScoreForLeaderboard_onCompleteCallback completeCb)
		{
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}/{1}/@self", leaderboardId, userId);
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = AllScoreFields;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				Score score = null;
				if (err != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					score = JsonMapper.ToObject<Score>(data.ToJson());
				}
				completeCb(status2, err, score);
			});
		}

		public static void getScoreForLeaderboard(GameLeaderboard leaderboard, User user, getScoreForLeaderboard_onCompleteCallback onComplete)
		{
			if (user == null)
			{
				onComplete(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "user is empty",
					domain = "GameLeaderboard",
					code = 400
				}, null);
			}
			else
			{
				getScoreForLeaderboardId(leaderboard.uid, user.uid, onComplete);
			}
		}

		public static void updateCurrentUserScoreForLeaderboardId(string leaderboardId, double val, updateCurrentUserScoreForLeaderboard_onCompleteCallback completeCb)
		{
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}/@me/@self", leaderboardId);
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "PUT";
			request.QueryString = new Dictionary<string, object> { { "fields", "value" } };
			request.PostBody = new Dictionary<string, object> { { "value", val } };
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				SimpleAPIStatus status2 = SimpleAPIStatus.Success;
				Score score = null;
				if (error != null)
				{
					status2 = SimpleAPIStatus.Error;
				}
				else
				{
					score = JsonMapper.ToObject<Score>(data.ToJson());
				}
				completeCb(status2, error, score);
			});
		}

		public static void updateCurrentUserScoreForLeaderboard(GameLeaderboard leaderboard, double val, updateCurrentUserScoreForLeaderboard_onCompleteCallback onComplete)
		{
			updateCurrentUserScoreForLeaderboardId(leaderboard.uid, val, onComplete);
		}

		public static void deleteCurrentUserScoreForLeaderboard(GameLeaderboard leaderboard, deleteCurrentUserScoreForLeaderboard_onCompleteCallback onComplete)
		{
			deleteCurrentUserScoreForLeaderboardId(leaderboard.uid, onComplete);
		}

		public static void deleteCurrentUserScoreForLeaderboardId(string leaderboardId, deleteCurrentUserScoreForLeaderboard_onCompleteCallback completeCb)
		{
			string aPIMethod = string.Format("opensocial/leaderboards/@app/{0}/@me/@self", leaderboardId);
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "DELETE";
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				completeCb((err != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, err);
			});
		}
	}
}
