namespace System.Net
{
	[Obsolete("Use WebRequest.DefaultProxy instead")]
	public class GlobalProxySelection
	{
		internal class EmptyWebProxy : IWebProxy
		{
			private ICredentials credentials;

			public ICredentials Credentials
			{
				get
				{
					return credentials;
				}
				set
				{
					credentials = value;
				}
			}

			internal EmptyWebProxy()
			{
			}

			public Uri GetProxy(Uri destination)
			{
				return destination;
			}

			public bool IsBypassed(Uri host)
			{
				return true;
			}
		}

		public static IWebProxy Select
		{
			get
			{
				return WebRequest.DefaultWebProxy;
			}
			set
			{
				WebRequest.DefaultWebProxy = value;
			}
		}

		public static IWebProxy GetEmptyWebProxy()
		{
			return new EmptyWebProxy();
		}
	}
}
