using System;
using UnityEngine;

namespace MobageEditor
{
	public class MB_WW_Registration : WebExperience
	{
		public Action<CancelableAPIStatus, Error> FinalCallback;

		private static JsonData abPayload;

		public MB_WW_Registration(JsonData dict)
			: base(dict)
		{
		}

		public static void PrepareRegistration()
		{
			Analytics.CachedPlatformDynamicConfiguration(delegate(SimpleAPIStatus status, Error error, JsonData configuration)
			{
				abPayload = configuration;
			});
		}

		public override void DismissAndReturnArrayToNative(JsonData arr)
		{
			Debug.Log("DismissAndReturnArrayToNative");
			Debug.Log("Registration sent the following to native: " + arr.ToJson());
			CancelableAPIStatus status = (CancelableAPIStatus)(int)arr[0];
			DismissWindow(delegate
			{
				if (FinalCallback != null)
				{
					FinalCallback(status, null);
					FinalCallback = null;
					MobageSession currentSession = MobageSession.CurrentSession;
					currentSession.UserUpdates();
				}
			});
		}
	}
}
