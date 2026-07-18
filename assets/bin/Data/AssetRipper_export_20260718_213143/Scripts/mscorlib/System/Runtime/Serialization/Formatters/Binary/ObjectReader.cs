using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class ObjectReader
	{
		private class TypeMetadata
		{
			public Type Type;

			public Type[] MemberTypes;

			public string[] MemberNames;

			public MemberInfo[] MemberInfos;

			public int FieldCount;

			public bool NeedsSerializationInfo;
		}

		private class ArrayNullFiller
		{
			public int NullCount;

			public ArrayNullFiller(int count)
			{
				NullCount = count;
			}
		}

		private ISurrogateSelector _surrogateSelector;

		private StreamingContext _context;

		private SerializationBinder _binder;

		private TypeFilterLevel _filterLevel;

		private ObjectManager _manager;

		private Hashtable _registeredAssemblies = new Hashtable();

		private Hashtable _typeMetadataCache = new Hashtable();

		private object _lastObject;

		private long _lastObjectID;

		private long _rootObjectID;

		private byte[] arrayBuffer;

		private int ArrayBufferLength = 4096;

		public object CurrentObject
		{
			get
			{
				return _lastObject;
			}
		}

		public ObjectReader(BinaryFormatter formatter)
		{
			_surrogateSelector = formatter.SurrogateSelector;
			_context = formatter.Context;
			_binder = formatter.Binder;
			_manager = new ObjectManager(_surrogateSelector, _context);
			_filterLevel = formatter.FilterLevel;
		}

		public void ReadObjectGraph(BinaryReader reader, bool readHeaders, out object result, out Header[] headers)
		{
			BinaryElement elem = (BinaryElement)reader.ReadByte();
			ReadObjectGraph(elem, reader, readHeaders, out result, out headers);
		}

		public void ReadObjectGraph(BinaryElement elem, BinaryReader reader, bool readHeaders, out object result, out Header[] headers)
		{
			headers = null;
			if (ReadNextObject(elem, reader))
			{
				do
				{
					if (readHeaders && headers == null)
					{
						headers = (Header[])CurrentObject;
					}
					else if (_rootObjectID == 0L)
					{
						_rootObjectID = _lastObjectID;
					}
				}
				while (ReadNextObject(reader));
			}
			result = _manager.GetObject(_rootObjectID);
		}

		private bool ReadNextObject(BinaryElement element, BinaryReader reader)
		{
			if (element == BinaryElement.End)
			{
				_manager.DoFixups();
				_manager.RaiseDeserializationEvent();
				return false;
			}
			long objectId;
			SerializationInfo info;
			ReadObject(element, reader, out objectId, out _lastObject, out info);
			if (objectId != 0L)
			{
				RegisterObject(objectId, _lastObject, info, 0L, null, null);
				_lastObjectID = objectId;
			}
			return true;
		}

		public bool ReadNextObject(BinaryReader reader)
		{
			BinaryElement binaryElement = (BinaryElement)reader.ReadByte();
			if (binaryElement == BinaryElement.End)
			{
				_manager.DoFixups();
				_manager.RaiseDeserializationEvent();
				return false;
			}
			long objectId;
			SerializationInfo info;
			ReadObject(binaryElement, reader, out objectId, out _lastObject, out info);
			if (objectId != 0L)
			{
				RegisterObject(objectId, _lastObject, info, 0L, null, null);
				_lastObjectID = objectId;
			}
			return true;
		}

		private void ReadObject(BinaryElement element, BinaryReader reader, out long objectId, out object value, out SerializationInfo info)
		{
			switch (element)
			{
			case BinaryElement.RefTypeObject:
				ReadRefTypeObjectInstance(reader, out objectId, out value, out info);
				break;
			case BinaryElement.UntypedRuntimeObject:
				ReadObjectInstance(reader, true, false, out objectId, out value, out info);
				break;
			case BinaryElement.UntypedExternalObject:
				ReadObjectInstance(reader, false, false, out objectId, out value, out info);
				break;
			case BinaryElement.RuntimeObject:
				ReadObjectInstance(reader, true, true, out objectId, out value, out info);
				break;
			case BinaryElement.ExternalObject:
				ReadObjectInstance(reader, false, true, out objectId, out value, out info);
				break;
			case BinaryElement.String:
				info = null;
				ReadStringIntance(reader, out objectId, out value);
				break;
			case BinaryElement.GenericArray:
				info = null;
				ReadGenericArray(reader, out objectId, out value);
				break;
			case BinaryElement.BoxedPrimitiveTypeValue:
				value = ReadBoxedPrimitiveTypeValue(reader);
				objectId = 0L;
				info = null;
				break;
			case BinaryElement.NullValue:
				value = null;
				objectId = 0L;
				info = null;
				break;
			case BinaryElement.Assembly:
				ReadAssembly(reader);
				ReadObject((BinaryElement)reader.ReadByte(), reader, out objectId, out value, out info);
				break;
			case BinaryElement.ArrayFiller8b:
				value = new ArrayNullFiller(reader.ReadByte());
				objectId = 0L;
				info = null;
				break;
			case BinaryElement.ArrayFiller32b:
				value = new ArrayNullFiller(reader.ReadInt32());
				objectId = 0L;
				info = null;
				break;
			case BinaryElement.ArrayOfPrimitiveType:
				ReadArrayOfPrimitiveType(reader, out objectId, out value);
				info = null;
				break;
			case BinaryElement.ArrayOfObject:
				ReadArrayOfObject(reader, out objectId, out value);
				info = null;
				break;
			case BinaryElement.ArrayOfString:
				ReadArrayOfString(reader, out objectId, out value);
				info = null;
				break;
			default:
				throw new SerializationException("Unexpected binary element: " + (int)element);
			}
		}

		private void ReadAssembly(BinaryReader reader)
		{
			long num = reader.ReadUInt32();
			string value = reader.ReadString();
			_registeredAssemblies[num] = value;
		}

		private void ReadObjectInstance(BinaryReader reader, bool isRuntimeObject, bool hasTypeInfo, out long objectId, out object value, out SerializationInfo info)
		{
			objectId = reader.ReadUInt32();
			TypeMetadata metadata = ReadTypeMetadata(reader, isRuntimeObject, hasTypeInfo);
			ReadObjectContent(reader, metadata, objectId, out value, out info);
		}

		private void ReadRefTypeObjectInstance(BinaryReader reader, out long objectId, out object value, out SerializationInfo info)
		{
			objectId = reader.ReadUInt32();
			long objectID = reader.ReadUInt32();
			object obj = _manager.GetObject(objectID);
			if (obj == null)
			{
				throw new SerializationException("Invalid binary format");
			}
			TypeMetadata metadata = (TypeMetadata)_typeMetadataCache[obj.GetType()];
			ReadObjectContent(reader, metadata, objectId, out value, out info);
		}

		private void ReadObjectContent(BinaryReader reader, TypeMetadata metadata, long objectId, out object objectInstance, out SerializationInfo info)
		{
			if (_filterLevel == TypeFilterLevel.Low)
			{
				objectInstance = FormatterServices.GetSafeUninitializedObject(metadata.Type);
			}
			else
			{
				objectInstance = FormatterServices.GetUninitializedObject(metadata.Type);
			}
			_manager.RaiseOnDeserializingEvent(objectInstance);
			info = ((!metadata.NeedsSerializationInfo) ? null : new SerializationInfo(metadata.Type, new FormatterConverter()));
			if (metadata.MemberNames != null)
			{
				for (int i = 0; i < metadata.FieldCount; i++)
				{
					ReadValue(reader, objectInstance, objectId, info, metadata.MemberTypes[i], metadata.MemberNames[i], null, null);
				}
			}
			else
			{
				for (int j = 0; j < metadata.FieldCount; j++)
				{
					ReadValue(reader, objectInstance, objectId, info, metadata.MemberTypes[j], metadata.MemberInfos[j].Name, metadata.MemberInfos[j], null);
				}
			}
		}

		private void RegisterObject(long objectId, object objectInstance, SerializationInfo info, long parentObjectId, MemberInfo parentObjectMemeber, int[] indices)
		{
			if (parentObjectId == 0L)
			{
				indices = null;
			}
			if (!objectInstance.GetType().IsValueType || parentObjectId == 0L)
			{
				_manager.RegisterObject(objectInstance, objectId, info, 0L, null, null);
				return;
			}
			if (indices != null)
			{
				indices = (int[])indices.Clone();
			}
			_manager.RegisterObject(objectInstance, objectId, info, parentObjectId, parentObjectMemeber, indices);
		}

		private void ReadStringIntance(BinaryReader reader, out long objectId, out object value)
		{
			objectId = reader.ReadUInt32();
			value = reader.ReadString();
		}

		private void ReadGenericArray(BinaryReader reader, out long objectId, out object val)
		{
			objectId = reader.ReadUInt32();
			reader.ReadByte();
			int num = reader.ReadInt32();
			bool flag = false;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadInt32();
				if (array[i] == 0)
				{
					flag = true;
				}
			}
			TypeTag code = (TypeTag)reader.ReadByte();
			Type type = ReadType(reader, code);
			Array array2 = Array.CreateInstance(type, array);
			if (flag)
			{
				val = array2;
				return;
			}
			int[] array3 = new int[num];
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				array3[num2] = array2.GetLowerBound(num2);
			}
			bool flag2 = false;
			while (!flag2)
			{
				ReadValue(reader, array2, objectId, null, type, null, null, array3);
				int num3 = array2.Rank - 1;
				while (num3 >= 0)
				{
					array3[num3]++;
					if (array3[num3] > array2.GetUpperBound(num3))
					{
						if (num3 > 0)
						{
							array3[num3] = array2.GetLowerBound(num3);
							num3--;
							continue;
						}
						flag2 = true;
						break;
					}
					break;
				}
			}
			val = array2;
		}

		private object ReadBoxedPrimitiveTypeValue(BinaryReader reader)
		{
			Type type = ReadType(reader, TypeTag.PrimitiveType);
			return ReadPrimitiveTypeValue(reader, type);
		}

		private void ReadArrayOfPrimitiveType(BinaryReader reader, out long objectId, out object val)
		{
			objectId = reader.ReadUInt32();
			int num = reader.ReadInt32();
			Type type = ReadType(reader, TypeTag.PrimitiveType);
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
			{
				bool[] array11 = new bool[num];
				for (int num8 = 0; num8 < num; num8++)
				{
					array11[num8] = reader.ReadBoolean();
				}
				val = array11;
				return;
			}
			case TypeCode.Byte:
			{
				byte[] array10 = new byte[num];
				int num7;
				for (int num6 = 0; num6 < num; num6 += num7)
				{
					num7 = reader.Read(array10, num6, num - num6);
					if (num7 == 0)
					{
						break;
					}
				}
				val = array10;
				return;
			}
			case TypeCode.Char:
			{
				char[] array3 = new char[num];
				int num2;
				for (int k = 0; k < num; k += num2)
				{
					num2 = reader.Read(array3, k, num - k);
					if (num2 == 0)
					{
						break;
					}
				}
				val = array3;
				return;
			}
			case TypeCode.DateTime:
			{
				DateTime[] array = new DateTime[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = DateTime.FromBinary(reader.ReadInt64());
				}
				val = array;
				return;
			}
			case TypeCode.Decimal:
			{
				decimal[] array4 = new decimal[num];
				for (int l = 0; l < num; l++)
				{
					array4[l] = reader.ReadDecimal();
				}
				val = array4;
				return;
			}
			case TypeCode.Double:
			{
				double[] array12 = new double[num];
				if (num > 2)
				{
					BlockRead(reader, array12, 8);
				}
				else
				{
					for (int num9 = 0; num9 < num; num9++)
					{
						array12[num9] = reader.ReadDouble();
					}
				}
				val = array12;
				return;
			}
			case TypeCode.Int16:
			{
				short[] array14 = new short[num];
				if (num > 2)
				{
					BlockRead(reader, array14, 2);
				}
				else
				{
					for (int num11 = 0; num11 < num; num11++)
					{
						array14[num11] = reader.ReadInt16();
					}
				}
				val = array14;
				return;
			}
			case TypeCode.Int32:
			{
				int[] array6 = new int[num];
				if (num > 2)
				{
					BlockRead(reader, array6, 4);
				}
				else
				{
					for (int n = 0; n < num; n++)
					{
						array6[n] = reader.ReadInt32();
					}
				}
				val = array6;
				return;
			}
			case TypeCode.Int64:
			{
				long[] array7 = new long[num];
				if (num > 2)
				{
					BlockRead(reader, array7, 8);
				}
				else
				{
					for (int num3 = 0; num3 < num; num3++)
					{
						array7[num3] = reader.ReadInt64();
					}
				}
				val = array7;
				return;
			}
			case TypeCode.SByte:
			{
				sbyte[] array15 = new sbyte[num];
				if (num > 2)
				{
					BlockRead(reader, array15, 1);
				}
				else
				{
					for (int num12 = 0; num12 < num; num12++)
					{
						array15[num12] = reader.ReadSByte();
					}
				}
				val = array15;
				return;
			}
			case TypeCode.Single:
			{
				float[] array9 = new float[num];
				if (num > 2)
				{
					BlockRead(reader, array9, 4);
				}
				else
				{
					for (int num5 = 0; num5 < num; num5++)
					{
						array9[num5] = reader.ReadSingle();
					}
				}
				val = array9;
				return;
			}
			case TypeCode.UInt16:
			{
				ushort[] array2 = new ushort[num];
				if (num > 2)
				{
					BlockRead(reader, array2, 2);
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						array2[j] = reader.ReadUInt16();
					}
				}
				val = array2;
				return;
			}
			case TypeCode.UInt32:
			{
				uint[] array13 = new uint[num];
				if (num > 2)
				{
					BlockRead(reader, array13, 4);
				}
				else
				{
					for (int num10 = 0; num10 < num; num10++)
					{
						array13[num10] = reader.ReadUInt32();
					}
				}
				val = array13;
				return;
			}
			case TypeCode.UInt64:
			{
				ulong[] array8 = new ulong[num];
				if (num > 2)
				{
					BlockRead(reader, array8, 8);
				}
				else
				{
					for (int num4 = 0; num4 < num; num4++)
					{
						array8[num4] = reader.ReadUInt64();
					}
				}
				val = array8;
				return;
			}
			case TypeCode.String:
			{
				string[] array5 = new string[num];
				for (int m = 0; m < num; m++)
				{
					array5[m] = reader.ReadString();
				}
				val = array5;
				return;
			}
			}
			if (type == typeof(TimeSpan))
			{
				TimeSpan[] array16 = new TimeSpan[num];
				for (int num13 = 0; num13 < num; num13++)
				{
					array16[num13] = new TimeSpan(reader.ReadInt64());
				}
				val = array16;
				return;
			}
			throw new NotSupportedException("Unsupported primitive type: " + type.FullName);
		}

		private void BlockRead(BinaryReader reader, Array array, int dataSize)
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
				int num4 = 0;
				do
				{
					int num5 = reader.Read(arrayBuffer, num4, num3 - num4);
					if (num5 == 0)
					{
						break;
					}
					num4 += num5;
				}
				while (num4 < num3);
				if (!BitConverter.IsLittleEndian && dataSize > 1)
				{
					BinaryCommon.SwapBytes(arrayBuffer, num3, dataSize);
				}
				Buffer.BlockCopy(arrayBuffer, 0, array, num2, num3);
				num -= num3;
				num2 += num3;
			}
		}

		private void ReadArrayOfObject(BinaryReader reader, out long objectId, out object array)
		{
			ReadSimpleArray(reader, typeof(object), out objectId, out array);
		}

		private void ReadArrayOfString(BinaryReader reader, out long objectId, out object array)
		{
			ReadSimpleArray(reader, typeof(string), out objectId, out array);
		}

		private void ReadSimpleArray(BinaryReader reader, Type elementType, out long objectId, out object val)
		{
			objectId = reader.ReadUInt32();
			int num = reader.ReadInt32();
			int[] array = new int[1];
			Array array2 = Array.CreateInstance(elementType, num);
			int num2;
			for (num2 = 0; num2 < num; num2++)
			{
				array[0] = num2;
				ReadValue(reader, array2, objectId, null, elementType, null, null, array);
				num2 = array[0];
			}
			val = array2;
		}

		private TypeMetadata ReadTypeMetadata(BinaryReader reader, bool isRuntimeObject, bool hasTypeInfo)
		{
			TypeMetadata typeMetadata = new TypeMetadata();
			string text = reader.ReadString();
			int num = reader.ReadInt32();
			Type[] array = new Type[num];
			string[] array2 = new string[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = reader.ReadString();
			}
			if (hasTypeInfo)
			{
				TypeTag[] array3 = new TypeTag[num];
				for (int j = 0; j < num; j++)
				{
					array3[j] = (TypeTag)reader.ReadByte();
				}
				for (int k = 0; k < num; k++)
				{
					array[k] = ReadType(reader, array3[k]);
				}
			}
			if (!isRuntimeObject)
			{
				long assemblyId = reader.ReadUInt32();
				typeMetadata.Type = GetDeserializationType(assemblyId, text);
			}
			else
			{
				typeMetadata.Type = Type.GetType(text, true);
			}
			typeMetadata.MemberTypes = array;
			typeMetadata.MemberNames = array2;
			typeMetadata.FieldCount = array2.Length;
			if (_surrogateSelector != null)
			{
				ISurrogateSelector selector;
				ISerializationSurrogate surrogate = _surrogateSelector.GetSurrogate(typeMetadata.Type, _context, out selector);
				typeMetadata.NeedsSerializationInfo = surrogate != null;
			}
			if (!typeMetadata.NeedsSerializationInfo)
			{
				if (!typeMetadata.Type.IsSerializable)
				{
					throw new SerializationException("Serializable objects must be marked with the Serializable attribute");
				}
				typeMetadata.NeedsSerializationInfo = typeof(ISerializable).IsAssignableFrom(typeMetadata.Type);
				if (!typeMetadata.NeedsSerializationInfo)
				{
					typeMetadata.MemberInfos = new MemberInfo[num];
					for (int l = 0; l < num; l++)
					{
						FieldInfo fieldInfo = null;
						string text2 = array2[l];
						int num2 = text2.IndexOf('+');
						if (num2 != -1)
						{
							string text3 = array2[l].Substring(0, num2);
							text2 = array2[l].Substring(num2 + 1);
							for (Type baseType = typeMetadata.Type.BaseType; baseType != null; baseType = baseType.BaseType)
							{
								if (baseType.Name == text3)
								{
									fieldInfo = baseType.GetField(text2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
									break;
								}
							}
						}
						else
						{
							fieldInfo = typeMetadata.Type.GetField(text2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						}
						if (fieldInfo == null)
						{
							throw new SerializationException("Field \"" + array2[l] + "\" not found in class " + typeMetadata.Type.FullName);
						}
						typeMetadata.MemberInfos[l] = fieldInfo;
						if (!hasTypeInfo)
						{
							array[l] = fieldInfo.FieldType;
						}
					}
					typeMetadata.MemberNames = null;
				}
			}
			if (!_typeMetadataCache.ContainsKey(typeMetadata.Type))
			{
				_typeMetadataCache[typeMetadata.Type] = typeMetadata;
			}
			return typeMetadata;
		}

		private void ReadValue(BinaryReader reader, object parentObject, long parentObjectId, SerializationInfo info, Type valueType, string fieldName, MemberInfo memberInfo, int[] indices)
		{
			object value;
			if (BinaryCommon.IsPrimitive(valueType))
			{
				value = ReadPrimitiveTypeValue(reader, valueType);
				SetObjectValue(parentObject, fieldName, memberInfo, info, value, valueType, indices);
				return;
			}
			BinaryElement binaryElement = (BinaryElement)reader.ReadByte();
			if (binaryElement == BinaryElement.ObjectReference)
			{
				long childObjectId = reader.ReadUInt32();
				RecordFixup(parentObjectId, childObjectId, parentObject, info, fieldName, memberInfo, indices);
				return;
			}
			long objectId;
			SerializationInfo info2;
			ReadObject(binaryElement, reader, out objectId, out value, out info2);
			bool flag = false;
			if (objectId != 0L)
			{
				if (value.GetType().IsValueType)
				{
					RecordFixup(parentObjectId, objectId, parentObject, info, fieldName, memberInfo, indices);
					flag = true;
				}
				if (info == null && !(parentObject is Array))
				{
					RegisterObject(objectId, value, info2, parentObjectId, memberInfo, null);
				}
				else
				{
					RegisterObject(objectId, value, info2, parentObjectId, null, indices);
				}
			}
			if (!flag)
			{
				SetObjectValue(parentObject, fieldName, memberInfo, info, value, valueType, indices);
			}
		}

		private void SetObjectValue(object parentObject, string fieldName, MemberInfo memberInfo, SerializationInfo info, object value, Type valueType, int[] indices)
		{
			if (value is IObjectReference)
			{
				value = ((IObjectReference)value).GetRealObject(_context);
			}
			if (parentObject is Array)
			{
				if (value is ArrayNullFiller)
				{
					int nullCount = ((ArrayNullFiller)value).NullCount;
					indices[0] += nullCount - 1;
				}
				else
				{
					((Array)parentObject).SetValue(value, indices);
				}
			}
			else if (info != null)
			{
				info.AddValue(fieldName, value, valueType);
			}
			else if (memberInfo is FieldInfo)
			{
				((FieldInfo)memberInfo).SetValue(parentObject, value);
			}
			else
			{
				((PropertyInfo)memberInfo).SetValue(parentObject, value, null);
			}
		}

		private void RecordFixup(long parentObjectId, long childObjectId, object parentObject, SerializationInfo info, string fieldName, MemberInfo memberInfo, int[] indices)
		{
			if (info != null)
			{
				_manager.RecordDelayedFixup(parentObjectId, fieldName, childObjectId);
			}
			else if (parentObject is Array)
			{
				if (indices.Length == 1)
				{
					_manager.RecordArrayElementFixup(parentObjectId, indices[0], childObjectId);
				}
				else
				{
					_manager.RecordArrayElementFixup(parentObjectId, (int[])indices.Clone(), childObjectId);
				}
			}
			else
			{
				_manager.RecordFixup(parentObjectId, memberInfo, childObjectId);
			}
		}

		private Type GetDeserializationType(long assemblyId, string className)
		{
			string text = (string)_registeredAssemblies[assemblyId];
			Type type;
			if (_binder != null)
			{
				type = _binder.BindToType(text, className);
				if (type != null)
				{
					return type;
				}
			}
			Assembly assembly = Assembly.Load(text);
			type = assembly.GetType(className, true);
			if (type != null)
			{
				return type;
			}
			throw new SerializationException("Couldn't find type '" + className + "'.");
		}

		public Type ReadType(BinaryReader reader, TypeTag code)
		{
			switch (code)
			{
			case TypeTag.PrimitiveType:
				return BinaryCommon.GetTypeFromCode(reader.ReadByte());
			case TypeTag.String:
				return typeof(string);
			case TypeTag.ObjectType:
				return typeof(object);
			case TypeTag.RuntimeType:
			{
				string text = reader.ReadString();
				if (_context.State == StreamingContextStates.Remoting)
				{
					if (text == "System.RuntimeType")
					{
						return typeof(MonoType);
					}
					if (text == "System.RuntimeType[]")
					{
						return typeof(MonoType[]);
					}
				}
				Type type = Type.GetType(text);
				if (type != null)
				{
					return type;
				}
				throw new SerializationException(string.Format("Could not find type '{0}'.", text));
			}
			case TypeTag.GenericType:
			{
				string className = reader.ReadString();
				long assemblyId = reader.ReadUInt32();
				return GetDeserializationType(assemblyId, className);
			}
			case TypeTag.ArrayOfObject:
				return typeof(object[]);
			case TypeTag.ArrayOfString:
				return typeof(string[]);
			case TypeTag.ArrayOfPrimitiveType:
			{
				Type typeFromCode = BinaryCommon.GetTypeFromCode(reader.ReadByte());
				return Type.GetType(typeFromCode.FullName + "[]");
			}
			default:
				throw new NotSupportedException("Unknow type tag");
			}
		}

		public static object ReadPrimitiveTypeValue(BinaryReader reader, Type type)
		{
			if (type == null)
			{
				return null;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return reader.ReadBoolean();
			case TypeCode.Byte:
				return reader.ReadByte();
			case TypeCode.Char:
				return reader.ReadChar();
			case TypeCode.DateTime:
				return DateTime.FromBinary(reader.ReadInt64());
			case TypeCode.Decimal:
				return decimal.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
			case TypeCode.Double:
				return reader.ReadDouble();
			case TypeCode.Int16:
				return reader.ReadInt16();
			case TypeCode.Int32:
				return reader.ReadInt32();
			case TypeCode.Int64:
				return reader.ReadInt64();
			case TypeCode.SByte:
				return reader.ReadSByte();
			case TypeCode.Single:
				return reader.ReadSingle();
			case TypeCode.UInt16:
				return reader.ReadUInt16();
			case TypeCode.UInt32:
				return reader.ReadUInt32();
			case TypeCode.UInt64:
				return reader.ReadUInt64();
			case TypeCode.String:
				return reader.ReadString();
			default:
				if (type == typeof(TimeSpan))
				{
					return new TimeSpan(reader.ReadInt64());
				}
				throw new NotSupportedException("Unsupported primitive type: " + type.FullName);
			}
		}
	}
}
