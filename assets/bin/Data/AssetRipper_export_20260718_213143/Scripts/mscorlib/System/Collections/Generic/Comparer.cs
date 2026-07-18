namespace System.Collections.Generic
{
	[Serializable]
	public abstract class Comparer<T> : IComparer<T>, IComparer
	{
		private sealed class DefaultComparer : Comparer<T>
		{
			public override int Compare(T x, T y)
			{
				if (x == null)
				{
					return (y != null) ? (-1) : 0;
				}
				if (y == null)
				{
					return 1;
				}
				if (x is IComparable<T>)
				{
					return ((IComparable<T>)(object)x).CompareTo(y);
				}
				if (x is IComparable)
				{
					return ((IComparable)(object)x).CompareTo(y);
				}
				throw new ArgumentException("does not implement right interface");
			}
		}

		private static readonly Comparer<T> _default;

		public static Comparer<T> Default
		{
			get
			{
				return _default;
			}
		}

		static Comparer()
		{
			if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
			{
				_default = (Comparer<T>)Activator.CreateInstance(typeof(GenericComparer<>).MakeGenericType(typeof(T)));
			}
			else
			{
				_default = new DefaultComparer();
			}
		}

		int IComparer.Compare(object x, object y)
		{
			if (x == null)
			{
				return (y != null) ? (-1) : 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x is T && y is T)
			{
				return Compare((T)x, (T)y);
			}
			throw new ArgumentException();
		}

		public abstract int Compare(T x, T y);
	}
}
