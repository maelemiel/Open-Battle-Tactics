namespace MobageEditor
{
	public interface INetworkingContext
	{
		string SocialServer { get; }

		string AcceptLanguage { get; }

		bool ServerModeIsProduction { get; }

		IOAuthContext OAuthContext { get; }

		string AppId { get; }

		string AnalyticsSessionId { get; }
	}
}
