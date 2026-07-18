using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobageEditor
{
	public class RemoteNotificationResponse
	{
		public string uid;

		public string responseId;

		public RemoteNotificationPayload payload;

		public string publishedTimestamp;

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRemoteNotificationResponse(IntPtr obj, short shouldDeepCopy);

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRemoteNotificationResponse_Array(IntPtr obj, short shouldCopyArrayElements);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRemoteNotificationResponse(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRemoteNotificationResponse(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRemoteNotificationResponse_Array(IntPtr objects);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRemoteNotificationResponse_Array(IntPtr objects);
	}
}
