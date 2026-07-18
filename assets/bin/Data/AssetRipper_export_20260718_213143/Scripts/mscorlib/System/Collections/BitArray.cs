using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public sealed class BitArray : IEnumerable, ICloneable, ICollection
	{
		[Serializable]
		private class BitArrayEnumerator : IEnumerator, ICloneable
		{
			private BitArray _bitArray;

			private bool _current;

			private int _index;

			private int _version;

			public object Current
			{
				get
				{
					if (_index == -1)
					{
						throw new InvalidOperationException("Enum not started");
					}
					if (_index >= _bitArray.Count)
					{
						throw new InvalidOperationException("Enum Ended");
					}
					return _current;
				}
			}

			public BitArrayEnumerator(BitArray ba)
			{
				_index = -1;
				_bitArray = ba;
				_version = ba._version;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				checkVersion();
				if (_index < _bitArray.Count - 1)
				{
					_current = _bitArray[++_index];
					return true;
				}
				_index = _bitArray.Count;
				return false;
			}

			public void Reset()
			{
				checkVersion();
				_index = -1;
			}

			private void checkVersion()
			{
				if (_version != _bitArray._version)
				{
					throw new InvalidOperationException();
				}
			}
		}

		private int[] m_array;

		private int m_length;

		private int _version;

		public int Count
		{
			get
			{
				return m_length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public bool this[int index]
		{
			get
			{
				return Get(index);
			}
			set
			{
				Set(index, value);
			}
		}

		public int Length
		{
			get
			{
				return m_length;
			}
			set
			{
				if (m_length == value)
				{
					return;
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value > m_length)
				{
					int num = (value + 31) / 32;
					int num2 = (m_length + 31) / 32;
					if (num > m_array.Length)
					{
						int[] array = new int[num];
						Array.Copy(m_array, array, m_array.Length);
						m_array = array;
					}
					else
					{
						Array.Clear(m_array, num2, num - num2);
					}
					int num3 = m_length % 32;
					if (num3 > 0)
					{
						m_array[num2 - 1] &= (1 << num3) - 1;
					}
				}
				m_length = value;
				_version++;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public BitArray(BitArray bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException("bits");
			}
			m_length = bits.m_length;
			m_array = new int[(m_length + 31) / 32];
			if (m_array.Length == 1)
			{
				m_array[0] = bits.m_array[0];
			}
			else
			{
				Array.Copy(bits.m_array, m_array, m_array.Length);
			}
		}

		public BitArray(bool[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			m_length = values.Length;
			m_array = new int[(m_length + 31) / 32];
			for (int i = 0; i < values.Length; i++)
			{
				this[i] = values[i];
			}
		}

		public BitArray(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			m_length = bytes.Length * 8;
			m_array = new int[(m_length + 31) / 32];
			for (int i = 0; i < bytes.Length; i++)
			{
				setByte(i, bytes[i]);
			}
		}

		public BitArray(int[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			m_length = num * 32;
			m_array = new int[num];
			Array.Copy(values, m_array, num);
		}

		public BitArray(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			m_length = length;
			m_array = new int[(m_length + 31) / 32];
		}

		public BitArray(int length, bool defaultValue)
			: this(length)
		{
			if (defaultValue)
			{
				for (int i = 0; i < m_array.Length; i++)
				{
					m_array[i] = -1;
				}
			}
		}

		private BitArray(int[] array, int length)
		{
			m_array = array;
			m_length = length;
		}

		private byte getByte(int byteIndex)
		{
			int num = byteIndex / 4;
			int num2 = byteIndex % 4 * 8;
			int num3 = m_array[num] & (255 << num2);
			return (byte)((num3 >> num2) & 0xFF);
		}

		private void setByte(int byteIndex, byte value)
		{
			int num = byteIndex / 4;
			int num2 = byteIndex % 4 * 8;
			m_array[num] &= ~(255 << num2);
			m_array[num] |= value << num2;
			_version++;
		}

		private void checkOperand(BitArray operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException();
			}
			if (operand.m_length != m_length)
			{
				throw new ArgumentException();
			}
		}

		public object Clone()
		{
			return new BitArray(this);
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("array", "Array rank must be 1");
			}
			if (index >= array.Length && m_length > 0)
			{
				throw new ArgumentException("index", "index is greater than array.Length");
			}
			if (array is bool[])
			{
				if (array.Length - index < m_length)
				{
					throw new ArgumentException();
				}
				bool[] array2 = (bool[])array;
				for (int i = 0; i < m_length; i++)
				{
					array2[index + i] = this[i];
				}
			}
			else if (array is byte[])
			{
				int num = (m_length + 7) / 8;
				if (array.Length - index < num)
				{
					throw new ArgumentException();
				}
				byte[] array3 = (byte[])array;
				for (int j = 0; j < num; j++)
				{
					array3[index + j] = getByte(j);
				}
			}
			else
			{
				if (!(array is int[]))
				{
					throw new ArgumentException("array", "Unsupported type");
				}
				Array.Copy(m_array, 0, array, index, (m_length + 31) / 32);
			}
		}

		public BitArray Not()
		{
			int num = (m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				m_array[i] = ~m_array[i];
			}
			_version++;
			return this;
		}

		public BitArray And(BitArray value)
		{
			checkOperand(value);
			int num = (m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				m_array[i] &= value.m_array[i];
			}
			_version++;
			return this;
		}

		public BitArray Or(BitArray value)
		{
			checkOperand(value);
			int num = (m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				m_array[i] |= value.m_array[i];
			}
			_version++;
			return this;
		}

		public BitArray Xor(BitArray value)
		{
			checkOperand(value);
			int num = (m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				m_array[i] ^= value.m_array[i];
			}
			_version++;
			return this;
		}

		public bool Get(int index)
		{
			if (index < 0 || index >= m_length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (m_array[index >> 5] & (1 << index)) != 0;
		}

		public void Set(int index, bool value)
		{
			if (index < 0 || index >= m_length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value)
			{
				m_array[index >> 5] |= 1 << (index & 0x1F);
			}
			else
			{
				m_array[index >> 5] &= ~(1 << (index & 0x1F));
			}
			_version++;
		}

		public void SetAll(bool value)
		{
			if (value)
			{
				for (int i = 0; i < m_array.Length; i++)
				{
					m_array[i] = -1;
				}
			}
			else
			{
				Array.Clear(m_array, 0, m_array.Length);
			}
			_version++;
		}

		public IEnumerator GetEnumerator()
		{
			return new BitArrayEnumerator(this);
		}
	}
}
