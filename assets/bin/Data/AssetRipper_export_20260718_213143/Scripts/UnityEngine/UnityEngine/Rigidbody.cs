using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Rigidbody : Component
	{
		public Vector3 velocity
		{
			get
			{
				Vector3 value;
				INTERNAL_get_velocity(out value);
				return value;
			}
			set
			{
				INTERNAL_set_velocity(ref value);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				Vector3 value;
				INTERNAL_get_angularVelocity(out value);
				return value;
			}
			set
			{
				INTERNAL_set_angularVelocity(ref value);
			}
		}

		public extern float drag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float angularDrag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool useGravity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isKinematic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool freezeRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RigidbodyConstraints constraints
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern CollisionDetectionMode collisionDetectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector3 centerOfMass
		{
			get
			{
				Vector3 value;
				INTERNAL_get_centerOfMass(out value);
				return value;
			}
			set
			{
				INTERNAL_set_centerOfMass(ref value);
			}
		}

		public Vector3 worldCenterOfMass
		{
			get
			{
				Vector3 value;
				INTERNAL_get_worldCenterOfMass(out value);
				return value;
			}
		}

		public Quaternion inertiaTensorRotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_inertiaTensorRotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_inertiaTensorRotation(ref value);
			}
		}

		public Vector3 inertiaTensor
		{
			get
			{
				Vector3 value;
				INTERNAL_get_inertiaTensor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_inertiaTensor(ref value);
			}
		}

		public extern bool detectCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool useConeFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector3 position
		{
			get
			{
				Vector3 value;
				INTERNAL_get_position(out value);
				return value;
			}
			set
			{
				INTERNAL_set_position(ref value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_rotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rotation(ref value);
			}
		}

		public extern RigidbodyInterpolation interpolation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int solverIterationCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float sleepVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float sleepAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float maxAngularVelocity
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
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_velocity(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_angularVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_angularVelocity(ref Vector3 value);

		public void SetDensity(float density)
		{
			INTERNAL_CALL_SetDensity(this, density);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetDensity(Rigidbody self, float density);

		public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector3 force)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddForce(Rigidbody self, ref Vector3 force, ForceMode mode);

		[ExcludeFromDocs]
		public void AddForce(float x, float y, float z)
		{
			ForceMode mode = ForceMode.Force;
			AddForce(x, y, z, mode);
		}

		public void AddForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			AddForce(new Vector3(x, y, z), mode);
		}

		public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector3 force)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody self, ref Vector3 force, ForceMode mode);

		[ExcludeFromDocs]
		public void AddRelativeForce(float x, float y, float z)
		{
			ForceMode mode = ForceMode.Force;
			AddRelativeForce(x, y, z, mode);
		}

		public void AddRelativeForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			AddRelativeForce(new Vector3(x, y, z), mode);
		}

		public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddTorque(this, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(Vector3 torque)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddTorque(this, ref torque, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);

		[ExcludeFromDocs]
		public void AddTorque(float x, float y, float z)
		{
			ForceMode mode = ForceMode.Force;
			AddTorque(x, y, z, mode);
		}

		public void AddTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			AddTorque(new Vector3(x, y, z), mode);
		}

		public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddRelativeTorque(this, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeTorque(Vector3 torque)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddRelativeTorque(this, ref torque, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddRelativeTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);

		[ExcludeFromDocs]
		public void AddRelativeTorque(float x, float y, float z)
		{
			ForceMode mode = ForceMode.Force;
			AddRelativeTorque(x, y, z, mode);
		}

		public void AddRelativeTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			AddRelativeTorque(new Vector3(x, y, z), mode);
		}

		public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody self, ref Vector3 force, ref Vector3 position, ForceMode mode);

		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0F")] float upwardsModifier, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
		{
			ForceMode mode = ForceMode.Force;
			float upwardsModifier = 0f;
			INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddExplosionForce(Rigidbody self, float explosionForce, ref Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode);

		public Vector3 ClosestPointOnBounds(Vector3 position)
		{
			return INTERNAL_CALL_ClosestPointOnBounds(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_ClosestPointOnBounds(Rigidbody self, ref Vector3 position);

		public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
		{
			return INTERNAL_CALL_GetRelativePointVelocity(this, ref relativePoint);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_GetRelativePointVelocity(Rigidbody self, ref Vector3 relativePoint);

		public Vector3 GetPointVelocity(Vector3 worldPoint)
		{
			return INTERNAL_CALL_GetPointVelocity(this, ref worldPoint);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_GetPointVelocity(Rigidbody self, ref Vector3 worldPoint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_centerOfMass(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_centerOfMass(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_worldCenterOfMass(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_inertiaTensorRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_inertiaTensorRotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_inertiaTensor(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_inertiaTensor(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_position(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_position(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rotation(ref Quaternion value);

		public void MovePosition(Vector3 position)
		{
			INTERNAL_CALL_MovePosition(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MovePosition(Rigidbody self, ref Vector3 position);

		public void MoveRotation(Quaternion rot)
		{
			INTERNAL_CALL_MoveRotation(this, ref rot);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MoveRotation(Rigidbody self, ref Quaternion rot);

		public void Sleep()
		{
			INTERNAL_CALL_Sleep(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Sleep(Rigidbody self);

		public bool IsSleeping()
		{
			return INTERNAL_CALL_IsSleeping(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_IsSleeping(Rigidbody self);

		public void WakeUp()
		{
			INTERNAL_CALL_WakeUp(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_WakeUp(Rigidbody self);

		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, distance);
		}

		[ExcludeFromDocs]
		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo)
		{
			float distance = float.PositiveInfinity;
			return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, distance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_SweepTest(Rigidbody self, ref Vector3 direction, out RaycastHit hitInfo, float distance);

		public RaycastHit[] SweepTestAll(Vector3 direction, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return INTERNAL_CALL_SweepTestAll(this, ref direction, distance);
		}

		[ExcludeFromDocs]
		public RaycastHit[] SweepTestAll(Vector3 direction)
		{
			float distance = float.PositiveInfinity;
			return INTERNAL_CALL_SweepTestAll(this, ref direction, distance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern RaycastHit[] INTERNAL_CALL_SweepTestAll(Rigidbody self, ref Vector3 direction, float distance);

		[Obsolete("use Rigidbody.maxAngularVelocity instead.")]
		public void SetMaxAngularVelocity(float a)
		{
			maxAngularVelocity = a;
		}
	}
}
