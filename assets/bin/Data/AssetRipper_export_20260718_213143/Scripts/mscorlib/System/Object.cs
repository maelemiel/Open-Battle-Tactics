using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class Object
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public Object()
		{
		}

		public virtual bool Equals(object obj)
		{
			return this == obj;
		}

		public static bool Equals(object objA, object objB)
		{
			if (objA == objB)
			{
				return true;
			}
			if (objA == null || objB == null)
			{
				return false;
			}
			return objA.Equals(objB);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		virtual ~Object()
		{
		}

		public virtual int GetHashCode()
		{
			return InternalGetHashCode(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern object MemberwiseClone();

		public virtual string ToString()
		{
			return GetType().ToString();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool ReferenceEquals(object objA, object objB)
		{
			return objA == objB;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetHashCode(object o);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr obj_address();

		private void FieldGetter(string typeName, string fieldName, ref object val)
		{
		}

		private void FieldSetter(string typeName, string fieldName, object val)
		{
		}
	}
}
