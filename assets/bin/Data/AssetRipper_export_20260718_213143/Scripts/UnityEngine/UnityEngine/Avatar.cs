using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Avatar : Object
	{
		public extern bool isValid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isHuman
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern void SetMuscleMinMax(int muscleId, float min, float max);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern void SetParameter(int parameterId, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern float GetAxisLength(int humanId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern Quaternion GetPreRotation(int humanId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern Quaternion GetPostRotation(int humanId);

		internal Quaternion GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
		{
			return INTERNAL_CALL_GetZYPostQ(this, humanId, ref parentQ, ref q);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Quaternion INTERNAL_CALL_GetZYPostQ(Avatar self, int humanId, ref Quaternion parentQ, ref Quaternion q);

		internal Quaternion GetZYRoll(int humanId, Vector3 uvw)
		{
			return INTERNAL_CALL_GetZYRoll(this, humanId, ref uvw);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Quaternion INTERNAL_CALL_GetZYRoll(Avatar self, int humanId, ref Vector3 uvw);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern Vector3 GetLimitSign(int humanId);
	}
}
