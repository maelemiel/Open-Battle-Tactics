using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class InteractiveCloth : Cloth
	{
		public extern Mesh mesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float density
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float pressure
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float collisionResponse
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float tearFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float attachmentTearFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float attachmentResponse
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isTeared
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, radius, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius)
		{
			ForceMode mode = ForceMode.Force;
			INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, radius, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_AddForceAtPosition(InteractiveCloth self, ref Vector3 force, ref Vector3 position, float radius, ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void AttachToCollider(Collider collider, [DefaultValue("false")] bool tearable, [DefaultValue("false")] bool twoWayInteraction);

		[ExcludeFromDocs]
		public void AttachToCollider(Collider collider, bool tearable)
		{
			bool twoWayInteraction = false;
			AttachToCollider(collider, tearable, twoWayInteraction);
		}

		[ExcludeFromDocs]
		public void AttachToCollider(Collider collider)
		{
			bool twoWayInteraction = false;
			bool tearable = false;
			AttachToCollider(collider, tearable, twoWayInteraction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void DetachFromCollider(Collider collider);
	}
}
