using LCD.Internal.Interface;
using LCD.Internal.Model;

namespace LCD.Internal.Web
{
	public class SDKWebViewManagerInternal
	{
		public enum WebViewAction
		{
			ACTION_CREATE_SESSION = 0,
			ACTION_PURCHASE = 1,
			ACTION_LINK_ACCOUNT = 2,
			ACTION_LOAD_ACCOUNT = 3,
			ACTION_INVITATION = 4
		}

		private const string mWebViewManager = "WebViewManager";

		private LCDSDK.SDKWebViewProcess process = LCDSDK.SDKWebViewProcess.FINISHED;

		private static SDKWebViewManagerInternal sharedInstance;

		private LCDUnityEventHandler handler;

		public LCDSDK.SDKWebViewProcess currentProcess
		{
			get
			{
				return process;
			}
			set
			{
				process = value;
			}
		}

		public static SDKWebViewManagerInternal sharedManager
		{
			get
			{
				if (sharedInstance == null)
				{
					sharedInstance = new SDKWebViewManagerInternal();
				}
				return sharedInstance;
			}
		}

		public LCDUnityEventHandler eventHandler
		{
			get
			{
				return handler;
			}
			set
			{
				if (value != null)
				{
					handler = value;
				}
			}
		}

		public string getActionPath(WebViewAction action)
		{
			string result = null;
			switch (action)
			{
			case WebViewAction.ACTION_CREATE_SESSION:
				result = "createSession";
				break;
			case WebViewAction.ACTION_PURCHASE:
				result = "purchase";
				break;
			case WebViewAction.ACTION_LINK_ACCOUNT:
				result = "linkAccount";
				break;
			case WebViewAction.ACTION_LOAD_ACCOUNT:
				result = "loadAccount";
				break;
			case WebViewAction.ACTION_INVITATION:
				result = "invitation";
				break;
			}
			return result;
		}

		internal string getWebServerHost()
		{
			string text = "https://lcd";
			text = ((!Capabilities.sharedManager.sandbox) ? (text + "-prod") : (text + "-sandbox"));
			return text + ".appspot.com";
		}

		public string getUrl(WebViewAction action)
		{
			if (action == WebViewAction.ACTION_INVITATION)
			{
				return getWebServerHost() + "/invitation/ui/index.html?locale=" + Capabilities.sharedManager.locale;
			}
			return getWebServerHost() + "/LCDWeb.html?action=" + getActionPath(action) + "&locale=" + Capabilities.sharedManager.locale;
		}
	}
}
