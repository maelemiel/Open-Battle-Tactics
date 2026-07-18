namespace MobageEditor
{
	public interface ISessionEstablishmentInfo
	{
		ServerStage ServerStage { get; }

		ServerEnvironment ServerEnvironment { get; }

		string ConsumerKey { get; }

		string ConsumerSecret { get; }

		string AppId { get; }

		string AppVersion { get; }
	}
}
