using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public sealed class SqlBytes : INullable, IXmlSerializable, ISerializable
	{
		private bool notNull;

		private byte[] buffer;

		private StorageState storage = StorageState.UnmanagedBuffer;

		private Stream stream;

		public byte[] Buffer
		{
			get
			{
				return buffer;
			}
		}

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public byte this[long offset]
		{
			get
			{
				if (buffer == null)
				{
					throw new SqlNullValueException("Data is Null");
				}
				if (offset < 0 || offset >= buffer.Length)
				{
					throw new ArgumentOutOfRangeException("Parameter name: offset");
				}
				return buffer[offset];
			}
			set
			{
				if (notNull && offset >= 0 && offset < buffer.Length)
				{
					buffer[offset] = value;
				}
			}
		}

		public long Length
		{
			get
			{
				if (!notNull || buffer == null)
				{
					throw new SqlNullValueException("Data is Null");
				}
				if (buffer.Length < 0)
				{
					return -1L;
				}
				return buffer.Length;
			}
		}

		public long MaxLength
		{
			get
			{
				if (!notNull || buffer == null || storage == StorageState.Stream)
				{
					return -1L;
				}
				return buffer.Length;
			}
		}

		public static SqlBytes Null
		{
			get
			{
				return new SqlBytes();
			}
		}

		public StorageState Storage
		{
			get
			{
				if (storage == StorageState.UnmanagedBuffer)
				{
					throw new SqlNullValueException("Data is Null");
				}
				return storage;
			}
		}

		public Stream Stream
		{
			get
			{
				return stream;
			}
			set
			{
				stream = value;
			}
		}

		public byte[] Value
		{
			get
			{
				if (buffer == null)
				{
					return buffer;
				}
				return (byte[])buffer.Clone();
			}
		}

		public SqlBytes()
		{
			buffer = null;
			notNull = false;
		}

		public SqlBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				notNull = false;
				buffer = null;
			}
			else
			{
				notNull = true;
				this.buffer = buffer;
				storage = StorageState.Buffer;
			}
		}

		public SqlBytes(SqlBinary value)
		{
			if (value.IsNull)
			{
				notNull = false;
				buffer = null;
			}
			else
			{
				notNull = true;
				buffer = value.Value;
				storage = StorageState.Buffer;
			}
		}

		public SqlBytes(Stream s)
		{
			if (s == null)
			{
				notNull = false;
				buffer = null;
				return;
			}
			notNull = true;
			int num = (int)s.Length;
			buffer = new byte[num];
			s.Read(buffer, 0, num);
			storage = StorageState.Stream;
			stream = s;
		}

		[System.MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader r)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public void SetLength(long value)
		{
			if (buffer == null)
			{
				throw new SqlTypeException("There is no buffer. Read or write operation failed.");
			}
			if (value < 0 || value > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("Specified argument was out of the range of valid values.");
			}
			Array.Resize(ref buffer, (int)value);
		}

		public void SetNull()
		{
			buffer = null;
			notNull = false;
		}

		public SqlBinary ToSqlBinary()
		{
			return new SqlBinary(buffer);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
		}

		public long Read(long offset, byte[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (IsNull)
			{
				throw new SqlNullValueException("There is no buffer. Read or write failed");
			}
			if (count > MaxLength || count > buffer.Length || count < 0 || offsetInBuffer + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset < 0 || offset > MaxLength)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offsetInBuffer < 0 || offsetInBuffer > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offsetInBuffer");
			}
			long num = count;
			if (count + offset > Length)
			{
				num = Length - offset;
			}
			Array.Copy(this.buffer, offset, buffer, offsetInBuffer, num);
			return num;
		}

		public void Write(long offset, byte[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (IsNull)
			{
				throw new SqlTypeException("There is no buffer. Read or write operation failed.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offsetInBuffer < 0 || offsetInBuffer > buffer.Length || offsetInBuffer > Length || offsetInBuffer + count > Length || offsetInBuffer + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offsetInBuffer");
			}
			if (count < 0 || count > MaxLength)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset > MaxLength || offset + count > MaxLength)
			{
				throw new SqlTypeException("The buffer is insufficient. Read or write operation failed.");
			}
			if (count + offset > Length && count + offset <= MaxLength)
			{
				SetLength(count);
			}
			Array.Copy(buffer, offsetInBuffer, this.buffer, offset, count);
		}

		public static explicit operator SqlBytes(SqlBinary value)
		{
			if (value.IsNull)
			{
				return Null;
			}
			return new SqlBytes(value.Value);
		}

		public static explicit operator SqlBinary(SqlBytes value)
		{
			if (value.IsNull)
			{
				return SqlBinary.Null;
			}
			return new SqlBinary(value.Value);
		}
	}
}
