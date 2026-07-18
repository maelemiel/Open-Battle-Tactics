using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Microphone
	{
		public static extern string[] devices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void End(string deviceName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool IsRecording(string deviceName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetPosition(string deviceName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq);
	}
}
