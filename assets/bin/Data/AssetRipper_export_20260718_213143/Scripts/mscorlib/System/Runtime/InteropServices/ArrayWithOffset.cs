namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public struct ArrayWithOffset
	{
		private object array;

		private int offset;

		public ArrayWithOffset(object array, int offset)
		{
			this.array = array;
			this.offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ArrayWithOffset))
			{
				return false;
			}
			ArrayWithOffset arrayWithOffset = (ArrayWithOffset)obj;
			return arrayWithOffset.array == array && arrayWithOffset.offset == offset;
		}

		public bool Equals(ArrayWithOffset obj)
		{
			return obj.array == array && obj.offset == offset;
		}

		public override int GetHashCode()
		{
			return offset;
		}

		public object GetArray()
		{
			return array;
		}

		public int GetOffset()
		{
			return offset;
		}

		public static bool operator ==(ArrayWithOffset a, ArrayWithOffset b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ArrayWithOffset a, ArrayWithOffset b)
		{
			return !a.Equals(b);
		}
	}
}
