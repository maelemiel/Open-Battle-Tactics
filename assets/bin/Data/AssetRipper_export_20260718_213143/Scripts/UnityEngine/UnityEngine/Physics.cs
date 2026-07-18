using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class Physics
	{
		public const int kIgnoreRaycastLayer = 4;

		public const int kDefaultRaycastLayers = -5;

		public const int kAllLayers = -1;

		public const int IgnoreRaycastLayer = 4;

		public const int DefaultRaycastLayers = -5;

		public const int AllLayers = -1;

		public static Vector3 gravity
		{
			get
			{
				Vector3 value;
				INTERNAL_get_gravity(out value);
				return value;
			}
			set
			{
				INTERNAL_set_gravity(ref value);
			}
		}

		public static extern float minPenetrationForPenalty
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern float bounceThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("Please use bounceThreshold instead.")]
		public static float bounceTreshold
		{
			get
			{
				return bounceThreshold;
			}
			set
			{
				bounceThreshold = value;
			}
		}

		public static extern float sleepVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern float sleepAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern float maxAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern int solverIterationCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("penetrationPenaltyForce has no effect.")]
		public static extern float penetrationPenaltyForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_gravity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_gravity(ref Vector3 value);

		private static bool Internal_Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layermask)
		{
			return INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, out hitInfo, distance, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Internal_Raycast(ref Vector3 origin, ref Vector3 direction, out RaycastHit hitInfo, float distance, int layermask);

		private static bool Internal_CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layermask)
		{
			return INTERNAL_CALL_Internal_CapsuleCast(ref point1, ref point2, radius, ref direction, out hitInfo, distance, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Internal_CapsuleCast(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, out RaycastHit hitInfo, float distance, int layermask);

		private static bool Internal_RaycastTest(Vector3 origin, Vector3 direction, float distance, int layermask)
		{
			return INTERNAL_CALL_Internal_RaycastTest(ref origin, ref direction, distance, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Internal_RaycastTest(ref Vector3 origin, ref Vector3 direction, float distance, int layermask);

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, float distance)
		{
			int layerMask = -5;
			return Raycast(origin, direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Raycast(origin, direction, distance, layerMask);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Internal_RaycastTest(origin, direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance)
		{
			int layerMask = -5;
			return Raycast(origin, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Raycast(origin, direction, out hitInfo, distance, layerMask);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Internal_Raycast(origin, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, float distance)
		{
			int layerMask = -5;
			return Raycast(ray, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Raycast(ray, distance, layerMask);
		}

		public static bool Raycast(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Raycast(ray.origin, ray.direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
		{
			int layerMask = -5;
			return Raycast(ray, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Raycast(Ray ray, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return Raycast(ray, out hitInfo, distance, layerMask);
		}

		public static bool Raycast(Ray ray, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Raycast(ray.origin, ray.direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray, float distance)
		{
			int layerMask = -5;
			return RaycastAll(ray, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Ray ray)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return RaycastAll(ray, distance, layerMask);
		}

		public static RaycastHit[] RaycastAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return RaycastAll(ray.origin, ray.direction, distance, layerMask);
		}

		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layermask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance)
		{
			int layermask = -5;
			return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layermask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
		{
			int layermask = -5;
			float distance = float.PositiveInfinity;
			return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern RaycastHit[] INTERNAL_CALL_RaycastAll(ref Vector3 origin, ref Vector3 direction, float distance, int layermask);

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end)
		{
			int layerMask = -5;
			return Linecast(start, end, layerMask);
		}

		public static bool Linecast(Vector3 start, Vector3 end, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			Vector3 direction = end - start;
			return Raycast(start, direction, direction.magnitude, layerMask);
		}

		[ExcludeFromDocs]
		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			return Linecast(start, end, out hitInfo, layerMask);
		}

		public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			Vector3 direction = end - start;
			return Raycast(start, direction, out hitInfo, direction.magnitude, layerMask);
		}

		public static Collider[] OverlapSphere(Vector3 position, float radius, [DefaultValue("AllLayers")] int layerMask)
		{
			return INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask);
		}

		[ExcludeFromDocs]
		public static Collider[] OverlapSphere(Vector3 position, float radius)
		{
			int layerMask = -1;
			return INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Collider[] INTERNAL_CALL_OverlapSphere(ref Vector3 position, float radius, int layerMask);

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance)
		{
			int layerMask = -5;
			return CapsuleCast(point1, point2, radius, direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return CapsuleCast(point1, point2, radius, direction, distance, layerMask);
		}

		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			RaycastHit hitInfo;
			return Internal_CapsuleCast(point1, point2, radius, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance)
		{
			int layerMask = -5;
			return CapsuleCast(point1, point2, radius, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return CapsuleCast(point1, point2, radius, direction, out hitInfo, distance, layerMask);
		}

		public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Internal_CapsuleCast(point1, point2, radius, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance)
		{
			int layerMask = -5;
			return SphereCast(origin, radius, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return SphereCast(origin, radius, direction, out hitInfo, distance, layerMask);
		}

		public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Internal_CapsuleCast(origin, origin, radius, direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, float distance)
		{
			int layerMask = -5;
			return SphereCast(ray, radius, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return SphereCast(ray, radius, distance, layerMask);
		}

		public static bool SphereCast(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			RaycastHit hitInfo;
			return Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance)
		{
			int layerMask = -5;
			return SphereCast(ray, radius, out hitInfo, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return SphereCast(ray, radius, out hitInfo, distance, layerMask);
		}

		public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out hitInfo, distance, layerMask);
		}

		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, distance, layermask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance)
		{
			int layermask = -5;
			return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, distance, layermask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
		{
			int layermask = -5;
			float distance = float.PositiveInfinity;
			return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, distance, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern RaycastHit[] INTERNAL_CALL_CapsuleCastAll(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, float distance, int layermask);

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance)
		{
			int layerMask = -5;
			return SphereCastAll(origin, radius, direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return SphereCastAll(origin, radius, direction, distance, layerMask);
		}

		public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return CapsuleCastAll(origin, origin, radius, direction, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius, float distance)
		{
			int layerMask = -5;
			return SphereCastAll(ray, radius, distance, layerMask);
		}

		[ExcludeFromDocs]
		public static RaycastHit[] SphereCastAll(Ray ray, float radius)
		{
			int layerMask = -5;
			float distance = float.PositiveInfinity;
			return SphereCastAll(ray, radius, distance, layerMask);
		}

		public static RaycastHit[] SphereCastAll(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return CapsuleCastAll(ray.origin, ray.origin, radius, ray.direction, distance, layerMask);
		}

		public static bool CheckSphere(Vector3 position, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask)
		{
			return INTERNAL_CALL_CheckSphere(ref position, radius, layerMask);
		}

		[ExcludeFromDocs]
		public static bool CheckSphere(Vector3 position, float radius)
		{
			int layerMask = -5;
			return INTERNAL_CALL_CheckSphere(ref position, radius, layerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_CheckSphere(ref Vector3 position, float radius, int layerMask);

		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, [DefaultValue("DefaultRaycastLayers")] int layermask)
		{
			return INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask);
		}

		[ExcludeFromDocs]
		public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
		{
			int layermask = -5;
			return INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_CheckCapsule(ref Vector3 start, ref Vector3 end, float radius, int layermask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void IgnoreCollision(Collider collider1, Collider collider2, [DefaultValue("true")] bool ignore);

		[ExcludeFromDocs]
		public static void IgnoreCollision(Collider collider1, Collider collider2)
		{
			bool ignore = true;
			IgnoreCollision(collider1, collider2, ignore);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);

		[ExcludeFromDocs]
		public static void IgnoreLayerCollision(int layer1, int layer2)
		{
			bool ignore = true;
			IgnoreLayerCollision(layer1, layer2, ignore);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);
	}
}
