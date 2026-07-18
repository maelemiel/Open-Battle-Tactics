namespace Mobage
{
	public class OAuthConsumerInfo
	{
		public string Token;

		public string TokenSecret;

		public string ConsumerKey;

		public string ConsumerSecret;

		public OAuthConsumerInfo(string token, string tokenSecret, string consumerKey, string consumerSecret)
		{
			Token = token;
			TokenSecret = tokenSecret;
			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
		}
	}
}
