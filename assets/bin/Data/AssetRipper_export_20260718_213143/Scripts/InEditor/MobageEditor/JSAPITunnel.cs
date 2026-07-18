using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MobageEditor
{
	public class JSAPITunnel
	{
		private class GamePacker
		{
			public string description;

			public string icon_url;

			public string name;

			public string ngcore_url;

			public GamePacker(Game game)
			{
				description = game.longDescription;
				icon_url = game.iconUrl;
				name = game.name;
				ngcore_url = game.ngcore_url;
			}
		}

		private class UserWrapper
		{
			public int num_session;

			public string badge_id;

			public bool hasApp;

			public bool new_user_experience_complete;

			public bool mobage_user;

			public int level_points;

			public string aboutMe;

			public string thumbnailUrl;

			public int gamerscore;

			public string email;

			public int level_next_points;

			public bool opt_in;

			public int level_position;

			public string first_name;

			public string relation;

			public string uid;

			public bool new_buddy;

			public string nickname;

			public int userGrade;

			public int age;

			public int grade;

			public int unread_wall_count;

			public string last_name;

			public bool isFamous;

			public bool gamehub_user;

			public bool age_restricted;

			public UserWrapper(User user)
			{
				num_session = user.sessionCount;
				badge_id = user.badge_id;
				hasApp = user.hasApp;
				new_user_experience_complete = user.new_user_experience_complete;
				mobage_user = user.mobage_user;
				level_points = user.currentLevelScore;
				aboutMe = user.aboutMe;
				thumbnailUrl = user.thumbnailUrl;
				gamerscore = user.gamerScore;
				email = user.email;
				level_next_points = user.nextLevelScore;
				opt_in = user.mailOptInFlag;
				level_position = user.levelNumber;
				first_name = user.firstName;
				relation = user.relation;
				uid = user.uid;
				new_buddy = user.isNewBuddy;
				nickname = user.nickname;
				userGrade = user.grade;
				age = user.age;
				grade = user.grade;
				unread_wall_count = user.unreadWallPostCount;
				last_name = user.lastName;
				isFamous = user.isFamous;
				gamehub_user = user.isGameHubUser;
				age_restricted = user.age_restricted;
			}
		}

		private class ErrorPacker
		{
			public string localizedDescription;

			public int code;

			public string domain;

			private bool isNull;

			public ErrorPacker(Error error)
			{
				if (error == null)
				{
					isNull = true;
					return;
				}
				localizedDescription = error.localizedDescription;
				code = error.code;
				domain = error.domain;
			}

			public override string ToString()
			{
				if (isNull)
				{
					return "null";
				}
				return JsonMapper.ToJson(this);
			}
		}

		private class UserPacker
		{
			public int num_session;

			public string badge_id;

			public bool hasApp;

			public bool new_user_experience_complete;

			public bool mobage_user;

			public int level_points;

			public string aboutMe;

			public string thumbnailUrl;

			public int gamerscore;

			public int level_next_points;

			public bool opt_in;

			public int level_position;

			public string first_name;

			public string relation;

			public string uid;

			public bool new_buddy;

			public string nickname;

			public int age;

			public int userGrade;

			public int grade;

			public int unread_wall_count;

			public string last_name;

			public bool isFamous;

			public bool gamehub_user;

			public bool age_restricted;

			private bool isNull;

			public UserPacker(User user)
			{
				if (user == null)
				{
					isNull = true;
					return;
				}
				num_session = user.num_session;
				badge_id = user.badge_id;
				hasApp = user.hasApp;
				new_user_experience_complete = user.new_user_experience_complete;
				mobage_user = user.mobage_user;
				level_points = user.currentLevelScore;
				aboutMe = user.aboutMe;
				thumbnailUrl = user.thumbnailUrl;
				gamerscore = user.gamerScore;
				level_next_points = user.level_next_points;
				opt_in = user.opt_in;
				level_position = user.level_position;
				first_name = user.first_name;
				relation = user.relation;
				uid = user.uid;
				new_buddy = user.isNewBuddy;
				nickname = user.nickname;
				age = user.age;
				userGrade = user.grade;
				grade = user.grade;
				unread_wall_count = user.unreadWallPostCount;
				last_name = user.last_name;
				isFamous = user.isFamous;
				gamehub_user = user.isGameHubUser;
				age_restricted = user.age_restricted;
			}

			public override string ToString()
			{
				if (isNull)
				{
					return "null";
				}
				return JsonMapper.ToJson(this);
			}
		}

		public static void ForTunnelClient(IJSAPITunnelClient client, string messagesJSON)
		{
			JsonData jsonData = JsonMapper.ToObject(messagesJSON);
			for (int i = 0; i < jsonData.Count; i++)
			{
				JsonData jsonData2 = jsonData[i];
				string text = string.Format("{0}_{1}", jsonData2["interface"], jsonData2["method"]);
				try
				{
					Type typeFromHandle = typeof(JSAPITunnel);
					object target = Activator.CreateInstance(typeFromHandle);
					typeFromHandle.InvokeMember(text, BindingFlags.InvokeMethod, null, target, new object[2] { client, jsonData2 });
				}
				catch (MissingMethodException)
				{
					Debug.LogError("Error: Could not find handler: " + text);
				}
			}
		}

		public void Logger_log(IJSAPITunnelClient client, JsonData message)
		{
			Debug.Log("JSLog: " + message.ToJson());
		}

		public void CPIProduct_getProducts(IJSAPITunnelClient client, JsonData msg)
		{
			JsonData parameterNames = msg["parameterNames"];
			JsonData parameterValues = msg["parameterValues"];
			CPIProduct.GetCPIProducts(delegate
			{
				Debug.Log("sending message from CPIProduct_getProducts");
				client.sendMessage(string.Format("{{\"parameterValues\":[0,null,[]],\"callback\":\"{0}\"}}", parameterValues[parameterNames.IndexOf("onComplete")]));
			});
		}

		public void Game_getCurrentGame(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("Game_getCurrentGame:" + msg.ToJson());
			JsonData parameterNames = msg["parameterNames"];
			JsonData parameterValues = msg["parameterValues"];
			Game.GetCurrentGame(delegate(SimpleAPIStatus status, Error error, Game game)
			{
				if (error != null)
				{
					Debug.LogError("Error preparing message for JSAPITunnel: " + error.localizedDescription);
				}
				else
				{
					string msg2 = string.Format("{{\"parameterValues\":[0,null,{0}],\"callback\":\"{1}\"}}", JsonMapper.ToJson(new GamePacker(game)), parameterValues[parameterNames.IndexOf("onComplete")]);
					Debug.Log("sending from Game_getCurrentGame");
					client.sendMessage(msg2);
				}
			});
		}

		public void Game_getGames(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("Game_getGames " + msg.ToJson());
			JsonData parameterNames = msg["parameterNames"];
			JsonData parameterValues = msg["parameterValues"];
			int howMany = (int)parameterValues[0];
			int startOffset = (int)parameterValues[1];
			Game.GetGames(howMany, startOffset, delegate(SimpleAPIStatus status, Error error, List<Game> games, int offset, int totalPossibleResults)
			{
				string text = string.Format("{{\"parameterValues\":[0,null,{0}],\"callback\":\"{1}\"}}", JsonMapper.ToJson(ArrayPacker(games)), parameterValues[parameterNames.IndexOf("onComplete")]);
				Debug.Log("sending from Game_getGames " + text);
				client.sendMessage(text);
			});
		}

		public void Analytics_reportEvent(IJSAPITunnelClient client, JsonData msg)
		{
			JsonData jsonData = msg["parameterNames"];
			JsonData jsonData2 = msg["parameterValues"];
			Analytics.reportEvent(jsonData2[0].ToJson().Replace("\\\"", "\"").Trim('"'));
		}

		private List<GamePacker> ArrayPacker(List<Game> games)
		{
			List<GamePacker> list = new List<GamePacker>();
			foreach (Game game in games)
			{
				list.Add(new GamePacker(game));
			}
			return list;
		}

		public void People_getCurrentUser(IJSAPITunnelClient client, JsonData msg)
		{
			JsonData parameterNames = msg["parameterNames"];
			JsonData parameterValues = msg["parameterValues"];
			People.getCurrentUser(delegate(SimpleAPIStatus status, Error error, User currentUser)
			{
				if (error == null)
				{
					string msg2 = string.Format("{{\"parameterValues\":[0,null,{0}],\"callback\":\"{1}\"}}", JsonMapper.ToJson(new UserWrapper(currentUser)), parameterValues[parameterNames.IndexOf("onComplete")]);
					client.sendMessage(msg2);
				}
				else
				{
					string msg3 = "{\"parameterValues\":[1,{\"localizedDescription\":\"The operation couldn’t be completed. (com.mobage.error.api error 20001.)\",\"code\":20001,\"domain\":\"com.mobage.error.api\"},null],\"callback\":\"2\"}";
					client.sendMessage(msg3);
				}
			});
		}

		public void Analytics_getPlatformDynamicConfiguration(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("Analytics_getPlatformDynamicConfiguration:" + msg.ToJson());
			JsonData jsonData = msg["parameterNames"];
			JsonData jsonData2 = msg["parameterValues"];
			Analytics.GetPlatformDynamicConfiguration(delegate
			{
				string msg2 = "{\"parameterValues\":[0,null,{\"regFlow\":\"og\",\"regFlowTieredVariant\":\"v1\"}],\"callback\":\"3\"}";
				Debug.Log("Sending from Analytics_getPlatformDynamicConfiguration");
				client.sendMessage(msg2);
			});
		}

		public static void People_getUserForId(IJSAPITunnelClient client, JsonData msg)
		{
			JsonData parameterNames = msg["parameterNames"];
			JsonData parameterValues = msg["parameterValues"];
			string userId = (string)parameterValues[parameterNames.IndexOf("userId")];
			People.getUserForId(userId, delegate(SimpleAPIStatus status, Error error, User user)
			{
				string msg2 = string.Format("{{\"parameterValues\":[{0},{1},{2}],\"callback\":\"{3}\"}}", (int)status, new ErrorPacker(error), new UserPacker(user), parameterValues[parameterNames.IndexOf("onComplete")]);
				client.sendMessage(msg2);
			});
		}

		public static void SocialService_showToast(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("SocialService_showToast: " + msg.ToJson());
			JsonData jsonData = msg["parameterNames"];
			JsonData jsonData2 = msg["parameterValues"];
			SocialService.ShowToast(jsonData2[0].ToJson());
		}

		public static void JSUIWindow_dismissAndReturnArrayToNative(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("JSUIWindow_dismissAndReturnArrayToNative: " + msg.ToJson());
			JsonData jsonData = msg["parameterNames"];
			JsonData jsonData2 = msg["parameterValues"];
			client.DismissAndReturnArrayToNative(jsonData2[0]);
		}

		public static void JSUIWindow_presentTabWithOptions(IJSAPITunnelClient client, JsonData msg)
		{
			Debug.Log("JSUIWindow_presentTabWithOptions: " + msg.ToJson());
			JsonData jsonData = msg["parameterNames"];
			JsonData jsonData2 = msg["parameterValues"];
			client.PresentTabNamed((string)jsonData2[0], null);
		}

		public static int SimpleAPIStatusPacker(SimpleAPIStatus status)
		{
			return (int)status;
		}

		public static Dictionary<string, object> ModelPacker(ISerializableItem model)
		{
			if (model == null)
			{
				return new Dictionary<string, object>();
			}
			return model.PackForEnvironment(ModelSerializationEnvironment.ToJS);
		}

		public static List<object> ModelArrayPacker(List<CPIProduct> products)
		{
			List<object> list = new List<object>();
			foreach (CPIProduct product in products)
			{
				list.Add(ModelPacker(product));
			}
			return list;
		}
	}
}
