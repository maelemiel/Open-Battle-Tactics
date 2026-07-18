using LCD.Internal.Impl;
using LCD.Internal.Web;

namespace LCD.User
{
	public class User
	{
		public delegate void LinkAccountCallback(LCDError error);

		public delegate void LoadAccountCallback(long newUserId, long oldUserId, LCDError error);

		public delegate void OpenInvitationUICallback(LCDError error);

		public readonly long userId;

		public readonly string country;

		public readonly string region;

		public readonly string city;

		public readonly bool developer;

		public readonly StoreAccount storeAccount;

		private static User instance;

		private User(long userId, string country, string region, string city, bool developer, StoreAccount storeAccount)
		{
			this.userId = userId;
			this.country = country;
			this.region = region;
			this.city = city;
			this.developer = developer;
			this.storeAccount = storeAccount;
		}

		public void LinkAccount(LinkAccountCallback callback)
		{
			SDKWebUtil.openSDKWebView("linkAccount", null, new LinkAccountCallbackImpl(callback), false);
		}

		public void LoadAccount(LoadAccountCallback callback)
		{
			SDKWebUtil.openSDKWebView("loadAccount", null, new LoadAccountCallbackImpl(callback), false);
		}

		public void OpenInvitationUI(OpenInvitationUICallback callback)
		{
			SDKWebUtil.openSDKWebView("invitation", null, new OpenInvitationUICallbackImpl(callback), false);
		}

		internal static User getInstance()
		{
			return instance;
		}

		internal static void setInstance(long userId, string country, string region, string city, bool developer, StoreAccount storeAccount)
		{
			instance = new User(userId, country, region, city, developer, storeAccount);
		}
	}
}
