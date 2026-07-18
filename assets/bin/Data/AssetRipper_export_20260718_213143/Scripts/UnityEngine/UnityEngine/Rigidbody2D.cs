using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Rigidbody2D : Component
	{
		public Vector2 position
		{
			get
			{
				Vector2 value;
				INTERNAL_get_position(out value);
				return value;
			}
			set
			{
				INTERNAL_set_position(ref value);
			}
		}

		public extern float rotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector2 velocity
		{
			get
			{
				Vector2 value;
				INTERNAL_get_velocity(out value);
				return value;
			}
			set
			{
				INTERNAL_set_velocity(ref value);
			}
		}

		public extern float angularVelocity
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

		public Vector2 centerOfMass
		{
			get
			{
				Vector2 value;
				INTERNAL_get_centerOfMass(out value);
				return value;
			}
			set
			{
				INTERNAL_set_centerOfMass(ref value);
			}
		}

		public Vector2 worldCenterOfMass
		{
			get
			{
				Vector2 value;
				INTERNAL_get_worldCenterOfMass(out value);
				return value;
			}
		}

		public extern float inertia
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
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

		public extern float gravityScale
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

		public extern bool fixedAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool simulated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RigidbodyInterpolation2D interpolation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RigidbodySleepMode2D sleepMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern CollisionDetectionMode2D collisionDetectionMode
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
		private extern void INTERNAL_get_position(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_position(ref Vector2 value);

		public void MovePosition(Vector2 position)
		{
			INTERNAL_CALL_MovePosition(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MovePosition(Rigidbody2D self, ref Vector2 position);

		public void MoveRotation(float angle)
		{
			INTERNAL_CALL_MoveRotation(this, angle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MoveRotation(Rigidbody2D self, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_velocity(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_velocity(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_centerOfMass(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_centerOfMass(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_worldCenterOfMass(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool IsSleeping();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool IsAwake();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Sleep();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void WakeUp();

		public void AddForce(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector2 force)
		{
			ForceMode2D mode = ForceMode2D.Force;
			INTERNAL_CALL_AddForce(this, ref force, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddForce(Rigidbody2D self, ref Vector2 force, ForceMode2D mode);

		public void AddRelativeForce(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector2 relativeForce)
		{
			ForceMode2D mode = ForceMode2D.Force;
			INTERNAL_CALL_AddRelativeForce(this, ref relativeForce, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody2D self, ref Vector2 relativeForce, ForceMode2D mode);

		public void AddForceAtPosition(Vector2 force, Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector2 force, Vector2 position)
		{
			ForceMode2D mode = ForceMode2D.Force;
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody2D self, ref Vector2 force, ref Vector2 position, ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void AddTorque(float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[ExcludeFromDocs]
		public void AddTorque(float torque)
		{
			ForceMode2D mode = ForceMode2D.Force;
			AddTorque(torque, mode);
		}

		public Vector2 GetPoint(Vector2 point)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetPoint(this, point, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(rigidbody, ref point, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPoint(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);

		public Vector2 GetRelativePoint(Vector2 relativePoint)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(this, relativePoint, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(rigidbody, ref relativePoint, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePoint(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);

		public Vector2 GetVector(Vector2 vector)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetVector(this, vector, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, Vector2 vector, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(rigidbody, ref vector, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetVector(Rigidbody2D rigidbody, ref Vector2 vector, out Vector2 value);

		public Vector2 GetRelativeVector(Vector2 relativeVector)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(this, relativeVector, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, Vector2 relativeVector, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(rigidbody, ref relativeVector, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativeVector(Rigidbody2D rigidbody, ref Vector2 relativeVector, out Vector2 value);

		public Vector2 GetPointVelocity(Vector2 point)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(this, point, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, Vector2 point, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(rigidbody, ref point, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetPointVelocity(Rigidbody2D rigidbody, ref Vector2 point, out Vector2 value);

		public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
		{
			Vector2 value;
			Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(this, relativePoint, out value);
			return value;
		}

		private static void Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, Vector2 relativePoint, out Vector2 value)
		{
			INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(rigidbody, ref relativePoint, out value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Rigidbody2D_CUSTOM_INTERNAL_GetRelativePointVelocity(Rigidbody2D rigidbody, ref Vector2 relativePoint, out Vector2 value);
	}
}
