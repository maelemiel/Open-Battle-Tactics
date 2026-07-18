using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public sealed class SerializationInfo
	{
		private Hashtable serialized = new Hashtable();

		private ArrayList values = new ArrayList();

		private string assemblyName;

		private string fullTypeName;

		private IFormatterConverter converter;

		public string AssemblyName
		{
			get
			{
				return assemblyName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Argument is null.");
				}
				assemblyName = value;
			}
		}

		public string FullTypeName
		{
			get
			{
				return fullTypeName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Argument is null.");
				}
				fullTypeName = value;
			}
		}

		public int MemberCount
		{
			get
			{
				return serialized.Count;
			}
		}

		private SerializationInfo(Type type)
		{
			assemblyName = type.Assembly.FullName;
			fullTypeName = type.FullName;
			converter = new FormatterConverter();
		}

		private SerializationInfo(Type type, SerializationEntry[] data)
		{
			int num = data.Length;
			assemblyName = type.Assembly.FullName;
			fullTypeName = type.FullName;
			converter = new FormatterConverter();
			for (int i = 0; i < num; i++)
			{
				serialized.Add(data[i].Name, data[i]);
				values.Add(data[i]);
			}
		}

		[CLSCompliant(false)]
		public SerializationInfo(Type type, IFormatterConverter converter)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type", "Null argument");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter", "Null argument");
			}
			this.converter = converter;
			assemblyName = type.Assembly.FullName;
			fullTypeName = type.FullName;
		}

		public void AddValue(string name, object value, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is null");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type is null");
			}
			if (serialized.ContainsKey(name))
			{
				throw new SerializationException("Value has been serialized already.");
			}
			SerializationEntry serializationEntry = new SerializationEntry(name, type, value);
			serialized.Add(name, serializationEntry);
			values.Add(serializationEntry);
		}

		public object GetValue(string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is null.");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!serialized.ContainsKey(name))
			{
				throw new SerializationException("No element named " + name + " could be found.");
			}
			SerializationEntry serializationEntry = (SerializationEntry)serialized[name];
			if (serializationEntry.Value != null && !type.IsInstanceOfType(serializationEntry.Value))
			{
				return converter.Convert(serializationEntry.Value, type);
			}
			return serializationEntry.Value;
		}

		public void SetType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			fullTypeName = type.FullName;
			assemblyName = type.Assembly.FullName;
		}

		public SerializationInfoEnumerator GetEnumerator()
		{
			return new SerializationInfoEnumerator(values);
		}

		public void AddValue(string name, short value)
		{
			AddValue(name, value, typeof(short));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, ushort value)
		{
			AddValue(name, value, typeof(ushort));
		}

		public void AddValue(string name, int value)
		{
			AddValue(name, value, typeof(int));
		}

		public void AddValue(string name, byte value)
		{
			AddValue(name, value, typeof(byte));
		}

		public void AddValue(string name, bool value)
		{
			AddValue(name, value, typeof(bool));
		}

		public void AddValue(string name, char value)
		{
			AddValue(name, value, typeof(char));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, sbyte value)
		{
			AddValue(name, value, typeof(sbyte));
		}

		public void AddValue(string name, double value)
		{
			AddValue(name, value, typeof(double));
		}

		public void AddValue(string name, decimal value)
		{
			AddValue(name, value, typeof(decimal));
		}

		public void AddValue(string name, DateTime value)
		{
			AddValue(name, value, typeof(DateTime));
		}

		public void AddValue(string name, float value)
		{
			AddValue(name, value, typeof(float));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, uint value)
		{
			AddValue(name, value, typeof(uint));
		}

		public void AddValue(string name, long value)
		{
			AddValue(name, value, typeof(long));
		}

		[CLSCompliant(false)]
		public void AddValue(string name, ulong value)
		{
			AddValue(name, value, typeof(ulong));
		}

		public void AddValue(string name, object value)
		{
			if (value == null)
			{
				AddValue(name, value, typeof(object));
			}
			else
			{
				AddValue(name, value, value.GetType());
			}
		}

		public bool GetBoolean(string name)
		{
			object value = GetValue(name, typeof(bool));
			return converter.ToBoolean(value);
		}

		public byte GetByte(string name)
		{
			object value = GetValue(name, typeof(byte));
			return converter.ToByte(value);
		}

		public char GetChar(string name)
		{
			object value = GetValue(name, typeof(char));
			return converter.ToChar(value);
		}

		public DateTime GetDateTime(string name)
		{
			object value = GetValue(name, typeof(DateTime));
			return converter.ToDateTime(value);
		}

		public decimal GetDecimal(string name)
		{
			object value = GetValue(name, typeof(decimal));
			return converter.ToDecimal(value);
		}

		public double GetDouble(string name)
		{
			object value = GetValue(name, typeof(double));
			return converter.ToDouble(value);
		}

		public short GetInt16(string name)
		{
			object value = GetValue(name, typeof(short));
			return converter.ToInt16(value);
		}

		public int GetInt32(string name)
		{
			object value = GetValue(name, typeof(int));
			return converter.ToInt32(value);
		}

		public long GetInt64(string name)
		{
			object value = GetValue(name, typeof(long));
			return converter.ToInt64(value);
		}

		[CLSCompliant(false)]
		public sbyte GetSByte(string name)
		{
			object value = GetValue(name, typeof(sbyte));
			return converter.ToSByte(value);
		}

		public float GetSingle(string name)
		{
			object value = GetValue(name, typeof(float));
			return converter.ToSingle(value);
		}

		public string GetString(string name)
		{
			object value = GetValue(name, typeof(string));
			if (value == null)
			{
				return null;
			}
			return converter.ToString(value);
		}

		[CLSCompliant(false)]
		public ushort GetUInt16(string name)
		{
			object value = GetValue(name, typeof(ushort));
			return converter.ToUInt16(value);
		}

		[CLSCompliant(false)]
		public uint GetUInt32(string name)
		{
			object value = GetValue(name, typeof(uint));
			return converter.ToUInt32(value);
		}

		[CLSCompliant(false)]
		public ulong GetUInt64(string name)
		{
			object value = GetValue(name, typeof(ulong));
			return converter.ToUInt64(value);
		}

		private SerializationEntry[] get_entries()
		{
			SerializationEntry[] array = new SerializationEntry[MemberCount];
			int num = 0;
			SerializationInfoEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				array[num++] = current;
			}
			return array;
		}
	}
}
