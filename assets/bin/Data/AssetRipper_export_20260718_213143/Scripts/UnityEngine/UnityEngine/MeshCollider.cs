using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class MeshCollider : Collider
	{
		[Obsolete("mesh has been replaced with sharedMesh and will be deprecated")]
		public Mesh mesh
		{
			get
			{
				return sharedMesh;
			}
			set
			{
				sharedMesh = value;
			}
		}

		public extern Mesh sharedMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool convex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool smoothSphereCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}
	}
}
