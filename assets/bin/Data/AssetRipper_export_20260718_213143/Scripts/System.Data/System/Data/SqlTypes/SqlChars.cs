using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public sealed class SqlChars : INullable, IXmlSerializable, ISerializable
	{
		private bool notNull;

		private char[] buffer;

		private StorageState storage = StorageState.UnmanagedBuffer;

		public char[] Buffer
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

		public char this[long offset]
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

		public static SqlChars Null
		{
			get
			{
				return new SqlChars();
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

		public char[] Value
		{
			get
			{
				if (buffer == null)
				{
					return buffer;
				}
				return (char[])buffer.Clone();
			}
		}

		public SqlChars()
		{
			notNull = false;
			buffer = null;
		}

		public SqlChars(char[] buffer)
		{
			if (buffer == null)
			{
				notNull = false;
				this.buffer = null;
			}
			else
			{
				notNull = true;
				this.buffer = buffer;
				storage = StorageState.Buffer;
			}
		}

		public SqlChars(SqlString value)
		{
			if (value.IsNull)
			{
				notNull = false;
				buffer = null;
			}
			else
			{
				notNull = true;
				buffer = value.Value.ToCharArray();
				storage = StorageState.Buffer;
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			}
			reader.MoveToContent();
			if (reader.EOF)
			{
				return;
			}
			reader.Read();
			if (reader.NodeType != XmlNodeType.EndElement && reader.Value.Length > 0)
			{
				if (string.Compare("Null", reader.Value) == 0)
				{
					notNull = false;
					return;
				}
				buffer = reader.Value.ToCharArray();
				notNull = true;
				storage = StorageState.Buffer;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(buffer.ToString());
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
				throw new SqlTypeException("There is no buffer");
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

		public SqlString ToSqlString()
		{
			if (buffer == null)
			{
				return SqlString.Null;
			}
			return new SqlString(buffer.ToString());
		}

		public long Read(long offset, char[] buffer, int offsetInBuffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (IsNull)
			{
				throw new SqlNullValueException("There is no buffer. Read or write operation failed");
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

		public void Write(long offset, char[] buffer, int offsetInBuffer, int count)
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

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "string";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		public static explicit operator SqlString(SqlChars value)
		{
			if (value.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(new string(value.Value));
		}

		public static explicit operator SqlChars(SqlString value)
		{
			if (value.IsNull)
			{
				return Null;
			}
			return new SqlChars(value.Value);
		}
	}
}
