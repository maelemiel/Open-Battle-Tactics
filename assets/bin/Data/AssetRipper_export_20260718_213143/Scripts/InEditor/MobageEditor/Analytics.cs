using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class Analytics
	{
		private static JsonData cachedPlatformDynamicConfiguration;

		public static bool DisableCaching;

		public static void reportEvent(string eventString)
		{
			Mobage.sharedInstance.AnalyticsSession.ReportEventString(eventString);
		}

		public static void GetPlatformDynamicConfiguration(Action<SimpleAPIStatus, Error, JsonData> onComplete)
		{
			Debug.Log("GetPlatformDynamicConfiguration");
			string aPIMethod = "ab/service_variants";
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = aPIMethod;
			request.HTTPMethod = "GET";
			request.QueryString = new Dictionary<string, object>
			{
				{
					"device_id",
					Characteristics.DefaultCharacteristics.IdForPlatformServer
				},
				{
					"release_id",
					Mobage.sharedInstance.AppVersion
				}
			};
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (error == null)
				{
					onComplete(SimpleAPIStatus.Success, null, data);
				}
				else
				{
					onComplete(SimpleAPIStatus.Error, error, null);
				}
			});
		}

		public static void CachedPlatformDynamicConfiguration(Action<SimpleAPIStatus, Error, JsonData> onComplete)
		{
			Debug.Log("CachedPlatformDynamicConfiguration");
			if (DisableCaching || cachedPlatformDynamicConfiguration == null)
			{
				GetPlatformDynamicConfiguration(delegate(SimpleAPIStatus status, Error error, JsonData configuration)
				{
					cachedPlatformDynamicConfiguration = configuration;
					if (onComplete != null)
					{
						onComplete(status, error, configuration);
					}
				});
			}
			else if (onComplete != null)
			{
				onComplete(SimpleAPIStatus.Success, null, cachedPlatformDynamicConfiguration);
			}
		}
	}
}
