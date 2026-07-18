namespace System.Net
{
	public interface IWebProxyScript
	{
		void Close();

		bool Load(Uri scriptLocation, string Script, Type helperType);

		string Run(string url, string host);
	}
}
