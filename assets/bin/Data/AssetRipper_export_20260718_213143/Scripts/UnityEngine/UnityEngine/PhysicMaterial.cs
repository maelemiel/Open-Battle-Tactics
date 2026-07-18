using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class PhysicMaterial : Object
	{
		public extern float dynamicFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float staticFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float bounciness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("Use PhysicMaterial.bounciness instead", true)]
		public float bouncyness
		{
			get
			{
				return bounciness;
			}
			set
			{
				bounciness = value;
			}
		}

		public Vector3 frictionDirection2
		{
			get
			{
				Vector3 value;
				INTERNAL_get_frictionDirection2(out value);
				return value;
			}
			set
			{
				INTERNAL_set_frictionDirection2(ref value);
			}
		}

		public extern float dynamicFriction2
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float staticFriction2
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern PhysicMaterialCombine frictionCombine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern PhysicMaterialCombine bounceCombine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("use PhysicMaterial.frictionDirection2 instead.")]
		public Vector3 frictionDirection
		{
			get
			{
				return frictionDirection2;
			}
			set
			{
				frictionDirection2 = value;
			}
		}

		public PhysicMaterial()
		{
			Internal_CreateDynamicsMaterial(this, null);
		}

		public PhysicMaterial(string name)
		{
			Internal_CreateDynamicsMaterial(this, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateDynamicsMaterial([Writable] PhysicMaterial mat, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_frictionDirection2(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_frictionDirection2(ref Vector3 value);
	}
}
