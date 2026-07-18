using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobageEditor
{
	public class RewardCampaignCode
	{
		public string uid;

		public string code;

		public string channel;

		public string marketingCopy;

		public string iconUrl;

		public int redemptions;

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRewardCampaignCode(IntPtr obj, short shouldDeepCopy);

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRewardCampaignCode_Array(IntPtr obj, short shouldCopyArrayElements);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRewardCampaignCode(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRewardCampaignCode(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRewardCampaignCode_Array(IntPtr objects);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRewardCampaignCode_Array(IntPtr objects);
	}
}
