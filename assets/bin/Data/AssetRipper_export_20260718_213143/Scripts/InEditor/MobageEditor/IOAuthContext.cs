using Mobage;

namespace MobageEditor
{
	public interface IOAuthContext
	{
		OAuthConsumerInfo OAuthConsumerInfo { get; }

		string OAuth2Token { get; }

		string AppVersion { get; }
	}
}
