namespace System
{
	[Serializable]
	public struct ArraySegment<T>
	{
		private T[] array;

		private int offset;

		private int count;

		public T[] Array
		{
			get
			{
				return array;
			}
		}

		public int Offset
		{
			get
			{
				return offset;
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		public ArraySegment(T[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (offset > array.Length)
			{
				throw new ArgumentException("out of bounds");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("out of bounds", "offset");
			}
			this.array = array;
			this.offset = offset;
			this.count = count;
		}

		public ArraySegment(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			this.array = array;
			offset = 0;
			count = array.Length;
		}

		public override bool Equals(object obj)
		{
			if (obj is ArraySegment<T>)
			{
				return Equals((ArraySegment<T>)obj);
			}
			return false;
		}

		public bool Equals(ArraySegment<T> obj)
		{
			if (array == obj.Array && offset == obj.Offset && count == obj.Count)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return array.GetHashCode() ^ offset ^ count;
		}

		public static bool operator ==(ArraySegment<T> a, ArraySegment<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ArraySegment<T> a, ArraySegment<T> b)
		{
			return !a.Equals(b);
		}
	}
}
