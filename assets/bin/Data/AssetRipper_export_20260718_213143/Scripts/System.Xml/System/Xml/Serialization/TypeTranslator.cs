using System.Collections;
using System.Globalization;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class TypeTranslator
	{
		private static Hashtable nameCache;

		private static Hashtable primitiveTypes;

		private static Hashtable primitiveArrayTypes;

		private static Hashtable nullableTypes;

		static TypeTranslator()
		{
			nameCache = new Hashtable();
			primitiveArrayTypes = Hashtable.Synchronized(new Hashtable());
			nameCache = Hashtable.Synchronized(nameCache);
			nameCache.Add(typeof(bool), new TypeData(typeof(bool), "boolean", true));
			nameCache.Add(typeof(short), new TypeData(typeof(short), "short", true));
			nameCache.Add(typeof(ushort), new TypeData(typeof(ushort), "unsignedShort", true));
			nameCache.Add(typeof(int), new TypeData(typeof(int), "int", true));
			nameCache.Add(typeof(uint), new TypeData(typeof(uint), "unsignedInt", true));
			nameCache.Add(typeof(long), new TypeData(typeof(long), "long", true));
			nameCache.Add(typeof(ulong), new TypeData(typeof(ulong), "unsignedLong", true));
			nameCache.Add(typeof(float), new TypeData(typeof(float), "float", true));
			nameCache.Add(typeof(double), new TypeData(typeof(double), "double", true));
			nameCache.Add(typeof(DateTime), new TypeData(typeof(DateTime), "dateTime", true));
			nameCache.Add(typeof(decimal), new TypeData(typeof(decimal), "decimal", true));
			nameCache.Add(typeof(XmlQualifiedName), new TypeData(typeof(XmlQualifiedName), "QName", true));
			nameCache.Add(typeof(string), new TypeData(typeof(string), "string", true));
			XmlSchemaPatternFacet facet = new XmlSchemaPatternFacet
			{
				Value = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}"
			};
			nameCache.Add(typeof(Guid), new TypeData(typeof(Guid), "guid", true, (TypeData)nameCache[typeof(string)], facet));
			nameCache.Add(typeof(byte), new TypeData(typeof(byte), "unsignedByte", true));
			nameCache.Add(typeof(sbyte), new TypeData(typeof(sbyte), "byte", true));
			nameCache.Add(typeof(char), new TypeData(typeof(char), "char", true, (TypeData)nameCache[typeof(ushort)], null));
			nameCache.Add(typeof(object), new TypeData(typeof(object), "anyType", false));
			nameCache.Add(typeof(byte[]), new TypeData(typeof(byte[]), "base64Binary", true));
			nameCache.Add(typeof(XmlNode), new TypeData(typeof(XmlNode), "XmlNode", false));
			nameCache.Add(typeof(XmlElement), new TypeData(typeof(XmlElement), "XmlElement", false));
			primitiveTypes = new Hashtable();
			ICollection values = nameCache.Values;
			foreach (TypeData item in values)
			{
				primitiveTypes.Add(item.XmlType, item);
			}
			primitiveTypes.Add("date", new TypeData(typeof(DateTime), "date", true));
			primitiveTypes.Add("time", new TypeData(typeof(DateTime), "time", true));
			primitiveTypes.Add("timePeriod", new TypeData(typeof(DateTime), "timePeriod", true));
			primitiveTypes.Add("gDay", new TypeData(typeof(string), "gDay", true));
			primitiveTypes.Add("gMonthDay", new TypeData(typeof(string), "gMonthDay", true));
			primitiveTypes.Add("gYear", new TypeData(typeof(string), "gYear", true));
			primitiveTypes.Add("gYearMonth", new TypeData(typeof(string), "gYearMonth", true));
			primitiveTypes.Add("month", new TypeData(typeof(DateTime), "month", true));
			primitiveTypes.Add("NMTOKEN", new TypeData(typeof(string), "NMTOKEN", true));
			primitiveTypes.Add("NMTOKENS", new TypeData(typeof(string), "NMTOKENS", true));
			primitiveTypes.Add("Name", new TypeData(typeof(string), "Name", true));
			primitiveTypes.Add("NCName", new TypeData(typeof(string), "NCName", true));
			primitiveTypes.Add("language", new TypeData(typeof(string), "language", true));
			primitiveTypes.Add("integer", new TypeData(typeof(string), "integer", true));
			primitiveTypes.Add("positiveInteger", new TypeData(typeof(string), "positiveInteger", true));
			primitiveTypes.Add("nonPositiveInteger", new TypeData(typeof(string), "nonPositiveInteger", true));
			primitiveTypes.Add("negativeInteger", new TypeData(typeof(string), "negativeInteger", true));
			primitiveTypes.Add("nonNegativeInteger", new TypeData(typeof(string), "nonNegativeInteger", true));
			primitiveTypes.Add("ENTITIES", new TypeData(typeof(string), "ENTITIES", true));
			primitiveTypes.Add("ENTITY", new TypeData(typeof(string), "ENTITY", true));
			primitiveTypes.Add("hexBinary", new TypeData(typeof(byte[]), "hexBinary", true));
			primitiveTypes.Add("ID", new TypeData(typeof(string), "ID", true));
			primitiveTypes.Add("IDREF", new TypeData(typeof(string), "IDREF", true));
			primitiveTypes.Add("IDREFS", new TypeData(typeof(string), "IDREFS", true));
			primitiveTypes.Add("NOTATION", new TypeData(typeof(string), "NOTATION", true));
			primitiveTypes.Add("token", new TypeData(typeof(string), "token", true));
			primitiveTypes.Add("normalizedString", new TypeData(typeof(string), "normalizedString", true));
			primitiveTypes.Add("anyURI", new TypeData(typeof(string), "anyURI", true));
			primitiveTypes.Add("base64", new TypeData(typeof(byte[]), "base64", true));
			primitiveTypes.Add("duration", new TypeData(typeof(string), "duration", true));
			nullableTypes = Hashtable.Synchronized(new Hashtable());
			foreach (DictionaryEntry primitiveType in primitiveTypes)
			{
				TypeData typeData2 = (TypeData)primitiveType.Value;
				TypeData value = new TypeData(typeData2.Type, typeData2.XmlType, true)
				{
					IsNullable = true
				};
				nullableTypes.Add(primitiveType.Key, value);
			}
		}

		public static TypeData GetTypeData(Type type)
		{
			return GetTypeData(type, null);
		}

		public static TypeData GetTypeData(Type runtimeType, string xmlDataType)
		{
			Type type = runtimeType;
			bool flag = false;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				flag = true;
				type = type.GetGenericArguments()[0];
				TypeData typeData = GetTypeData(type);
				if (typeData != null)
				{
					TypeData typeData2 = (TypeData)nullableTypes[typeData.XmlType];
					if (typeData2 == null)
					{
						typeData2 = new TypeData(type, typeData.XmlType, false);
						typeData2.IsNullable = true;
						nullableTypes[typeData.XmlType] = typeData2;
					}
					return typeData2;
				}
			}
			if (xmlDataType != null && xmlDataType.Length != 0)
			{
				TypeData primitiveTypeData = GetPrimitiveTypeData(xmlDataType);
				if (type.IsArray && type != primitiveTypeData.Type)
				{
					TypeData typeData3 = (TypeData)primitiveArrayTypes[xmlDataType];
					if (typeData3 != null)
					{
						return typeData3;
					}
					if (primitiveTypeData.Type == type.GetElementType())
					{
						typeData3 = new TypeData(type, GetArrayName(primitiveTypeData.XmlType), false);
						primitiveArrayTypes[xmlDataType] = typeData3;
						return typeData3;
					}
					throw new InvalidOperationException(string.Concat("Cannot convert values of type '", type.GetElementType(), "' to '", xmlDataType, "'"));
				}
				return primitiveTypeData;
			}
			TypeData typeData4 = nameCache[runtimeType] as TypeData;
			if (typeData4 != null)
			{
				return typeData4;
			}
			string text;
			if (type.IsArray)
			{
				string xmlType = GetTypeData(type.GetElementType()).XmlType;
				text = GetArrayName(xmlType);
			}
			else if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				text = XmlConvert.EncodeLocalName(type.Name.Substring(0, type.Name.IndexOf('`'))) + "Of";
				Type[] genericArguments = type.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					text += ((!type2.IsArray && !type2.IsGenericType) ? CodeIdentifier.MakePascal(XmlConvert.EncodeLocalName(type2.Name)) : GetTypeData(type2).XmlType);
				}
			}
			else
			{
				text = XmlConvert.EncodeLocalName(type.Name);
			}
			typeData4 = new TypeData(type, text, false);
			if (flag)
			{
				typeData4.IsNullable = true;
			}
			nameCache[runtimeType] = typeData4;
			return typeData4;
		}

		public static bool IsPrimitive(Type type)
		{
			return GetTypeData(type).SchemaType == SchemaTypes.Primitive;
		}

		public static TypeData GetPrimitiveTypeData(string typeName)
		{
			return GetPrimitiveTypeData(typeName, false);
		}

		public static TypeData GetPrimitiveTypeData(string typeName, bool nullable)
		{
			TypeData typeData = (TypeData)primitiveTypes[typeName];
			if (typeData != null && !typeData.Type.IsValueType)
			{
				return typeData;
			}
			Hashtable hashtable = ((!nullable || nullableTypes == null) ? primitiveTypes : nullableTypes);
			typeData = (TypeData)hashtable[typeName];
			if (typeData == null)
			{
				throw new NotSupportedException("Data type '" + typeName + "' not supported");
			}
			return typeData;
		}

		public static TypeData FindPrimitiveTypeData(string typeName)
		{
			return (TypeData)primitiveTypes[typeName];
		}

		public static TypeData GetDefaultPrimitiveTypeData(TypeData primType)
		{
			if (primType.SchemaType == SchemaTypes.Primitive)
			{
				TypeData typeData = GetTypeData(primType.Type, null);
				if (typeData != primType)
				{
					return typeData;
				}
			}
			return primType;
		}

		public static bool IsDefaultPrimitiveTpeData(TypeData primType)
		{
			return GetDefaultPrimitiveTypeData(primType) == primType;
		}

		public static TypeData CreateCustomType(string typeName, string fullTypeName, string xmlType, SchemaTypes schemaType, TypeData listItemTypeData)
		{
			return new TypeData(typeName, fullTypeName, xmlType, schemaType, listItemTypeData);
		}

		public static string GetArrayName(string elemName)
		{
			return "ArrayOf" + char.ToUpper(elemName[0], CultureInfo.InvariantCulture) + elemName.Substring(1);
		}

		public static string GetArrayName(string elemName, int dimensions)
		{
			string text = GetArrayName(elemName);
			while (dimensions > 1)
			{
				text = "ArrayOf" + text;
				dimensions--;
			}
			return text;
		}

		public static void ParseArrayType(string arrayType, out string type, out string ns, out string dimensions)
		{
			int num = arrayType.LastIndexOf(":");
			if (num == -1)
			{
				ns = string.Empty;
			}
			else
			{
				ns = arrayType.Substring(0, num);
			}
			int num2 = arrayType.IndexOf("[", num + 1);
			if (num2 == -1)
			{
				throw new InvalidOperationException("Cannot parse WSDL array type: " + arrayType);
			}
			type = arrayType.Substring(num + 1, num2 - num - 1);
			dimensions = arrayType.Substring(num2);
		}
	}
}
