using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with.NET")]
	[ComVisible(true)]
	public class ObjectIDGenerator
	{
		private class InstanceComparer : IComparer, IHashCodeProvider
		{
			int IComparer.Compare(object o1, object o2)
			{
				if (o1 is string)
				{
					return (!o1.Equals(o2)) ? 1 : 0;
				}
				return (o1 != o2) ? 1 : 0;
			}

			int IHashCodeProvider.GetHashCode(object o)
			{
				return object.InternalGetHashCode(o);
			}
		}

		private Hashtable table;

		private long current;

		private static InstanceComparer comparer = new InstanceComparer();

		internal long NextId
		{
			get
			{
				return current++;
			}
		}

		public ObjectIDGenerator()
		{
			table = new Hashtable(comparer, comparer);
			current = 1L;
		}

		public virtual long GetId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			object obj2 = table[obj];
			if (obj2 != null)
			{
				firstTime = false;
				return (long)obj2;
			}
			firstTime = true;
			table.Add(obj, current);
			return current++;
		}

		public virtual long HasId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			object obj2 = table[obj];
			if (obj2 != null)
			{
				firstTime = false;
				return (long)obj2;
			}
			firstTime = true;
			return 0L;
		}
	}
}
