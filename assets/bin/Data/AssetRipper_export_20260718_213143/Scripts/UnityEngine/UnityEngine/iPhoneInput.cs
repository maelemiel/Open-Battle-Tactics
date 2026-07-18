using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class iPhoneInput
	{
		[Obsolete("accelerationEvents property is deprecated. Please use Input.accelerationEvents instead.")]
		public static iPhoneAccelerationEvent[] accelerationEvents
		{
			get
			{
				int num = accelerationEventCount;
				iPhoneAccelerationEvent[] array = new iPhoneAccelerationEvent[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = GetAccelerationEvent(i);
				}
				return array;
			}
		}

		[Obsolete("touches property is deprecated. Please use Input.touches instead.")]
		public static iPhoneTouch[] touches
		{
			get
			{
				int num = touchCount;
				iPhoneTouch[] array = new iPhoneTouch[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = GetTouch(i);
				}
				return array;
			}
		}

		[Obsolete("touchCount property is deprecated. Please use Input.touchCount instead.")]
		public static extern int touchCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("multiTouchEnabled property is deprecated. Please use Input.multiTouchEnabled instead.")]
		public static extern bool multiTouchEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("accelerationEventCount property is deprecated. Please use Input.accelerationEventCount instead.")]
		public static extern int accelerationEventCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("acceleration property is deprecated. Please use Input.acceleration instead.")]
		public static extern Vector3 acceleration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("orientation property is deprecated. Please use Input.deviceOrientation instead.")]
		public static extern iPhoneOrientation orientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.")]
		public static extern LocationInfo lastLocation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("GetTouch method is deprecated. Please use Input.GetTouch instead.")]
		public static extern iPhoneTouch GetTouch(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("GetAccelerationEvent method is deprecated. Please use Input.GetAccelerationEvent instead.")]
		public static extern iPhoneAccelerationEvent GetAccelerationEvent(int index);
	}
}
