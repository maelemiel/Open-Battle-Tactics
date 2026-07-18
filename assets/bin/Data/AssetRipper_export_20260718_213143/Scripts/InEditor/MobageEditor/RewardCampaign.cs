using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobageEditor
{
	public class RewardCampaign
	{
		public delegate void getActiveCampaigns_onCompleteCallback(SimpleAPIStatus status, Error error, List<RewardCampaign> activeCampaigns);

		public string uid;

		public int redemptions;

		public int startsAt;

		public int endsAt;

		public int expiresAt;

		public string payload;

		public List<RewardCampaignCode> codes;

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRewardCampaign(IntPtr obj, short shouldDeepCopy);

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRewardCampaign_Array(IntPtr obj, short shouldCopyArrayElements);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRewardCampaign(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRewardCampaign(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRewardCampaign_Array(IntPtr objects);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRewardCampaign_Array(IntPtr objects);
	}
}
