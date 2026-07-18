using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class BitStream
	{
		internal IntPtr m_Ptr;

		public extern bool isReading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isWriting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serializeb(ref int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serializec(ref char value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serializes(ref short value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serializei(ref int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serializef(ref float value, float maximumDelta);

		private void Serializeq(ref Quaternion value, float maximumDelta)
		{
			INTERNAL_CALL_Serializeq(this, ref value, maximumDelta);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Serializeq(BitStream self, ref Quaternion value, float maximumDelta);

		private void Serializev(ref Vector3 value, float maximumDelta)
		{
			INTERNAL_CALL_Serializev(this, ref value, maximumDelta);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Serializev(BitStream self, ref Vector3 value, float maximumDelta);

		private void Serializen(ref NetworkViewID viewID)
		{
			INTERNAL_CALL_Serializen(this, ref viewID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Serializen(BitStream self, ref NetworkViewID viewID);

		public void Serialize(ref bool value)
		{
			int value2 = (value ? 1 : 0);
			Serializeb(ref value2);
			value = ((value2 != 0) ? true : false);
		}

		public void Serialize(ref char value)
		{
			Serializec(ref value);
		}

		public void Serialize(ref short value)
		{
			Serializes(ref value);
		}

		public void Serialize(ref int value)
		{
			Serializei(ref value);
		}

		[ExcludeFromDocs]
		public void Serialize(ref float value)
		{
			float maxDelta = 1E-05f;
			Serialize(ref value, maxDelta);
		}

		public void Serialize(ref float value, [DefaultValue("0.00001F")] float maxDelta)
		{
			Serializef(ref value, maxDelta);
		}

		[ExcludeFromDocs]
		public void Serialize(ref Quaternion value)
		{
			float maxDelta = 1E-05f;
			Serialize(ref value, maxDelta);
		}

		public void Serialize(ref Quaternion value, [DefaultValue("0.00001F")] float maxDelta)
		{
			Serializeq(ref value, maxDelta);
		}

		[ExcludeFromDocs]
		public void Serialize(ref Vector3 value)
		{
			float maxDelta = 1E-05f;
			Serialize(ref value, maxDelta);
		}

		public void Serialize(ref Vector3 value, [DefaultValue("0.00001F")] float maxDelta)
		{
			Serializev(ref value, maxDelta);
		}

		public void Serialize(ref NetworkPlayer value)
		{
			int value2 = value.index;
			Serializei(ref value2);
			value.index = value2;
		}

		public void Serialize(ref NetworkViewID viewID)
		{
			Serializen(ref viewID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Serialize(ref string value);
	}
}
