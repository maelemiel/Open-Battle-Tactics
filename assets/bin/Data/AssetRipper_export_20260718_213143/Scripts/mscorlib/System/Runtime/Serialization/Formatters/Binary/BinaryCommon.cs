namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class BinaryCommon
	{
		public static byte[] BinaryHeader;

		private static Type[] _typeCodesToType;

		private static byte[] _typeCodeMap;

		public static bool UseReflectionSerialization;

		static BinaryCommon()
		{
			BinaryHeader = new byte[17]
			{
				0, 1, 0, 0, 0, 255, 255, 255, 255, 1,
				0, 0, 0, 0, 0, 0, 0
			};
			UseReflectionSerialization = false;
			_typeCodesToType = new Type[19];
			_typeCodesToType[1] = typeof(bool);
			_typeCodesToType[2] = typeof(byte);
			_typeCodesToType[3] = typeof(char);
			_typeCodesToType[12] = typeof(TimeSpan);
			_typeCodesToType[13] = typeof(DateTime);
			_typeCodesToType[5] = typeof(decimal);
			_typeCodesToType[6] = typeof(double);
			_typeCodesToType[7] = typeof(short);
			_typeCodesToType[8] = typeof(int);
			_typeCodesToType[9] = typeof(long);
			_typeCodesToType[10] = typeof(sbyte);
			_typeCodesToType[11] = typeof(float);
			_typeCodesToType[14] = typeof(ushort);
			_typeCodesToType[15] = typeof(uint);
			_typeCodesToType[16] = typeof(ulong);
			_typeCodesToType[17] = null;
			_typeCodesToType[18] = typeof(string);
			_typeCodeMap = new byte[30];
			_typeCodeMap[3] = 1;
			_typeCodeMap[6] = 2;
			_typeCodeMap[4] = 3;
			_typeCodeMap[16] = 13;
			_typeCodeMap[15] = 5;
			_typeCodeMap[14] = 6;
			_typeCodeMap[7] = 7;
			_typeCodeMap[9] = 8;
			_typeCodeMap[11] = 9;
			_typeCodeMap[5] = 10;
			_typeCodeMap[13] = 11;
			_typeCodeMap[8] = 14;
			_typeCodeMap[10] = 15;
			_typeCodeMap[12] = 16;
			_typeCodeMap[18] = 18;
			string text = Environment.GetEnvironmentVariable("MONO_REFLECTION_SERIALIZER");
			if (text == null)
			{
				text = "no";
			}
			UseReflectionSerialization = text != "no";
		}

		public static bool IsPrimitive(Type type)
		{
			return (type.IsPrimitive && type != typeof(IntPtr)) || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(decimal);
		}

		public static byte GetTypeCode(Type type)
		{
			if (type == typeof(TimeSpan))
			{
				return 12;
			}
			return _typeCodeMap[(int)Type.GetTypeCode(type)];
		}

		public static Type GetTypeFromCode(int code)
		{
			return _typeCodesToType[code];
		}

		public static void CheckSerializable(Type type, ISurrogateSelector selector, StreamingContext context)
		{
			if (!type.IsSerializable && !type.IsInterface && (selector == null || selector.GetSurrogate(type, context, out selector) == null))
			{
				throw new SerializationException(string.Concat("Type ", type, " is not marked as Serializable."));
			}
		}

		public static void SwapBytes(byte[] byteArray, int size, int dataSize)
		{
			switch (dataSize)
			{
			case 8:
			{
				for (int j = 0; j < size; j += 8)
				{
					byte b = byteArray[j];
					byteArray[j] = byteArray[j + 7];
					byteArray[j + 7] = b;
					b = byteArray[j + 1];
					byteArray[j + 1] = byteArray[j + 6];
					byteArray[j + 6] = b;
					b = byteArray[j + 2];
					byteArray[j + 2] = byteArray[j + 5];
					byteArray[j + 5] = b;
					b = byteArray[j + 3];
					byteArray[j + 3] = byteArray[j + 4];
					byteArray[j + 4] = b;
				}
				break;
			}
			case 4:
			{
				for (int k = 0; k < size; k += 4)
				{
					byte b = byteArray[k];
					byteArray[k] = byteArray[k + 3];
					byteArray[k + 3] = b;
					b = byteArray[k + 1];
					byteArray[k + 1] = byteArray[k + 2];
					byteArray[k + 2] = b;
				}
				break;
			}
			case 2:
			{
				for (int i = 0; i < size; i += 2)
				{
					byte b = byteArray[i];
					byteArray[i] = byteArray[i + 1];
					byteArray[i + 1] = b;
				}
				break;
			}
			}
		}
	}
}
