using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Input
	{
		private static Gyroscope m_MainGyro;

		private static LocationService locationServiceInstance;

		private static Compass compassInstance;

		public static extern bool compensateSensors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("isGyroAvailable property is deprecated. Please use SystemInfo.supportsGyroscope instead.")]
		public static extern bool isGyroAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static Gyroscope gyro
		{
			get
			{
				if (m_MainGyro == null)
				{
					m_MainGyro = new Gyroscope(mainGyroIndex_Internal());
				}
				return m_MainGyro;
			}
		}

		public static extern Vector3 mousePosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool mousePresent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool simulateMouseWithTouches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool anyKey
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool anyKeyDown
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string inputString
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern Vector3 acceleration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static AccelerationEvent[] accelerationEvents
		{
			get
			{
				int num = accelerationEventCount;
				AccelerationEvent[] array = new AccelerationEvent[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = GetAccelerationEvent(i);
				}
				return array;
			}
		}

		public static extern int accelerationEventCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static Touch[] touches
		{
			get
			{
				int num = touchCount;
				Touch[] array = new Touch[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = GetTouch(i);
				}
				return array;
			}
		}

		public static extern int touchCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("eatKeyPressOnTextFieldFocus property is deprecated, and only provided to support legacy behavior.")]
		public static extern bool eatKeyPressOnTextFieldFocus
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool multiTouchEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static LocationService location
		{
			get
			{
				if (locationServiceInstance == null)
				{
					locationServiceInstance = new LocationService();
				}
				return locationServiceInstance;
			}
		}

		public static Compass compass
		{
			get
			{
				if (compassInstance == null)
				{
					compassInstance = new Compass();
				}
				return compassInstance;
			}
		}

		public static extern DeviceOrientation deviceOrientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern IMECompositionMode imeCompositionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern string compositionString
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool imeIsSelected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static Vector2 compositionCursorPos
		{
			get
			{
				Vector2 value;
				INTERNAL_get_compositionCursorPos(out value);
				return value;
			}
			set
			{
				INTERNAL_set_compositionCursorPos(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int mainGyroIndex_Internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyInt(int key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyUpInt(int key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyUpString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyDownInt(int key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool GetKeyDownString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern float GetAxis(string axisName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern float GetAxisRaw(string axisName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetButton(string buttonName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetButtonDown(string buttonName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetButtonUp(string buttonName);

		public static bool GetKey(string name)
		{
			return GetKeyString(name);
		}

		public static bool GetKey(KeyCode key)
		{
			return GetKeyInt((int)key);
		}

		public static bool GetKeyDown(string name)
		{
			return GetKeyDownString(name);
		}

		public static bool GetKeyDown(KeyCode key)
		{
			return GetKeyDownInt((int)key);
		}

		public static bool GetKeyUp(string name)
		{
			return GetKeyUpString(name);
		}

		public static bool GetKeyUp(KeyCode key)
		{
			return GetKeyUpInt((int)key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern string[] GetJoystickNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetMouseButton(int button);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetMouseButtonDown(int button);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetMouseButtonUp(int button);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void ResetInputAxes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AccelerationEvent GetAccelerationEvent(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Touch GetTouch(int index);

		[Obsolete("Use ps3 move API instead", true)]
		public static Quaternion GetRotation(int deviceID)
		{
			return Quaternion.identity;
		}

		[Obsolete("Use ps3 move API instead", true)]
		public static Vector3 GetPosition(int deviceID)
		{
			return Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_compositionCursorPos(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_compositionCursorPos(ref Vector2 value);
	}
}
