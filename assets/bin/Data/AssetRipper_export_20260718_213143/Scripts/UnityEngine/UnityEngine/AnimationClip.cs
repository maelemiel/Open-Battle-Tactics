using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AnimationClip : Motion
	{
		public extern float length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal extern float startTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal extern float stopTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float frameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern WrapMode wrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Bounds localBounds
		{
			get
			{
				Bounds value;
				INTERNAL_get_localBounds(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localBounds(ref value);
			}
		}

		public AnimationClip()
		{
			Internal_CreateAnimationClip(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetCurve(string relativePath, Type type, string propertyName, AnimationCurve curve);

		public void EnsureQuaternionContinuity()
		{
			INTERNAL_CALL_EnsureQuaternionContinuity(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_EnsureQuaternionContinuity(AnimationClip self);

		public void ClearCurves()
		{
			INTERNAL_CALL_ClearCurves(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ClearCurves(AnimationClip self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void AddEvent(AnimationEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localBounds(out Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localBounds(ref Bounds value);
	}
}
