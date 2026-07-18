using System;
using System.Collections.Generic;
using LCD.Internal.Util;

namespace LCD.Internal.Impl
{
	public class LCDErrorImpl : LCDError
	{
		public LCDErrorImpl(string errorType, int errorCode, string errorMessage)
			: base(errorType, errorCode, errorMessage)
		{
		}

		public LCDErrorImpl(ErrorType errorType, int errorCode, string errorMessage)
			: base(errorType, errorCode, errorMessage)
		{
		}

		public static LCDErrorImpl CreateLCDError(string json)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
			string text = (string)dictionary["type"];
			int num = -1;
			if (dictionary["code"] != null)
			{
				num = Convert.ToInt32(dictionary["code"]);
			}
			string text2 = null;
			if (dictionary["message"] != null)
			{
				text2 = (string)dictionary["message"];
			}
			return new LCDErrorImpl(text, num, text2);
		}

		public string ToJsonString()
		{
			return ToJsonString(null);
		}

		public string ToJsonString(object requestId)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", errorType.ToString());
			dictionary.Add("code", errorCode);
			dictionary.Add("message", errorMessage);
			if (requestId != null)
			{
				dictionary.Add("requestId", requestId);
			}
			return Json.Serialize(dictionary);
		}
	}
}
