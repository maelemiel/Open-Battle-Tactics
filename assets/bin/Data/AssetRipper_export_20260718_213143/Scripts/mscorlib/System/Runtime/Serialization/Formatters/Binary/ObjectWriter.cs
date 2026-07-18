using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class ObjectWriter
	{
		private class MetadataReference
		{
			public TypeMetadata Metadata;

			public long ObjectID;

			public MetadataReference(TypeMetadata metadata, long id)
			{
				Metadata = metadata;
				ObjectID = id;
			}
		}

		private ObjectIDGenerator _idGenerator = new ObjectIDGenerator();

		private Hashtable _cachedMetadata = new Hashtable();

		private Queue _pendingObjects = new Queue();

		private Hashtable _assemblyCache = new Hashtable();

		private static Hashtable _cachedTypes = new Hashtable();

		internal static Assembly CorlibAssembly = typeof(string).Assembly;

		internal static string CorlibAssemblyName = typeof(string).Assembly.FullName;

		private ISurrogateSelector _surrogateSelector;

		private StreamingContext _context;

		private FormatterAssemblyStyle _assemblyFormat;

		private FormatterTypeStyle _typeFormat;

		private byte[] arrayBuffer;

		private int ArrayBufferLength = 4096;

		private SerializationObjectManager _manager;

		public ObjectWriter(ISurrogateSelector surrogateSelector, StreamingContext context, FormatterAssemblyStyle assemblyFormat, FormatterTypeStyle typeFormat)
		{
			_surrogateSelector = surrogateSelector;
			_context = context;
			_assemblyFormat = assemblyFormat;
			_typeFormat = typeFormat;
			_manager = new SerializationObjectManager(context);
		}

		public void WriteObjectGraph(BinaryWriter writer, object obj, Header[] headers)
		{
			_pendingObjects.Clear();
			if (headers != null)
			{
				QueueObject(headers);
			}
			QueueObject(obj);
			WriteQueuedObjects(writer);
			WriteSerializationEnd(writer);
			_manager.RaiseOnSerializedEvent();
		}

		public void QueueObject(object obj)
		{
			_pendingObjects.Enqueue(obj);
		}

		public void WriteQueuedObjects(BinaryWriter writer)
		{
			while (_pendingObjects.Count > 0)
			{
				WriteObjectInstance(writer, _pendingObjects.Dequeue(), false);
			}
		}

		public void WriteObjectInstance(BinaryWriter writer, object obj, bool isValueObject)
		{
			bool firstTime;
			long id = ((!isValueObject) ? _idGenerator.GetId(obj, out firstTime) : _idGenerator.NextId);
			if (obj is string)
			{
				WriteString(writer, id, (string)obj);
			}
			else if (obj is Array)
			{
				WriteArray(writer, id, (Array)obj);
			}
			else
			{
				WriteObject(writer, id, obj);
			}
		}

		public static void WriteSerializationEnd(BinaryWriter writer)
		{
			writer.Write((byte)11);
		}

		private void WriteObject(BinaryWriter writer, long id, object obj)
		{
			TypeMetadata metadata;
			object data;
			GetObjectData(obj, out metadata, out data);
			MetadataReference metadataReference = (MetadataReference)_cachedMetadata[metadata.InstanceTypeName];
			if (metadataReference != null && metadata.IsCompatible(metadataReference.Metadata))
			{
				writer.Write((byte)1);
				writer.Write((int)id);
				writer.Write((int)metadataReference.ObjectID);
				metadata.WriteObjectData(this, writer, data);
				return;
			}
			if (metadataReference == null)
			{
				metadataReference = new MetadataReference(metadata, id);
				_cachedMetadata[metadata.InstanceTypeName] = metadataReference;
			}
			bool flag = metadata.RequiresTypes || _typeFormat == FormatterTypeStyle.TypesAlways;
			BinaryElement value;
			int num;
			if (metadata.TypeAssemblyName == CorlibAssemblyName)
			{
				value = ((!flag) ? BinaryElement.UntypedRuntimeObject : BinaryElement.RuntimeObject);
				num = -1;
			}
			else
			{
				value = ((!flag) ? BinaryElement.UntypedExternalObject : BinaryElement.ExternalObject);
				num = WriteAssemblyName(writer, metadata.TypeAssemblyName);
			}
			metadata.WriteAssemblies(this, writer);
			writer.Write((byte)value);
			writer.Write((int)id);
			writer.Write(metadata.InstanceTypeName);
			metadata.WriteTypeData(this, writer, flag);
			if (num != -1)
			{
				writer.Write(num);
			}
			metadata.WriteObjectData(this, writer, data);
		}

		private void GetObjectData(object obj, out TypeMetadata metadata, out object data)
		{
			Type type = obj.GetType();
			if (_surrogateSelector != null)
			{
				ISurrogateSelector selector;
				ISerializationSurrogate surrogate = _surrogateSelector.GetSurrogate(type, _context, out selector);
				if (surrogate != null)
				{
					SerializationInfo serializationInfo = new SerializationInfo(type, new FormatterConverter());
					surrogate.GetObjectData(obj, serializationInfo, _context);
					metadata = new SerializableTypeMetadata(type, serializationInfo);
					data = serializationInfo;
					return;
				}
			}
			BinaryCommon.CheckSerializable(type, _surrogateSelector, _context);
			_manager.RegisterObject(obj);
			ISerializable serializable = obj as ISerializable;
			if (serializable != null)
			{
				SerializationInfo serializationInfo2 = new SerializationInfo(type, new FormatterConverter());
				serializable.GetObjectData(serializationInfo2, _context);
				metadata = new SerializableTypeMetadata(type, serializationInfo2);
				data = serializationInfo2;
				return;
			}
			data = obj;
			if (_context.Context != null)
			{
				metadata = new MemberTypeMetadata(type, _context);
				return;
			}
			bool flag = false;
			Hashtable hashtable;
			lock (_cachedTypes)
			{
				hashtable = (Hashtable)_cachedTypes[_context.State];
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					_cachedTypes[_context.State] = hashtable;
					flag = true;
				}
			}
			metadata = null;
			lock (hashtable)
			{
				if (!flag)
				{
					metadata = (TypeMetadata)hashtable[type];
				}
				if (metadata == null)
				{
					metadata = CreateMemberTypeMetadata(type);
				}
				hashtable[type] = metadata;
			}
		}

		private TypeMetadata CreateMemberTypeMetadata(Type type)
		{
			if (!BinaryCommon.UseReflectionSerialization)
			{
				Type type2 = CodeGenerator.GenerateMetadataType(type, _context);
				return (TypeMetadata)Activator.CreateInstance(type2);
			}
			return new MemberTypeMetadata(type, _context);
		}

		private void WriteArray(BinaryWriter writer, long id, Array array)
		{
			Type elementType = array.GetType().GetElementType();
			if (elementType == typeof(object) && array.Rank == 1)
			{
				WriteObjectArray(writer, id, array);
			}
			else if (elementType == typeof(string) && array.Rank == 1)
			{
				WriteStringArray(writer, id, array);
			}
			else if (BinaryCommon.IsPrimitive(elementType) && array.Rank == 1)
			{
				WritePrimitiveTypeArray(writer, id, array);
			}
			else
			{
				WriteGenericArray(writer, id, array);
			}
		}

		private void WriteGenericArray(BinaryWriter writer, long id, Array array)
		{
			Type elementType = array.GetType().GetElementType();
			if (!elementType.IsArray)
			{
				WriteAssembly(writer, elementType.Assembly);
			}
			writer.Write((byte)7);
			writer.Write((int)id);
			if (elementType.IsArray)
			{
				writer.Write((byte)1);
			}
			else if (array.Rank == 1)
			{
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)2);
			}
			writer.Write(array.Rank);
			for (int i = 0; i < array.Rank; i++)
			{
				writer.Write(array.GetUpperBound(i) + 1);
			}
			WriteTypeCode(writer, elementType);
			WriteTypeSpec(writer, elementType);
			if (array.Rank == 1 && !elementType.IsValueType)
			{
				WriteSingleDimensionArrayElements(writer, array, elementType);
				return;
			}
			foreach (object item in array)
			{
				WriteValue(writer, elementType, item);
			}
		}

		private void WriteObjectArray(BinaryWriter writer, long id, Array array)
		{
			writer.Write((byte)16);
			writer.Write((int)id);
			writer.Write(array.Length);
			WriteSingleDimensionArrayElements(writer, array, typeof(object));
		}

		private void WriteStringArray(BinaryWriter writer, long id, Array array)
		{
			writer.Write((byte)17);
			writer.Write((int)id);
			writer.Write(array.Length);
			WriteSingleDimensionArrayElements(writer, array, typeof(string));
		}

		private void WritePrimitiveTypeArray(BinaryWriter writer, long id, Array array)
		{
			writer.Write((byte)15);
			writer.Write((int)id);
			writer.Write(array.Length);
			Type elementType = array.GetType().GetElementType();
			WriteTypeSpec(writer, elementType);
			switch (Type.GetTypeCode(elementType))
			{
			case TypeCode.Boolean:
			{
				bool[] array11 = (bool[])array;
				foreach (bool value9 in array11)
				{
					writer.Write(value9);
				}
				return;
			}
			case TypeCode.Byte:
				writer.Write((byte[])array);
				return;
			case TypeCode.Char:
				writer.Write((char[])array);
				return;
			case TypeCode.DateTime:
			{
				DateTime[] array10 = (DateTime[])array;
				foreach (DateTime dateTime in array10)
				{
					writer.Write(dateTime.ToBinary());
				}
				return;
			}
			case TypeCode.Decimal:
			{
				decimal[] array3 = (decimal[])array;
				foreach (decimal value2 in array3)
				{
					writer.Write(value2);
				}
				return;
			}
			case TypeCode.Double:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 8);
					return;
				}
				double[] array7 = (double[])array;
				foreach (double value6 in array7)
				{
					writer.Write(value6);
				}
				return;
			}
			case TypeCode.Int16:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 2);
					return;
				}
				short[] array12 = (short[])array;
				foreach (short value10 in array12)
				{
					writer.Write(value10);
				}
				return;
			}
			case TypeCode.Int32:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 4);
					return;
				}
				int[] array6 = (int[])array;
				foreach (int value5 in array6)
				{
					writer.Write(value5);
				}
				return;
			}
			case TypeCode.Int64:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 8);
					return;
				}
				long[] array14 = (long[])array;
				foreach (long value12 in array14)
				{
					writer.Write(value12);
				}
				return;
			}
			case TypeCode.SByte:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 1);
					return;
				}
				sbyte[] array8 = (sbyte[])array;
				foreach (sbyte value7 in array8)
				{
					writer.Write(value7);
				}
				return;
			}
			case TypeCode.Single:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 4);
					return;
				}
				float[] array2 = (float[])array;
				foreach (float value in array2)
				{
					writer.Write(value);
				}
				return;
			}
			case TypeCode.UInt16:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 2);
					return;
				}
				ushort[] array13 = (ushort[])array;
				foreach (ushort value11 in array13)
				{
					writer.Write(value11);
				}
				return;
			}
			case TypeCode.UInt32:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 4);
					return;
				}
				uint[] array9 = (uint[])array;
				foreach (uint value8 in array9)
				{
					writer.Write(value8);
				}
				return;
			}
			case TypeCode.UInt64:
			{
				if (array.Length > 2)
				{
					BlockWrite(writer, array, 8);
					return;
				}
				ulong[] array5 = (ulong[])array;
				foreach (ulong value4 in array5)
				{
					writer.Write(value4);
				}
				return;
			}
			case TypeCode.String:
			{
				string[] array4 = (string[])array;
				foreach (string value3 in array4)
				{
					writer.Write(value3);
				}
				return;
			}
			}
			if (elementType == typeof(TimeSpan))
			{
				TimeSpan[] array15 = (TimeSpan[])array;
				foreach (TimeSpan timeSpan in array15)
				{
					writer.Write(timeSpan.Ticks);
				}
				return;
			}
			throw new NotSupportedException("Unsupported primitive type: " + elementType.FullName);
		}

		private void BlockWrite(BinaryWriter writer, Array array, int dataSize)
		{
			int num = Buffer.ByteLength(array);
			if (arrayBuffer == null || (num > arrayBuffer.Length && arrayBuffer.Length != ArrayBufferLength))
			{
				arrayBuffer = new byte[(num > ArrayBufferLength) ? ArrayBufferLength : num];
			}
			int num2 = 0;
			while (num > 0)
			{
				int num3 = ((num >= arrayBuffer.Length) ? arrayBuffer.Length : num);
				Buffer.BlockCopy(array, num2, arrayBuffer, 0, num3);
				if (!BitConverter.IsLittleEndian && dataSize > 1)
				{
					BinaryCommon.SwapBytes(arrayBuffer, num3, dataSize);
				}
				writer.Write(arrayBuffer, 0, num3);
				num -= num3;
				num2 += num3;
			}
		}

		private void WriteSingleDimensionArrayElements(BinaryWriter writer, Array array, Type elementType)
		{
			int num = 0;
			foreach (object item in array)
			{
				if (item != null && num > 0)
				{
					WriteNullFiller(writer, num);
					WriteValue(writer, elementType, item);
					num = 0;
				}
				else if (item == null)
				{
					num++;
				}
				else
				{
					WriteValue(writer, elementType, item);
				}
			}
			if (num > 0)
			{
				WriteNullFiller(writer, num);
			}
		}

		private void WriteNullFiller(BinaryWriter writer, int numNulls)
		{
			if (numNulls == 1)
			{
				writer.Write((byte)10);
			}
			else if (numNulls == 2)
			{
				writer.Write((byte)10);
				writer.Write((byte)10);
			}
			else if (numNulls <= 255)
			{
				writer.Write((byte)13);
				writer.Write((byte)numNulls);
			}
			else
			{
				writer.Write((byte)14);
				writer.Write(numNulls);
			}
		}

		private void WriteObjectReference(BinaryWriter writer, long id)
		{
			writer.Write((byte)9);
			writer.Write((int)id);
		}

		public void WriteValue(BinaryWriter writer, Type valueType, object val)
		{
			if (val == null)
			{
				BinaryCommon.CheckSerializable(valueType, _surrogateSelector, _context);
				writer.Write((byte)10);
			}
			else if (BinaryCommon.IsPrimitive(val.GetType()))
			{
				if (!BinaryCommon.IsPrimitive(valueType))
				{
					writer.Write((byte)8);
					WriteTypeSpec(writer, val.GetType());
				}
				WritePrimitiveValue(writer, val);
			}
			else if (valueType.IsValueType)
			{
				WriteObjectInstance(writer, val, true);
			}
			else if (val is string)
			{
				bool firstTime;
				long id = _idGenerator.GetId(val, out firstTime);
				if (firstTime)
				{
					WriteObjectInstance(writer, val, false);
				}
				else
				{
					WriteObjectReference(writer, id);
				}
			}
			else
			{
				bool firstTime2;
				long id2 = _idGenerator.GetId(val, out firstTime2);
				if (firstTime2)
				{
					_pendingObjects.Enqueue(val);
				}
				WriteObjectReference(writer, id2);
			}
		}

		private void WriteString(BinaryWriter writer, long id, string str)
		{
			writer.Write((byte)6);
			writer.Write((int)id);
			writer.Write(str);
		}

		public int WriteAssembly(BinaryWriter writer, Assembly assembly)
		{
			return WriteAssemblyName(writer, assembly.FullName);
		}

		public int WriteAssemblyName(BinaryWriter writer, string assembly)
		{
			if (assembly == CorlibAssemblyName)
			{
				return -1;
			}
			bool firstTime;
			int num = RegisterAssembly(assembly, out firstTime);
			if (!firstTime)
			{
				return num;
			}
			writer.Write((byte)12);
			writer.Write(num);
			if (_assemblyFormat == FormatterAssemblyStyle.Full)
			{
				writer.Write(assembly);
			}
			else
			{
				int num2 = assembly.IndexOf(',');
				if (num2 != -1)
				{
					assembly = assembly.Substring(0, num2);
				}
				writer.Write(assembly);
			}
			return num;
		}

		public int GetAssemblyId(Assembly assembly)
		{
			return GetAssemblyNameId(assembly.FullName);
		}

		public int GetAssemblyNameId(string assembly)
		{
			return (int)_assemblyCache[assembly];
		}

		private int RegisterAssembly(string assembly, out bool firstTime)
		{
			if (_assemblyCache.ContainsKey(assembly))
			{
				firstTime = false;
				return (int)_assemblyCache[assembly];
			}
			int num = (int)_idGenerator.GetId(0, out firstTime);
			_assemblyCache.Add(assembly, num);
			return num;
		}

		public static void WritePrimitiveValue(BinaryWriter writer, object value)
		{
			Type type = value.GetType();
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				writer.Write((bool)value);
				return;
			case TypeCode.Byte:
				writer.Write((byte)value);
				return;
			case TypeCode.Char:
				writer.Write((char)value);
				return;
			case TypeCode.DateTime:
				writer.Write(((DateTime)value).ToBinary());
				return;
			case TypeCode.Decimal:
				writer.Write(((decimal)value).ToString(CultureInfo.InvariantCulture));
				return;
			case TypeCode.Double:
				writer.Write((double)value);
				return;
			case TypeCode.Int16:
				writer.Write((short)value);
				return;
			case TypeCode.Int32:
				writer.Write((int)value);
				return;
			case TypeCode.Int64:
				writer.Write((long)value);
				return;
			case TypeCode.SByte:
				writer.Write((sbyte)value);
				return;
			case TypeCode.Single:
				writer.Write((float)value);
				return;
			case TypeCode.UInt16:
				writer.Write((ushort)value);
				return;
			case TypeCode.UInt32:
				writer.Write((uint)value);
				return;
			case TypeCode.UInt64:
				writer.Write((ulong)value);
				return;
			case TypeCode.String:
				writer.Write((string)value);
				return;
			}
			if (type == typeof(TimeSpan))
			{
				writer.Write(((TimeSpan)value).Ticks);
				return;
			}
			throw new NotSupportedException("Unsupported primitive type: " + value.GetType().FullName);
		}

		public static void WriteTypeCode(BinaryWriter writer, Type type)
		{
			writer.Write((byte)GetTypeTag(type));
		}

		public static TypeTag GetTypeTag(Type type)
		{
			if (type == typeof(string))
			{
				return TypeTag.String;
			}
			if (BinaryCommon.IsPrimitive(type))
			{
				return TypeTag.PrimitiveType;
			}
			if (type == typeof(object))
			{
				return TypeTag.ObjectType;
			}
			if (type.IsArray && type.GetArrayRank() == 1 && type.GetElementType() == typeof(object))
			{
				return TypeTag.ArrayOfObject;
			}
			if (type.IsArray && type.GetArrayRank() == 1 && type.GetElementType() == typeof(string))
			{
				return TypeTag.ArrayOfString;
			}
			if (type.IsArray && type.GetArrayRank() == 1 && BinaryCommon.IsPrimitive(type.GetElementType()))
			{
				return TypeTag.ArrayOfPrimitiveType;
			}
			if (type.Assembly == CorlibAssembly)
			{
				return TypeTag.RuntimeType;
			}
			return TypeTag.GenericType;
		}

		public void WriteTypeSpec(BinaryWriter writer, Type type)
		{
			switch (GetTypeTag(type))
			{
			case TypeTag.PrimitiveType:
				writer.Write(BinaryCommon.GetTypeCode(type));
				break;
			case TypeTag.RuntimeType:
			{
				string value = type.FullName;
				if (_context.State == StreamingContextStates.Remoting)
				{
					if (type == typeof(MonoType))
					{
						value = "System.RuntimeType";
					}
					else if (type == typeof(MonoType[]))
					{
						value = "System.RuntimeType[]";
					}
				}
				writer.Write(value);
				break;
			}
			case TypeTag.GenericType:
				writer.Write(type.FullName);
				writer.Write(GetAssemblyId(type.Assembly));
				break;
			case TypeTag.ArrayOfPrimitiveType:
				writer.Write(BinaryCommon.GetTypeCode(type.GetElementType()));
				break;
			case TypeTag.String:
			case TypeTag.ObjectType:
			case TypeTag.ArrayOfObject:
			case TypeTag.ArrayOfString:
				break;
			}
		}
	}
}
