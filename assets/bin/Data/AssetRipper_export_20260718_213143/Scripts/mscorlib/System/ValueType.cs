using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class ValueType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalEquals(object o1, object o2, out object[] fields);

		internal static bool DefaultEquals(object o1, object o2)
		{
			if (o2 == null)
			{
				return false;
			}
			object[] fields;
			bool result = InternalEquals(o1, o2, out fields);
			if (fields == null)
			{
				return result;
			}
			for (int i = 0; i < fields.Length; i += 2)
			{
				object obj = fields[i];
				object obj2 = fields[i + 1];
				if (obj == null)
				{
					if (obj2 != null)
					{
						return false;
					}
				}
				else if (!obj.Equals(obj2))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return DefaultEquals(this, obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetHashCode(object o, out object[] fields);

		public override int GetHashCode()
		{
			object[] fields;
			int num = InternalGetHashCode(this, out fields);
			if (fields != null)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					if (fields[i] != null)
					{
						num ^= fields[i].GetHashCode();
					}
				}
			}
			return num;
		}

		public override string ToString()
		{
			return GetType().FullName;
		}
	}
}
