using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class TypeData
	{
		private Type type;

		private string elementName;

		private SchemaTypes sType;

		private Type listItemType;

		private string typeName;

		private string fullTypeName;

		private string csharpName;

		private string csharpFullName;

		private TypeData listItemTypeData;

		private TypeData listTypeData;

		private TypeData mappedType;

		private XmlSchemaPatternFacet facet;

		private bool hasPublicConstructor = true;

		private bool nullableOverride;

		private static Hashtable keywordsTable;

		private static string[] keywords = new string[80]
		{
			"abstract", "event", "new", "struct", "as", "explicit", "null", "switch", "base", "extern",
			"this", "false", "operator", "throw", "break", "finally", "out", "true", "fixed", "override",
			"try", "case", "params", "typeof", "catch", "for", "private", "foreach", "protected", "checked",
			"goto", "public", "unchecked", "class", "if", "readonly", "unsafe", "const", "implicit", "ref",
			"continue", "in", "return", "using", "virtual", "default", "interface", "sealed", "volatile", "delegate",
			"internal", "do", "is", "sizeof", "while", "lock", "stackalloc", "else", "static", "enum",
			"namespace", "object", "bool", "byte", "float", "uint", "char", "ulong", "ushort", "decimal",
			"int", "sbyte", "short", "double", "long", "string", "void", "partial", "yield", "where"
		};

		public string TypeName
		{
			get
			{
				return typeName;
			}
		}

		public string XmlType
		{
			get
			{
				return elementName;
			}
		}

		public Type Type
		{
			get
			{
				return type;
			}
		}

		public string FullTypeName
		{
			get
			{
				return fullTypeName;
			}
		}

		public string CSharpName
		{
			get
			{
				if (csharpName == null)
				{
					csharpName = ((Type != null) ? ToCSharpName(Type, false) : TypeName);
				}
				return csharpName;
			}
		}

		public string CSharpFullName
		{
			get
			{
				if (csharpFullName == null)
				{
					csharpFullName = ((Type != null) ? ToCSharpName(Type, true) : TypeName);
				}
				return csharpFullName;
			}
		}

		public SchemaTypes SchemaType
		{
			get
			{
				return sType;
			}
		}

		public bool IsListType
		{
			get
			{
				return SchemaType == SchemaTypes.Array;
			}
		}

		public bool IsComplexType
		{
			get
			{
				return SchemaType == SchemaTypes.Class || SchemaType == SchemaTypes.Array || SchemaType == SchemaTypes.Enum || SchemaType == SchemaTypes.XmlNode || SchemaType == SchemaTypes.XmlSerializable || !IsXsdType;
			}
		}

		public bool IsValueType
		{
			get
			{
				if (type != null)
				{
					return type.IsValueType;
				}
				return sType == SchemaTypes.Primitive || sType == SchemaTypes.Enum;
			}
		}

		public bool NullableOverride
		{
			get
			{
				return nullableOverride;
			}
		}

		public bool IsNullable
		{
			get
			{
				if (nullableOverride)
				{
					return true;
				}
				return !IsValueType || (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
			}
			set
			{
				nullableOverride = value;
			}
		}

		public TypeData ListItemTypeData
		{
			get
			{
				if (listItemTypeData == null && type != null)
				{
					listItemTypeData = TypeTranslator.GetTypeData(ListItemType);
				}
				return listItemTypeData;
			}
		}

		public Type ListItemType
		{
			get
			{
				if (this.type == null)
				{
					throw new InvalidOperationException("Property ListItemType is not supported for custom types");
				}
				if (listItemType != null)
				{
					return listItemType;
				}
				Type type = null;
				if (SchemaType != SchemaTypes.Array)
				{
					throw new InvalidOperationException(Type.FullName + " is not a collection");
				}
				if (this.type.IsArray)
				{
					listItemType = this.type.GetElementType();
				}
				else if (typeof(ICollection).IsAssignableFrom(this.type) || (type = GetGenericListItemType(this.type)) != null)
				{
					if (typeof(IDictionary).IsAssignableFrom(this.type))
					{
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The type {0} is not supported because it implements IDictionary.", this.type.FullName));
					}
					if (type != null)
					{
						listItemType = type;
					}
					else
					{
						PropertyInfo indexerProperty = GetIndexerProperty(this.type);
						if (indexerProperty == null)
						{
							throw new InvalidOperationException("You must implement a default accessor on " + this.type.FullName + " because it inherits from ICollection");
						}
						listItemType = indexerProperty.PropertyType;
					}
					MethodInfo method = this.type.GetMethod("Add", new Type[1] { listItemType });
					if (method == null)
					{
						throw CreateMissingAddMethodException(this.type, "ICollection", listItemType);
					}
				}
				else
				{
					MethodInfo method2 = this.type.GetMethod("GetEnumerator", Type.EmptyTypes);
					if (method2 == null)
					{
						method2 = this.type.GetMethod("System.Collections.IEnumerable.GetEnumerator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
					}
					PropertyInfo property = method2.ReturnType.GetProperty("Current");
					if (property == null)
					{
						listItemType = typeof(object);
					}
					else
					{
						listItemType = property.PropertyType;
					}
					MethodInfo method3 = this.type.GetMethod("Add", new Type[1] { listItemType });
					if (method3 == null)
					{
						throw CreateMissingAddMethodException(this.type, "IEnumerable", listItemType);
					}
				}
				return listItemType;
			}
		}

		public TypeData ListTypeData
		{
			get
			{
				if (listTypeData != null)
				{
					return listTypeData;
				}
				listTypeData = new TypeData(TypeName + "[]", FullTypeName + "[]", TypeTranslator.GetArrayName(XmlType), SchemaTypes.Array, this);
				return listTypeData;
			}
		}

		public bool IsXsdType
		{
			get
			{
				return mappedType == null;
			}
		}

		public TypeData MappedType
		{
			get
			{
				return (mappedType == null) ? this : mappedType;
			}
		}

		public XmlSchemaPatternFacet XmlSchemaPatternFacet
		{
			get
			{
				return facet;
			}
		}

		public bool HasPublicConstructor
		{
			get
			{
				return hasPublicConstructor;
			}
		}

		public TypeData(Type type, string elementName, bool isPrimitive)
			: this(type, elementName, isPrimitive, null, null)
		{
		}

		public TypeData(Type type, string elementName, bool isPrimitive, TypeData mappedType, XmlSchemaPatternFacet facet)
		{
			if (type.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException("Generic type definition cannot be used in serialization. Only specific generic types can be used.");
			}
			this.mappedType = mappedType;
			this.facet = facet;
			this.type = type;
			typeName = type.Name;
			fullTypeName = type.FullName.Replace('+', '.');
			if (isPrimitive)
			{
				sType = SchemaTypes.Primitive;
			}
			else if (type.IsEnum)
			{
				sType = SchemaTypes.Enum;
			}
			else if (typeof(IXmlSerializable).IsAssignableFrom(type))
			{
				sType = SchemaTypes.XmlSerializable;
			}
			else if (typeof(XmlNode).IsAssignableFrom(type))
			{
				sType = SchemaTypes.XmlNode;
			}
			else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
			{
				sType = SchemaTypes.Array;
			}
			else
			{
				sType = SchemaTypes.Class;
			}
			if (IsListType)
			{
				this.elementName = TypeTranslator.GetArrayName(ListItemTypeData.XmlType);
			}
			else
			{
				this.elementName = elementName;
			}
			if (sType == SchemaTypes.Array || sType == SchemaTypes.Class)
			{
				hasPublicConstructor = !type.IsInterface && (type.IsArray || type.GetConstructor(Type.EmptyTypes) != null || type.IsAbstract || type.IsValueType);
			}
		}

		internal TypeData(string typeName, string fullTypeName, string xmlType, SchemaTypes schemaType, TypeData listItemTypeData)
		{
			elementName = xmlType;
			this.typeName = typeName;
			this.fullTypeName = fullTypeName.Replace('+', '.');
			this.listItemTypeData = listItemTypeData;
			sType = schemaType;
			hasPublicConstructor = true;
		}

		public static string ToCSharpName(Type type, bool full)
		{
			if (type.IsArray)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(ToCSharpName(type.GetElementType(), full));
				stringBuilder.Append('[');
				int arrayRank = type.GetArrayRank();
				for (int i = 1; i < arrayRank; i++)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(ToCSharpName(type.GetGenericTypeDefinition(), full));
				stringBuilder2.Append('<');
				Type[] genericArguments = type.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					stringBuilder2.Append(ToCSharpName(type2, full)).Append(',');
				}
				stringBuilder2.Length--;
				stringBuilder2.Append('>');
				return stringBuilder2.ToString();
			}
			string text = ((!full) ? type.Name : type.FullName);
			text = text.Replace('+', '.');
			int num = text.IndexOf('`');
			text = ((num <= 0) ? text : text.Substring(0, num));
			if (IsKeyword(text))
			{
				return "@" + text;
			}
			return text;
		}

		private static bool IsKeyword(string name)
		{
			if (keywordsTable == null)
			{
				Hashtable hashtable = new Hashtable();
				string[] array = keywords;
				foreach (string text in array)
				{
					hashtable[text] = text;
				}
				keywordsTable = hashtable;
			}
			return keywordsTable.Contains(name);
		}

		public static PropertyInfo GetIndexerProperty(Type collectionType)
		{
			PropertyInfo[] properties = collectionType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
				if (indexParameters != null && indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
				{
					return propertyInfo;
				}
			}
			return null;
		}

		private static InvalidOperationException CreateMissingAddMethodException(Type type, string inheritFrom, Type argumentType)
		{
			return new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "To be XML serializable, types which inherit from {0} must have an implementation of Add({1}) at all levels of their inheritance hierarchy. {2} does not implement Add({1}).", inheritFrom, argumentType.FullName, type.FullName));
		}

		private Type GetGenericListItemType(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
			{
				return type.GetGenericArguments()[0];
			}
			Type type2 = null;
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type3 in interfaces)
			{
				if ((type2 = GetGenericListItemType(type3)) != null)
				{
					return type2;
				}
			}
			return null;
		}
	}
}
