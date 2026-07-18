using System;
using System.Collections.Generic;
using LitJson0;
using UnityEngine;

public class ServerUtilities
{
	public class BossResponse : BaseResponse
	{
	}

	public class CreateMatchResponse : BaseResponse
	{
		public bool success;

		public OpponentData opponent;

		public OpponentData player;

		public bool playerIsHost;

		public int battleSeed;

		public int playerSeed;

		public int enemySeed;

		public string matchId;

		public bool abortSequence;
	}

	public class RequestTokenResponse : BaseResponse
	{
		public string requestToken;
	}

	public class LoginResponse : Response
	{
	}

	public class PvpResponse : BaseResponse
	{
	}

	public class ReceiveActionsResponse : BaseResponse
	{
		public List<BattleAction> actions;

		public bool success;
	}

	public class Session
	{
		public string sessionCookie;

		public string sessionCookieUID;

		public int dataModelVersion;

		public string dataModelHash;

		public string dataModelUrl;

		public string dataModelFullUrl;

		public long serverTime;
	}

	public class Error
	{
		public enum Code
		{
			None = 0,
			Parse = 1,
			Exception = 2,
			Communication = 3,
			API = 4,
			DataModelUpdateRequired = 5,
			BinaryUpdateRequired = 6,
			SessionExpired = 7,
			Maintenance = 8,
			TimeOutForfeit = 9,
			MobagePushNotificationNotActive = 10,
			MobagePushLimitReached = 11,
			OutOfSync = 12,
			PCLoadLetter = 13
		}

		public Code code;

		public string message;

		private string _description;

		public object payload;

		public string description
		{
			get
			{
				return code.ToString() + ": " + _description;
			}
		}

		public Error(Code code, string description, string msg = null)
		{
			this.code = code;
			_description = description;
			message = msg;
		}

		public Error(Code code, string description, object payload)
		{
			this.code = code;
			_description = description;
			this.payload = payload;
		}

		public override string ToString()
		{
			return description;
		}

		public static Error Parse(string description, string message = null)
		{
			return new Error(Code.Parse, description, message);
		}

		public static Error Exception(Exception e)
		{
			return new Error(Code.Exception, e.ToString());
		}

		public static Error Communication(string message)
		{
			return new Error(Code.Communication, message);
		}

		public static Error DataModelUpdateRequired()
		{
			return new Error(Code.DataModelUpdateRequired, string.Empty);
		}

		public static Error OutOfSync()
		{
			return new Error(Code.OutOfSync, string.Empty);
		}

		public static Error MobagePushNotificationNotActive()
		{
			return new Error(Code.MobagePushNotificationNotActive, string.Empty);
		}

		public static Error MobagePushLimitReached()
		{
			return new Error(Code.MobagePushLimitReached, string.Empty);
		}
	}

	public class BaseResponse
	{
		public Error error;

		public Session session;

		public JsonObject json;

		public BaseResponse()
		{
		}

		public BaseResponse(Session session, JsonObject json)
		{
			this.session = session;
			this.json = json;
		}
	}

	public class Response : BaseResponse
	{
	}

	public class ShopResponse : BaseResponse
	{
	}

	public delegate void RequestTokenCallback(RequestTokenResponse response);

	public delegate void LoginCallback(LoginResponse response);

	private static int requestId = UnityEngine.Random.Range(1, 2147473647);

	private static string PC_LOAD_LETTER_HEADER = "superbattletactics-pc-load-letter";
}
