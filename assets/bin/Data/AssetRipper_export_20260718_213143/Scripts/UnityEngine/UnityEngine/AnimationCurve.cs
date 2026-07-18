using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationCurve
	{
		internal IntPtr m_Ptr;

		public Keyframe[] keys
		{
			get
			{
				return GetKeys();
			}
			set
			{
				SetKeys(value);
			}
		}

		public Keyframe this[int index]
		{
			get
			{
				return GetKey_Internal(index);
			}
		}

		public extern int length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern WrapMode preWrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern WrapMode postWrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public AnimationCurve(params Keyframe[] keys)
		{
			Init(keys);
		}

		public AnimationCurve()
		{
			Init(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Cleanup();

		~AnimationCurve()
		{
			Cleanup();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern float Evaluate(float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int AddKey(float time, float value);

		public int AddKey(Keyframe key)
		{
			return AddKey_Internal(key);
		}

		private int AddKey_Internal(Keyframe key)
		{
			return INTERNAL_CALL_AddKey_Internal(this, ref key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int INTERNAL_CALL_AddKey_Internal(AnimationCurve self, ref Keyframe key);

		public int MoveKey(int index, Keyframe key)
		{
			return INTERNAL_CALL_MoveKey(this, index, ref key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int INTERNAL_CALL_MoveKey(AnimationCurve self, int index, ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void RemoveKey(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetKeys(Keyframe[] keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Keyframe GetKey_Internal(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Keyframe[] GetKeys();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SmoothTangents(int index, float weight);

		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			float num = (valueEnd - valueStart) / (timeEnd - timeStart);
			Keyframe[] array = new Keyframe[2]
			{
				new Keyframe(timeStart, valueStart, 0f, num),
				new Keyframe(timeEnd, valueEnd, num, 0f)
			};
			return new AnimationCurve(array);
		}

		public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			Keyframe[] array = new Keyframe[2]
			{
				new Keyframe(timeStart, valueStart, 0f, 0f),
				new Keyframe(timeEnd, valueEnd, 0f, 0f)
			};
			return new AnimationCurve(array);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Init(Keyframe[] keys);
	}
}
