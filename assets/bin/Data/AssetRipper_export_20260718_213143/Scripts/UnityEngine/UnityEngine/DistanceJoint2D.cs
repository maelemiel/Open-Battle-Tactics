using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class DistanceJoint2D : AnchoredJoint2D
	{
		public extern float distance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool maxDistanceOnly
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 value;
			DistanceJoint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void DistanceJoint2D_CUSTOM_INTERNAL_GetReactionForce(DistanceJoint2D joint, float timeStep, out Vector2 value);

		public float GetReactionTorque(float timeStep)
		{
			return INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_GetReactionTorque(DistanceJoint2D self, float timeStep);
	}
}
