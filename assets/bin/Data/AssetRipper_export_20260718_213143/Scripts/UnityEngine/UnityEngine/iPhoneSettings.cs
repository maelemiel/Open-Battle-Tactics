using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class iPhoneSettings
	{
		[Obsolete("screenOrientation property is deprecated. Please use Screen.orientation instead.")]
		public static extern iPhoneScreenOrientation screenOrientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("verticalOrientation property is deprecated. Please use Screen.orientation instead.")]
		public static extern bool verticalOrientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("screenCanDarken property is deprecated. Please use Screen.sleepTimeout instead.")]
		public static extern bool screenCanDarken
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("uniqueIdentifier property is deprecated. Please use SystemInfo.deviceUniqueIdentifier instead.")]
		public static extern string uniqueIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("name property is deprecated. Please use SystemInfo.deviceName instead.")]
		public static extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("model property is deprecated. Please use SystemInfo.deviceModel instead.")]
		public static extern string model
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("systemName property is deprecated. Please use SystemInfo.operatingSystem instead.")]
		public static extern string systemName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("systemVersion property is deprecated. Please use SystemInfo.operatingSystem instead.")]
		public static extern string systemVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("internetReachability property is deprecated. Please use Application.internetReachability instead.")]
		public static extern iPhoneNetworkReachability internetReachability
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("generation property is deprecated. Please use iPhone.generation instead.")]
		public static extern iPhoneGeneration generation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("locationServiceStatus property is deprecated. Please use Input.location.status instead.")]
		public static extern LocationServiceStatus locationServiceStatus
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("locationServiceEnabledByUser property is deprecated. Please use Input.location.isEnabledByUser instead.")]
		public static extern bool locationServiceEnabledByUser
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static extern void StartLocationServiceUpdates(float desiredAccuracyInMeters, float updateDistanceInMeters);

		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters)
		{
			StartLocationServiceUpdates(desiredAccuracyInMeters, 10f);
		}

		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates()
		{
			StartLocationServiceUpdates(10f, 10f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("StopLocationServiceUpdates method is deprecated. Please use Input.location.Stop instead.")]
		public static extern void StopLocationServiceUpdates();
	}
}
