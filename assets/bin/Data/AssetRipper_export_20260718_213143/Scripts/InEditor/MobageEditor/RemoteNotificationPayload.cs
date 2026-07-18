using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobageEditor
{
	public class RemoteNotificationPayload
	{
		public string message;

		public int badge;

		public string sound;

		public string collapseKey;

		public string style;

		public string iconUrl;

		public List<string> extraKeys;

		public List<string> extraValues;

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRemoteNotificationPayload(IntPtr obj, short shouldDeepCopy);

		[DllImport("NDKPlugin")]
		public static extern IntPtr MBCCopyConstructRemoteNotificationPayload_Array(IntPtr obj, short shouldCopyArrayElements);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRemoteNotificationPayload(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRemoteNotificationPayload(IntPtr obj);

		[DllImport("NDKPlugin")]
		public static extern void MBCRetainRemoteNotificationPayload_Array(IntPtr objects);

		[DllImport("NDKPlugin")]
		public static extern short MBCReleaseRemoteNotificationPayload_Array(IntPtr objects);
	}
}
