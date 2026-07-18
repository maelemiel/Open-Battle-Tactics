using SimpleJSON;

namespace com.adjust.sdk
{
	public class ResponseData
	{
		public enum ActivityKind
		{
			UNKNOWN = 0,
			SESSION = 1,
			EVENT = 2,
			REVENUE = 3,
			REATTRIBUTION = 4
		}

		public ActivityKind? activityKind { get; private set; }

		public string activityKindString { get; private set; }

		public bool? success { get; private set; }

		public bool? willRetry { get; private set; }

		public string error { get; private set; }

		public string trackerToken { get; private set; }

		public string trackerName { get; private set; }

		public string network { get; private set; }

		public string campaign { get; private set; }

		public string adgroup { get; private set; }

		public string creative { get; private set; }

		public ResponseData(string jsonString)
		{
			JSONNode jSONNode = JSON.Parse(jsonString);
			if (!(jSONNode == null))
			{
				activityKind = ParseActivityKind(getJsonString(jSONNode, "activityKind"));
				activityKindString = activityKind.ToString().ToLower();
				success = getJsonBool(jSONNode, "success");
				willRetry = getJsonBool(jSONNode, "willRetry");
				error = getJsonString(jSONNode, "error");
				trackerName = getJsonString(jSONNode, "trackerName");
				trackerToken = getJsonString(jSONNode, "trackerToken");
				network = getJsonString(jSONNode, "network");
				campaign = getJsonString(jSONNode, "campaign");
				adgroup = getJsonString(jSONNode, "adgroup");
				creative = getJsonString(jSONNode, "creative");
			}
		}

		private string getJsonString(JSONNode node, string key)
		{
			JSONNode jsonValue = getJsonValue(node, key);
			if (jsonValue == null)
			{
				return null;
			}
			return jsonValue.Value;
		}

		private bool? getJsonBool(JSONNode node, string key)
		{
			JSONNode jsonValue = getJsonValue(node, key);
			if (jsonValue == null)
			{
				return null;
			}
			return jsonValue.AsBool;
		}

		private JSONNode getJsonValue(JSONNode node, string key)
		{
			if (node == null)
			{
				return null;
			}
			JSONNode jSONNode = node[key];
			if (jSONNode.GetType() == typeof(JSONLazyCreator))
			{
				return null;
			}
			return jSONNode;
		}

		private ActivityKind ParseActivityKind(string sActivityKind)
		{
			if ("session" == sActivityKind)
			{
				return ActivityKind.SESSION;
			}
			if ("event" == sActivityKind)
			{
				return ActivityKind.EVENT;
			}
			if ("revenue" == sActivityKind)
			{
				return ActivityKind.REVENUE;
			}
			if ("reattribution" == sActivityKind)
			{
				return ActivityKind.REATTRIBUTION;
			}
			return ActivityKind.UNKNOWN;
		}
	}
}
