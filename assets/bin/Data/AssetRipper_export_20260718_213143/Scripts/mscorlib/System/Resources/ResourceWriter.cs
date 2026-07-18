using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Resources
{
	[ComVisible(true)]
	public sealed class ResourceWriter : IDisposable, IResourceWriter
	{
		private class TypeByNameObject
		{
			public readonly string TypeName;

			public readonly byte[] Value;

			public TypeByNameObject(string typeName, byte[] value)
			{
				TypeName = typeName;
				Value = (byte[])value.Clone();
			}
		}

		private SortedList resources = new SortedList(StringComparer.OrdinalIgnoreCase);

		private Stream stream;

		internal Stream Stream
		{
			get
			{
				return stream;
			}
		}

		public ResourceWriter(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("Stream was not writable.");
			}
			this.stream = stream;
		}

		public ResourceWriter(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		}

		public void AddResource(string name, byte[] value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (resources == null)
			{
				throw new InvalidOperationException("The resource writer has already been closed and cannot be edited");
			}
			if (resources[name] != null)
			{
				throw new ArgumentException("Resource already present: " + name);
			}
			resources.Add(name, value);
		}

		public void AddResource(string name, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (resources == null)
			{
				throw new InvalidOperationException("The resource writer has already been closed and cannot be edited");
			}
			if (resources[name] != null)
			{
				throw new ArgumentException("Resource already present: " + name);
			}
			resources.Add(name, value);
		}

		public void AddResource(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (resources == null)
			{
				throw new InvalidOperationException("The resource writer has already been closed and cannot be edited");
			}
			if (resources[name] != null)
			{
				throw new ArgumentException("Resource already present: " + name);
			}
			resources.Add(name, value);
		}

		public void Close()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (resources != null)
				{
					Generate();
				}
				if (stream != null)
				{
					stream.Close();
				}
				GC.SuppressFinalize(this);
			}
			resources = null;
			stream = null;
		}

		public void AddResourceData(string name, string typeName, byte[] serializedData)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (serializedData == null)
			{
				throw new ArgumentNullException("serializedData");
			}
			AddResource(name, new TypeByNameObject(typeName, serializedData));
		}

		public void Generate()
		{
			if (resources == null)
			{
				throw new InvalidOperationException("The resource writer has already been closed and cannot be edited");
			}
			BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8);
			IFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence));
			binaryWriter.Write(ResourceManager.MagicNumber);
			binaryWriter.Write(ResourceManager.HeaderVersionNumber);
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream, Encoding.UTF8);
			binaryWriter2.Write(typeof(ResourceReader).AssemblyQualifiedName);
			binaryWriter2.Write(typeof(RuntimeResourceSet).FullName);
			int num = (int)memoryStream.Length;
			binaryWriter.Write(num);
			binaryWriter.Write(memoryStream.GetBuffer(), 0, num);
			MemoryStream memoryStream2 = new MemoryStream();
			BinaryWriter binaryWriter3 = new BinaryWriter(memoryStream2, Encoding.Unicode);
			MemoryStream memoryStream3 = new MemoryStream();
			BinaryWriter binaryWriter4 = new BinaryWriter(memoryStream3, Encoding.UTF8);
			ArrayList arrayList = new ArrayList();
			int[] array = new int[resources.Count];
			int[] array2 = new int[resources.Count];
			int num2 = 0;
			IDictionaryEnumerator enumerator = resources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[num2] = GetHash((string)enumerator.Key);
				array2[num2] = (int)binaryWriter3.BaseStream.Position;
				binaryWriter3.Write((string)enumerator.Key);
				binaryWriter3.Write((int)binaryWriter4.BaseStream.Position);
				if (enumerator.Value == null)
				{
					Write7BitEncodedInt(binaryWriter4, -1);
					num2++;
					continue;
				}
				TypeByNameObject typeByNameObject = enumerator.Value as TypeByNameObject;
				Type type = ((typeByNameObject == null) ? enumerator.Value.GetType() : null);
				object obj = ((typeByNameObject == null) ? ((object)type) : ((object)typeByNameObject.TypeName));
				switch ((type != null && !type.IsEnum) ? Type.GetTypeCode(type) : TypeCode.Empty)
				{
				default:
					if (type != typeof(TimeSpan) && type != typeof(MemoryStream) && type != typeof(byte[]))
					{
						if (!arrayList.Contains(obj))
						{
							arrayList.Add(obj);
						}
						Write7BitEncodedInt(binaryWriter4, 64 + arrayList.IndexOf(obj));
					}
					break;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					break;
				}
				if (typeByNameObject != null)
				{
					binaryWriter4.Write(typeByNameObject.Value);
				}
				else if (type == typeof(byte))
				{
					binaryWriter4.Write((byte)4);
					binaryWriter4.Write((byte)enumerator.Value);
				}
				else if (type == typeof(decimal))
				{
					binaryWriter4.Write((byte)14);
					binaryWriter4.Write((decimal)enumerator.Value);
				}
				else if (type == typeof(DateTime))
				{
					binaryWriter4.Write((byte)15);
					binaryWriter4.Write(((DateTime)enumerator.Value).Ticks);
				}
				else if (type == typeof(double))
				{
					binaryWriter4.Write((byte)13);
					binaryWriter4.Write((double)enumerator.Value);
				}
				else if (type == typeof(short))
				{
					binaryWriter4.Write((byte)6);
					binaryWriter4.Write((short)enumerator.Value);
				}
				else if (type == typeof(int))
				{
					binaryWriter4.Write((byte)8);
					binaryWriter4.Write((int)enumerator.Value);
				}
				else if (type == typeof(long))
				{
					binaryWriter4.Write((byte)10);
					binaryWriter4.Write((long)enumerator.Value);
				}
				else if (type == typeof(sbyte))
				{
					binaryWriter4.Write((byte)5);
					binaryWriter4.Write((sbyte)enumerator.Value);
				}
				else if (type == typeof(float))
				{
					binaryWriter4.Write((byte)12);
					binaryWriter4.Write((float)enumerator.Value);
				}
				else if (type == typeof(string))
				{
					binaryWriter4.Write((byte)1);
					binaryWriter4.Write((string)enumerator.Value);
				}
				else if (type == typeof(TimeSpan))
				{
					binaryWriter4.Write((byte)16);
					binaryWriter4.Write(((TimeSpan)enumerator.Value).Ticks);
				}
				else if (type == typeof(ushort))
				{
					binaryWriter4.Write((byte)7);
					binaryWriter4.Write((ushort)enumerator.Value);
				}
				else if (type == typeof(uint))
				{
					binaryWriter4.Write((byte)9);
					binaryWriter4.Write((uint)enumerator.Value);
				}
				else if (type == typeof(ulong))
				{
					binaryWriter4.Write((byte)11);
					binaryWriter4.Write((ulong)enumerator.Value);
				}
				else if (type == typeof(byte[]))
				{
					binaryWriter4.Write((byte)32);
					byte[] array3 = (byte[])enumerator.Value;
					binaryWriter4.Write((uint)array3.Length);
					binaryWriter4.Write(array3, 0, array3.Length);
				}
				else if (type == typeof(MemoryStream))
				{
					binaryWriter4.Write((byte)33);
					byte[] array4 = ((MemoryStream)enumerator.Value).ToArray();
					binaryWriter4.Write((uint)array4.Length);
					binaryWriter4.Write(array4, 0, array4.Length);
				}
				else
				{
					formatter.Serialize(binaryWriter4.BaseStream, enumerator.Value);
				}
				num2++;
			}
			Array.Sort(array, array2);
			binaryWriter.Write(2);
			binaryWriter.Write(resources.Count);
			binaryWriter.Write(arrayList.Count);
			foreach (object item in arrayList)
			{
				if (item is Type)
				{
					binaryWriter.Write(((Type)item).AssemblyQualifiedName);
				}
				else
				{
					binaryWriter.Write((string)item);
				}
			}
			int num3 = (int)(binaryWriter.BaseStream.Position & 7);
			int num4 = 0;
			if (num3 != 0)
			{
				num4 = 8 - num3;
			}
			for (int i = 0; i < num4; i++)
			{
				binaryWriter.Write((byte)"PAD"[i % 3]);
			}
			for (int j = 0; j < resources.Count; j++)
			{
				binaryWriter.Write(array[j]);
			}
			for (int k = 0; k < resources.Count; k++)
			{
				binaryWriter.Write(array2[k]);
			}
			int value = (int)binaryWriter.BaseStream.Position + (int)memoryStream2.Length + 4;
			binaryWriter.Write(value);
			binaryWriter.Write(memoryStream2.GetBuffer(), 0, (int)memoryStream2.Length);
			binaryWriter.Write(memoryStream3.GetBuffer(), 0, (int)memoryStream3.Length);
			binaryWriter3.Close();
			binaryWriter4.Close();
			binaryWriter.Flush();
			resources = null;
		}

		private int GetHash(string name)
		{
			uint num = 5381u;
			for (int i = 0; i < name.Length; i++)
			{
				num = ((num << 5) + num) ^ name[i];
			}
			return (int)num;
		}

		private void Write7BitEncodedInt(BinaryWriter writer, int value)
		{
			do
			{
				int num = (value >> 7) & 0x1FFFFFF;
				byte b = (byte)(value & 0x7F);
				if (num != 0)
				{
					b |= 0x80;
				}
				writer.Write(b);
				value = num;
			}
			while (value != 0);
		}
	}
}
