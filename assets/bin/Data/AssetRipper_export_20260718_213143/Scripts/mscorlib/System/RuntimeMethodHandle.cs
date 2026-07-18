using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("Serialization needs tests")]
	public struct RuntimeMethodHandle : ISerializable
	{
		private IntPtr value;

		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		internal RuntimeMethodHandle(IntPtr v)
		{
			value = v;
		}

		private RuntimeMethodHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoMethod monoMethod = (MonoMethod)info.GetValue("MethodObj", typeof(MonoMethod));
			value = monoMethod.MethodHandle.Value;
			if (value == IntPtr.Zero)
			{
				throw new SerializationException(Locale.GetText("Insufficient state."));
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (value == IntPtr.Zero)
			{
				throw new SerializationException("Object fields may not be properly initialized");
			}
			info.AddValue("MethodObj", (MonoMethod)MethodBase.GetMethodFromHandle(this), typeof(MonoMethod));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFunctionPointer(IntPtr m);

		public IntPtr GetFunctionPointer()
		{
			return GetFunctionPointer(value);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return value == ((RuntimeMethodHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeMethodHandle handle)
		{
			return value == handle.Value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right)
		{
			return !left.Equals(right);
		}
	}
}
