using LCD.Internal.Util;
using LCD.Internal.Web;
using LCD.User;

namespace LCD.Internal.Impl
{
	internal class OpenInvitationUICallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "OpenInvitationUICallbackImpl";

		private LCD.User.User.OpenInvitationUICallback callback;

		internal OpenInvitationUICallbackImpl(LCD.User.User.OpenInvitationUICallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			if (message != null && callback != null)
			{
				callback(null);
			}
		}

		public void onFailure(string message)
		{
			LCDSDKLog.Debug(TAG, "onFailure : " + message);
			LCDErrorImpl error = LCDErrorImpl.CreateLCDError(message);
			if (callback != null)
			{
				callback(error);
			}
		}
	}
}
