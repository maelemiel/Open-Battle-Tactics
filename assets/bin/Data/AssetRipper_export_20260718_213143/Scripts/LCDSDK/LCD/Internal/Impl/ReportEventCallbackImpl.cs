using LCD.Internal.Util;
using LCD.Internal.Web;

namespace LCD.Internal.Impl
{
	internal class ReportEventCallbackImpl : SDKWebCallbackHandler
	{
		private static string TAG = "ReportEventCallbackImpl";

		private LCDSDK.ReportEventCallback callback;

		internal ReportEventCallbackImpl(LCDSDK.ReportEventCallback callback)
		{
			this.callback = callback;
		}

		public void onSuccess(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			if (callback != null)
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
