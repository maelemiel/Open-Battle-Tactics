using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Text
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public sealed class StringBuilder : ISerializable
	{
		private const int constDefaultCapacity = 16;

		private int _length;

		private string _str;

		private string _cached_str;

		private int _maxCapacity;

		public int MaxCapacity
		{
			get
			{
				return _maxCapacity;
			}
		}

		public int Capacity
		{
			get
			{
				if (_str.Length == 0)
				{
					return Math.Min(_maxCapacity, 16);
				}
				return _str.Length;
			}
			set
			{
				if (value < _length)
				{
					throw new ArgumentException("Capacity must be larger than length");
				}
				if (value > _maxCapacity)
				{
					throw new ArgumentOutOfRangeException("value", "Should be less than or equal to MaxCapacity");
				}
				InternalEnsureCapacity(value);
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				if (value < 0 || value > _maxCapacity)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value != _length)
				{
					if (value < _length)
					{
						InternalEnsureCapacity(value);
						_length = value;
					}
					else
					{
						Append('\0', value - _length);
					}
				}
			}
		}

		[IndexerName("Chars")]
		public char this[int index]
		{
			get
			{
				if (index >= _length || index < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return _str[index];
			}
			set
			{
				if (index >= _length || index < 0)
				{
					throw new IndexOutOfRangeException();
				}
				if (_cached_str != null)
				{
					InternalEnsureCapacity(_length);
				}
				_str.InternalSetChar(index, value);
			}
		}

		public StringBuilder(string value, int startIndex, int length, int capacity)
			: this(value, startIndex, length, capacity, int.MaxValue)
		{
		}

		private StringBuilder(string value, int startIndex, int length, int capacity, int maxCapacity)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex cannot be less than zero.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero.");
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be greater than zero.");
			}
			if (maxCapacity < 1)
			{
				throw new ArgumentOutOfRangeException("maxCapacity", "maxCapacity is less than one.");
			}
			if (capacity > maxCapacity)
			{
				throw new ArgumentOutOfRangeException("capacity", "Capacity exceeds maximum capacity.");
			}
			if (startIndex > value.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex and length must refer to a location within the string.");
			}
			if (capacity == 0)
			{
				if (maxCapacity > 16)
				{
					capacity = 16;
				}
				else
				{
					_str = (_cached_str = string.Empty);
				}
			}
			_maxCapacity = maxCapacity;
			if (_str == null)
			{
				_str = string.InternalAllocateStr((length <= capacity) ? capacity : length);
			}
			if (length > 0)
			{
				string.CharCopy(_str, 0, value, startIndex, length);
			}
			_length = length;
		}

		public StringBuilder()
			: this(null)
		{
		}

		public StringBuilder(int capacity)
			: this(string.Empty, 0, 0, capacity)
		{
		}

		public StringBuilder(int capacity, int maxCapacity)
			: this(string.Empty, 0, 0, capacity, maxCapacity)
		{
		}

		public StringBuilder(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			_length = value.Length;
			_str = (_cached_str = value);
			_maxCapacity = int.MaxValue;
		}

		public StringBuilder(string value, int capacity)
			: this((value != null) ? value : string.Empty, 0, (value != null) ? value.Length : 0, capacity)
		{
		}

		private StringBuilder(SerializationInfo info, StreamingContext context)
		{
			string text = info.GetString("m_StringValue");
			if (text == null)
			{
				text = string.Empty;
			}
			_length = text.Length;
			_str = (_cached_str = text);
			_maxCapacity = info.GetInt32("m_MaxCapacity");
			if (_maxCapacity < 0)
			{
				_maxCapacity = int.MaxValue;
			}
			Capacity = info.GetInt32("Capacity");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_MaxCapacity", _maxCapacity);
			info.AddValue("Capacity", Capacity);
			info.AddValue("m_StringValue", ToString());
			info.AddValue("m_currentThread", 0);
		}

		public override string ToString()
		{
			if (_length == 0)
			{
				return string.Empty;
			}
			if (_cached_str != null)
			{
				return _cached_str;
			}
			if (_length < _str.Length >> 1)
			{
				_cached_str = _str.SubstringUnchecked(0, _length);
				return _cached_str;
			}
			_cached_str = _str;
			_str.InternalSetLength(_length);
			return _str;
		}

		public string ToString(int startIndex, int length)
		{
			if (startIndex < 0 || length < 0 || startIndex > _length - length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (startIndex == 0 && length == _length)
			{
				return ToString();
			}
			return _str.SubstringUnchecked(startIndex, length);
		}

		public int EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("Capacity must be greater than 0.");
			}
			if (capacity <= _str.Length)
			{
				return _str.Length;
			}
			InternalEnsureCapacity(capacity);
			return _str.Length;
		}

		public bool Equals(StringBuilder sb)
		{
			if (sb == null)
			{
				return false;
			}
			if (_length == sb.Length && _str == sb._str)
			{
				return true;
			}
			return false;
		}

		public StringBuilder Remove(int startIndex, int length)
		{
			if (startIndex < 0 || length < 0 || startIndex > _length - length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (_cached_str != null)
			{
				InternalEnsureCapacity(_length);
			}
			if (_length - (startIndex + length) > 0)
			{
				string.CharCopy(_str, startIndex, _str, startIndex + length, _length - (startIndex + length));
			}
			_length -= length;
			return this;
		}

		public StringBuilder Replace(char oldChar, char newChar)
		{
			return Replace(oldChar, newChar, 0, _length);
		}

		public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
		{
			if (startIndex > _length - count || startIndex < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (_cached_str != null)
			{
				InternalEnsureCapacity(_str.Length);
			}
			for (int i = startIndex; i < startIndex + count; i++)
			{
				if (_str[i] == oldChar)
				{
					_str.InternalSetChar(i, newChar);
				}
			}
			return this;
		}

		public StringBuilder Replace(string oldValue, string newValue)
		{
			return Replace(oldValue, newValue, 0, _length);
		}

		public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("The old value cannot be null.");
			}
			if (startIndex < 0 || count < 0 || startIndex > _length - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("The old value cannot be zero length.");
			}
			string text = _str.Substring(startIndex, count);
			string text2 = text.Replace(oldValue, newValue);
			if ((object)text2 == text)
			{
				return this;
			}
			InternalEnsureCapacity(text2.Length + (_length - count));
			if (text2.Length < count)
			{
				string.CharCopy(_str, startIndex + text2.Length, _str, startIndex + count, _length - startIndex - count);
			}
			else if (text2.Length > count)
			{
				string.CharCopyReverse(_str, startIndex + text2.Length, _str, startIndex + count, _length - startIndex - count);
			}
			string.CharCopy(_str, startIndex, text2, 0, text2.Length);
			_length = text2.Length + (_length - count);
			return this;
		}

		public StringBuilder Append(char[] value)
		{
			if (value == null)
			{
				return this;
			}
			int num = _length + value.Length;
			if (_cached_str != null || _str.Length < num)
			{
				InternalEnsureCapacity(num);
			}
			string.CharCopy(_str, _length, value, 0, value.Length);
			_length = num;
			return this;
		}

		public StringBuilder Append(string value)
		{
			if (value == null)
			{
				return this;
			}
			if (_length == 0 && value.Length < _maxCapacity && value.Length > _str.Length)
			{
				_length = value.Length;
				_str = (_cached_str = value);
				return this;
			}
			int num = _length + value.Length;
			if (_cached_str != null || _str.Length < num)
			{
				InternalEnsureCapacity(num);
			}
			string.CharCopy(_str, _length, value, 0, value.Length);
			_length = num;
			return this;
		}

		public StringBuilder Append(bool value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(byte value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(decimal value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(double value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(short value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(int value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(long value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(object value)
		{
			if (value == null)
			{
				return this;
			}
			return Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(sbyte value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(float value)
		{
			return Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(ushort value)
		{
			return Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(uint value)
		{
			return Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(ulong value)
		{
			return Append(value.ToString());
		}

		public StringBuilder Append(char value)
		{
			int num = _length + 1;
			if (_cached_str != null || _str.Length < num)
			{
				InternalEnsureCapacity(num);
			}
			_str.InternalSetChar(_length, value);
			_length = num;
			return this;
		}

		public StringBuilder Append(char value, int repeatCount)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			InternalEnsureCapacity(_length + repeatCount);
			for (int i = 0; i < repeatCount; i++)
			{
				_str.InternalSetChar(_length++, value);
			}
			return this;
		}

		public StringBuilder Append(char[] value, int startIndex, int charCount)
		{
			if (value == null)
			{
				if (startIndex != 0 || charCount != 0)
				{
					throw new ArgumentNullException("value");
				}
				return this;
			}
			if (charCount < 0 || startIndex < 0 || startIndex > value.Length - charCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = _length + charCount;
			InternalEnsureCapacity(num);
			string.CharCopy(_str, _length, value, startIndex, charCount);
			_length = num;
			return this;
		}

		public StringBuilder Append(string value, int startIndex, int count)
		{
			if (value == null)
			{
				if (startIndex != 0 && count != 0)
				{
					throw new ArgumentNullException("value");
				}
				return this;
			}
			if (count < 0 || startIndex < 0 || startIndex > value.Length - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = _length + count;
			if (_cached_str != null || _str.Length < num)
			{
				InternalEnsureCapacity(num);
			}
			string.CharCopy(_str, _length, value, startIndex, count);
			_length = num;
			return this;
		}

		[ComVisible(false)]
		public StringBuilder AppendLine()
		{
			return Append(Environment.NewLine);
		}

		[ComVisible(false)]
		public StringBuilder AppendLine(string value)
		{
			return Append(value).Append(Environment.NewLine);
		}

		public StringBuilder AppendFormat(string format, params object[] args)
		{
			return AppendFormat(null, format, args);
		}

		public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
		{
			string.FormatHelper(this, provider, format, args);
			return this;
		}

		public StringBuilder AppendFormat(string format, object arg0)
		{
			return AppendFormat(null, format, arg0);
		}

		public StringBuilder AppendFormat(string format, object arg0, object arg1)
		{
			return AppendFormat(null, format, arg0, arg1);
		}

		public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
		{
			return AppendFormat(null, format, arg0, arg1, arg2);
		}

		public StringBuilder Insert(int index, char[] value)
		{
			return Insert(index, new string(value));
		}

		public StringBuilder Insert(int index, string value)
		{
			if (index > _length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value == null || value.Length == 0)
			{
				return this;
			}
			InternalEnsureCapacity(_length + value.Length);
			string.CharCopyReverse(_str, index + value.Length, _str, index, _length - index);
			string.CharCopy(_str, index, value, 0, value.Length);
			_length += value.Length;
			return this;
		}

		public StringBuilder Insert(int index, bool value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, byte value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, char value)
		{
			if (index > _length || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			InternalEnsureCapacity(_length + 1);
			string.CharCopyReverse(_str, index + 1, _str, index, _length - index);
			_str.InternalSetChar(index, value);
			_length++;
			return this;
		}

		public StringBuilder Insert(int index, decimal value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, double value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, short value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, int value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, long value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, object value)
		{
			return Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, sbyte value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, float value)
		{
			return Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, ushort value)
		{
			return Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, uint value)
		{
			return Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, ulong value)
		{
			return Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, string value, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value != null && value != string.Empty)
			{
				for (int i = 0; i < count; i++)
				{
					Insert(index, value);
				}
			}
			return this;
		}

		public StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
		{
			if (value == null)
			{
				if (startIndex == 0 && charCount == 0)
				{
					return this;
				}
				throw new ArgumentNullException("value");
			}
			if (charCount < 0 || startIndex < 0 || startIndex > value.Length - charCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			return Insert(index, new string(value, startIndex, charCount));
		}

		private void InternalEnsureCapacity(int size)
		{
			if (size > _str.Length || (object)_cached_str == _str)
			{
				int num = _str.Length;
				if (size > num)
				{
					if ((object)_cached_str == _str && num < 16)
					{
						num = 16;
					}
					num <<= 1;
					if (size > num)
					{
						num = size;
					}
					if (num >= int.MaxValue || num < 0)
					{
						num = int.MaxValue;
					}
					if (num > _maxCapacity && size <= _maxCapacity)
					{
						num = _maxCapacity;
					}
					if (num > _maxCapacity)
					{
						throw new ArgumentOutOfRangeException("size", "capacity was less than the current size.");
					}
				}
				string text = string.InternalAllocateStr(num);
				if (_length > 0)
				{
					string.CharCopy(text, 0, _str, 0, _length);
				}
				_str = text;
			}
			_cached_str = null;
		}

		[ComVisible(false)]
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (Length - count < sourceIndex || destination.Length - count < destinationIndex || sourceIndex < 0 || destinationIndex < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = 0; i < count; i++)
			{
				destination[destinationIndex + i] = _str[sourceIndex + i];
			}
		}
	}
}
