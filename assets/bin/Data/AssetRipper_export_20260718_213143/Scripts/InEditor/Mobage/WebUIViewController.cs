namespace Mobage
{
	public class WebUIViewController
	{
		private const string PRODUCTION_HOST = "webview.mobage.com";

		private const int WEBVIEW_INTERFACE_NUMBER = 4;

		private const int PRODUCTION = 3;

		private static string WebUI_host;

		private static string WebUI_portStr = string.Empty;

		private static string WebUI_interfaceString;

		public static string WebviewBaseURL
		{
			get
			{
				return string.Format("http://{0}{1}/{2}/", WebviewHost, WebUI_portStr, WebviewInterface);
			}
		}

		public static string WebviewHost
		{
			get
			{
				if (WebUI_host == null)
				{
					WebViewType = 3;
				}
				return WebUI_host;
			}
		}

		public static int WebViewType
		{
			set
			{
				WebUI_host = "webview.mobage.com";
			}
		}

		public static string WebviewInterface
		{
			get
			{
				if (WebUI_interfaceString == null)
				{
					InterfaceVersion = 4;
				}
				return WebUI_interfaceString;
			}
		}

		public static int InterfaceVersion
		{
			set
			{
				WebUI_interfaceString = string.Format("{0}", value);
			}
		}

		public static string URLWithURLStub(string urlStub)
		{
			string arg = "en";
			return string.Format("{0}{1}/{2}", WebviewBaseURL, arg, urlStub);
		}
	}
}
