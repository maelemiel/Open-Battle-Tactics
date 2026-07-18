using System.Collections.Generic;

namespace MobageEditor.US
{
	public class WW_MobageSession : MobageSession
	{
		public new ServerStage ServerStage
		{
			get
			{
				return establishmentInfo.ServerStage;
			}
		}

		public override string SocialServer
		{
			get
			{
				if (establishmentInfo.ServerStage == ServerStage.UnitTests)
				{
					return "localhost:3000";
				}
				string arg = string.Empty;
				string arg2 = string.Empty;
				if (establishmentInfo.ServerEnvironment != ServerEnvironment.Production)
				{
					arg2 = "-sandbox";
				}
				switch (establishmentInfo.ServerStage)
				{
				case ServerStage.Staging:
					arg = ".staging";
					break;
				case ServerStage.Integration:
					arg = ".integration";
					break;
				}
				return string.Format("app{0}{1}.mobage.com", arg2, arg);
			}
		}

		private Dictionary<string, object> LoginParameters
		{
			get
			{
				string appId = Mobage.sharedInstance.AppId;
				Dictionary<string, object> dictionary;
				if (appId != null)
				{
					appId = appId.ToLower();
					if (appId.Contains("-android"))
					{
						dictionary = new Dictionary<string, object>();
						dictionary.Add("timezone", characteristics.Timezone);
						dictionary.Add("device_type", characteristics.PlatformOS);
						dictionary.Add("os_version", string.Format("Android/{0}", characteristics.PlatformOSVersion));
						dictionary.Add("id", characteristics.PrefixedDeviceId);
						dictionary.Add("locale", characteristics.Locale);
						return dictionary;
					}
					dictionary = new Dictionary<string, object>();
					dictionary.Add("timezone", characteristics.Timezone);
					dictionary.Add("device_type", characteristics.PlatformOS);
					dictionary.Add("os_version", string.Format("iOS/{0}", characteristics.PlatformOSVersion));
					dictionary.Add("id", characteristics.PrefixedDeviceId);
					dictionary.Add("locale", characteristics.Locale);
					return dictionary;
				}
				dictionary = new Dictionary<string, object>();
				dictionary.Add("timezone", characteristics.Timezone);
				dictionary.Add("device_type", characteristics.PlatformOS);
				dictionary.Add("os_version", string.Format("Android/{0}", characteristics.PlatformOSVersion));
				dictionary.Add("id", characteristics.PrefixedDeviceId);
				dictionary.Add("locale", characteristics.Locale);
				return dictionary;
			}
		}

		public WW_MobageSession(ISessionEstablishmentInfo info, ISessionAnalytics anal)
			: base(info, anal)
		{
			MB_WW_AssetCaching.StartCaching();
			MobageUI.Instance.UpdateSitemap(new List<string> { "Registration" }, delegate
			{
				MobageLogger.log("Sitemap with only registration experience became ready during initial setup!");
			});
		}

		private string TokenKeyForServer(string server)
		{
			return string.Format("AuthToken_{0}", server);
		}

		public User UserFromDictionary(JsonData dictionary, string etag)
		{
			User user = User.Create(dictionary);
			if (!string.IsNullOrEmpty(etag))
			{
				user.CacheControl = etag;
			}
			return user;
		}
	}
}
