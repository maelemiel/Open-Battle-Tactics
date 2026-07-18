using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("Serialization needs tests")]
	public struct RuntimeFieldHandle : ISerializable
	{
		private IntPtr value;

		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		internal RuntimeFieldHandle(IntPtr v)
		{
			value = v;
		}

		private RuntimeFieldHandle(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MonoField monoField = (MonoField)info.GetValue("FieldObj", typeof(MonoField));
			value = monoField.FieldHandle.Value;
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
			info.AddValue("FieldObj", (MonoField)FieldInfo.GetFieldFromHandle(this), typeof(MonoField));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return value == ((RuntimeFieldHandle)obj).Value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals(RuntimeFieldHandle handle)
		{
			return value == handle.Value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static bool operator ==(RuntimeFieldHandle left, RuntimeFieldHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RuntimeFieldHandle left, RuntimeFieldHandle right)
		{
			return !left.Equals(right);
		}
	}
}
