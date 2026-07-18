namespace LCD.Internal.Web
{
	public interface SDKWebCallbackHandler
	{
		void onSuccess(string message);

		void onFailure(string message);
	}
}
