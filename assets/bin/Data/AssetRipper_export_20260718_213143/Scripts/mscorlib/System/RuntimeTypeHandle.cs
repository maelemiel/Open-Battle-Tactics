using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[MonoTODO("Serialization needs tests")]
	[ComVisible(true)]
	public struct RuntimeTypeHandle : ISerializable
	{
		private IntPtr value;

		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		internal RuntimeTypeHandle(IntPtr val)
		{
			value = val;
		}

		private RuntimeTypeHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoType monoType = (MonoType)info.GetValue("TypeObj", typeof(MonoType));
			value = monoType.TypeHandle.Value;
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
			info.AddValue("TypeObj", Type.GetTypeHandle(this), typeof(MonoType));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return value == ((RuntimeTypeHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeTypeHandle handle)
		{
			return value == handle.Value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public ModuleHandle GetModuleHandle()
		{
			if (value == IntPtr.Zero)
			{
				throw new InvalidOperationException("Object fields may not be properly initialized");
			}
			return Type.GetTypeFromHandle(this).Module.ModuleHandle;
		}

		public static bool operator ==(RuntimeTypeHandle left, object right)
		{
			return right != null && right is RuntimeTypeHandle && left.Equals((RuntimeTypeHandle)right);
		}

		public static bool operator !=(RuntimeTypeHandle left, object right)
		{
			return right == null || !(right is RuntimeTypeHandle) || !left.Equals((RuntimeTypeHandle)right);
		}

		public static bool operator ==(object left, RuntimeTypeHandle right)
		{
			return left != null && left is RuntimeTypeHandle && ((RuntimeTypeHandle)left).Equals(right);
		}

		public static bool operator !=(object left, RuntimeTypeHandle right)
		{
			return left == null || !(left is RuntimeTypeHandle) || !((RuntimeTypeHandle)left).Equals(right);
		}
	}
}
