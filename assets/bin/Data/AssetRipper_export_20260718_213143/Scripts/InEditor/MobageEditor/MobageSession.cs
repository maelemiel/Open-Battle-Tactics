using Mobage;
using MobageEditor.US;
using UnityEngine;

namespace MobageEditor
{
	public abstract class MobageSession : IOAuthContext, INetworkingContext, IMobageSessionInternal
	{
		private static object lockObject = new object();

		public static MobageSession CurrentSession;

		protected ISessionEstablishmentInfo establishmentInfo;

		protected Characteristics characteristics;

		private bool triedWithProvidedCredentials;

		private ISessionAnalytics analytics;

		public string OAuth2Token
		{
			get
			{
				return NativeAPI.Instance.Oauth2Token;
			}
		}

		public string AnalyticsSessionId
		{
			get
			{
				return analytics.SessionId;
			}
		}

		public abstract string SocialServer { get; }

		public bool ServerModeIsProduction
		{
			get
			{
				return ServerStage == ServerStage.Production;
			}
		}

		public ServerStage ServerStage
		{
			get
			{
				return establishmentInfo.ServerStage;
			}
		}

		public ServerEnvironment ServerEnvironment
		{
			get
			{
				return establishmentInfo.ServerEnvironment;
			}
		}

		public IOAuthContext OAuthContext
		{
			get
			{
				return this;
			}
		}

		public OAuthConsumerInfo OAuthConsumerInfo
		{
			get
			{
				NativeAPI instance = NativeAPI.Instance;
				return new OAuthConsumerInfo(instance.OauthToken, instance.OauthSecret, establishmentInfo.ConsumerKey, establishmentInfo.ConsumerSecret);
			}
		}

		public string AppId
		{
			get
			{
				return establishmentInfo.AppId;
			}
		}

		public string AppVersion
		{
			get
			{
				string appVersion = establishmentInfo.AppVersion;
				return (!string.IsNullOrEmpty(appVersion)) ? appVersion : "1.0";
			}
		}

		public string AcceptLanguage
		{
			get
			{
				return "ja, en, fr, de, nl";
			}
		}

		public MobageSession(ISessionEstablishmentInfo establishmentInfo, ISessionAnalytics analytics)
		{
			characteristics = Characteristics.DefaultCharacteristics;
			this.analytics = analytics;
			this.establishmentInfo = establishmentInfo;
			triedWithProvidedCredentials = false;
			MobageLogger.log(string.Format("MB_G_MobageSession: Given Social server! {0}. appId: {1}", SocialServer, establishmentInfo.AppId));
		}

		public static MobageSession Session(ISessionEstablishmentInfo establishmentInfo, ISessionAnalytics analytics)
		{
			lock (lockObject)
			{
				CurrentSession = new WW_MobageSession(establishmentInfo, analytics);
				return CurrentSession;
			}
		}

		public void Logout()
		{
			EndSession();
			ModelDiskCache.DefaultCache.DeleteKey(TOKEN_KEY_FOR_SERVER(SocialServer));
		}

		public void EndSession()
		{
			CurrentSession = null;
		}

		public void UserUpdates()
		{
			Debug.Log("UserUpdates");
			MobageRequest mobageRequest = new MobageRequest(this);
			mobageRequest.APIMethod = "user_updates";
			mobageRequest.HTTPMethod = "GET";
			MobageRequest mobageRequest2 = mobageRequest;
			mobageRequest2.Send(delegate
			{
			});
		}

		protected string TOKEN_KEY_FOR_SERVER(string server)
		{
			return string.Format("AuthToken_{0}", server);
		}
	}
}
