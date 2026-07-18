using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class LocationService
	{
		public extern bool isEnabledByUser
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern LocationServiceStatus status
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern LocationInfo lastData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Start([DefaultValue("10f")] float desiredAccuracyInMeters, [DefaultValue("10f")] float updateDistanceInMeters);

		[ExcludeFromDocs]
		public void Start(float desiredAccuracyInMeters)
		{
			float updateDistanceInMeters = 10f;
			Start(desiredAccuracyInMeters, updateDistanceInMeters);
		}

		[ExcludeFromDocs]
		public void Start()
		{
			float updateDistanceInMeters = 10f;
			float desiredAccuracyInMeters = 10f;
			Start(desiredAccuracyInMeters, updateDistanceInMeters);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Stop();
	}
}
