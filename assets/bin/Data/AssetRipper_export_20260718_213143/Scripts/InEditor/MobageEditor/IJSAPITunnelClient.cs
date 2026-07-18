namespace MobageEditor
{
	public interface IJSAPITunnelClient
	{
		void sendMessage(string msg);

		void PresentTabNamed(string name, JsonData options);

		void DismissAndReturnArrayToNative(JsonData arr);
	}
}
