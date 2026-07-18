using System.Collections;
using System.Globalization;
using System.Reflection;

namespace System.Xml.Serialization
{
	public class SoapReflectionImporter
	{
		private SoapAttributeOverrides attributeOverrides;

		private string initialDefaultNamespace;

		private ArrayList includedTypes;

		private ArrayList relatedMaps = new ArrayList();

		private ReflectionHelper helper = new ReflectionHelper();

		public SoapReflectionImporter()
			: this(null, null)
		{
		}

		public SoapReflectionImporter(SoapAttributeOverrides attributeOverrides)
			: this(attributeOverrides, null)
		{
		}

		public SoapReflectionImporter(string defaultNamespace)
			: this(null, defaultNamespace)
		{
		}

		public SoapReflectionImporter(SoapAttributeOverrides attributeOverrides, string defaultNamespace)
		{
			if (defaultNamespace == null)
			{
				initialDefaultNamespace = string.Empty;
			}
			else
			{
				initialDefaultNamespace = defaultNamespace;
			}
			if (attributeOverrides == null)
			{
				this.attributeOverrides = new SoapAttributeOverrides();
			}
			else
			{
				this.attributeOverrides = attributeOverrides;
			}
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members)
		{
			return ImportMembersMapping(elementName, ns, members, true, true, false);
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors)
		{
			return ImportMembersMapping(elementName, ns, members, hasWrapperElement, writeAccessors, false);
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors, bool validate)
		{
			return ImportMembersMapping(elementName, ns, members, hasWrapperElement, writeAccessors, validate, XmlMappingAccess.Read | XmlMappingAccess.Write);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors, bool validate, XmlMappingAccess access)
		{
			elementName = XmlConvert.EncodeLocalName(elementName);
			XmlMemberMapping[] array = new XmlMemberMapping[members.Length];
			for (int i = 0; i < members.Length; i++)
			{
				XmlTypeMapMember mapMem = CreateMapMember(members[i], ns);
				array[i] = new XmlMemberMapping(XmlConvert.EncodeLocalName(members[i].MemberName), ns, mapMem, true);
			}
			XmlMembersMapping xmlMembersMapping = new XmlMembersMapping(elementName, ns, hasWrapperElement, writeAccessors, array);
			xmlMembersMapping.RelatedMaps = relatedMaps;
			xmlMembersMapping.Format = SerializationFormat.Encoded;
			Type[] array2 = ((includedTypes == null) ? null : ((Type[])includedTypes.ToArray(typeof(Type))));
			xmlMembersMapping.Source = new MembersSerializationSource(elementName, hasWrapperElement, members, writeAccessors, false, null, array2);
			return xmlMembersMapping;
		}

		public XmlTypeMapping ImportTypeMapping(Type type)
		{
			return ImportTypeMapping(type, null);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type == typeof(void))
			{
				throw new InvalidOperationException("Type " + type.Name + " may not be serialized.");
			}
			return ImportTypeMapping(TypeTranslator.GetTypeData(type), defaultNamespace);
		}

		internal XmlTypeMapping ImportTypeMapping(TypeData typeData, string defaultNamespace)
		{
			if (typeData == null)
			{
				throw new ArgumentNullException("typeData");
			}
			if (typeData.Type == null)
			{
				throw new ArgumentException("Specified TypeData instance does not have Type set.");
			}
			string text = initialDefaultNamespace;
			if (defaultNamespace == null)
			{
				defaultNamespace = initialDefaultNamespace;
			}
			if (defaultNamespace == null)
			{
				defaultNamespace = string.Empty;
			}
			initialDefaultNamespace = defaultNamespace;
			XmlTypeMapping xmlTypeMapping;
			switch (typeData.SchemaType)
			{
			case SchemaTypes.Class:
				xmlTypeMapping = ImportClassMapping(typeData, defaultNamespace);
				break;
			case SchemaTypes.Array:
				xmlTypeMapping = ImportListMapping(typeData, defaultNamespace);
				break;
			case SchemaTypes.XmlNode:
				throw CreateTypeException(typeData.Type);
			case SchemaTypes.Primitive:
				xmlTypeMapping = ImportPrimitiveMapping(typeData, defaultNamespace);
				break;
			case SchemaTypes.Enum:
				xmlTypeMapping = ImportEnumMapping(typeData, defaultNamespace);
				break;
			default:
				throw new NotSupportedException("Type " + typeData.Type.FullName + " not supported for XML serialization");
			}
			xmlTypeMapping.RelatedMaps = relatedMaps;
			xmlTypeMapping.Format = SerializationFormat.Encoded;
			Type[] array = ((includedTypes == null) ? null : ((Type[])includedTypes.ToArray(typeof(Type))));
			xmlTypeMapping.Source = new SoapTypeSerializationSource(typeData.Type, attributeOverrides, defaultNamespace, array);
			initialDefaultNamespace = text;
			return xmlTypeMapping;
		}

		private XmlTypeMapping CreateTypeMapping(TypeData typeData, string defaultXmlType, string defaultNamespace)
		{
			string text = defaultNamespace;
			bool includeInSchema = true;
			SoapAttributes soapAttributes = null;
			if (defaultXmlType == null)
			{
				defaultXmlType = typeData.XmlType;
			}
			if (!typeData.IsListType)
			{
				if (attributeOverrides != null)
				{
					soapAttributes = attributeOverrides[typeData.Type];
				}
				if (soapAttributes != null && typeData.SchemaType == SchemaTypes.Primitive)
				{
					throw new InvalidOperationException("SoapType attribute may not be specified for the type " + typeData.FullTypeName);
				}
			}
			if (soapAttributes == null)
			{
				soapAttributes = new SoapAttributes(typeData.Type);
			}
			if (soapAttributes.SoapType != null)
			{
				if (soapAttributes.SoapType.Namespace != null && soapAttributes.SoapType.Namespace != string.Empty)
				{
					text = soapAttributes.SoapType.Namespace;
				}
				if (soapAttributes.SoapType.TypeName != null && soapAttributes.SoapType.TypeName != string.Empty)
				{
					defaultXmlType = XmlConvert.EncodeLocalName(soapAttributes.SoapType.TypeName);
				}
				includeInSchema = soapAttributes.SoapType.IncludeInSchema;
			}
			if (text == null)
			{
				text = string.Empty;
			}
			XmlTypeMapping xmlTypeMapping = new XmlTypeMapping(defaultXmlType, text, typeData, defaultXmlType, text);
			xmlTypeMapping.IncludeInSchema = includeInSchema;
			relatedMaps.Add(xmlTypeMapping);
			return xmlTypeMapping;
		}

		private XmlTypeMapping ImportClassMapping(Type type, string defaultNamespace)
		{
			TypeData typeData = TypeTranslator.GetTypeData(type);
			return ImportClassMapping(typeData, defaultNamespace);
		}

		private XmlTypeMapping ImportClassMapping(TypeData typeData, string defaultNamespace)
		{
			Type type = typeData.Type;
			if (type.IsValueType)
			{
				throw CreateStructException(type);
			}
			if (type == typeof(object))
			{
				defaultNamespace = "http://www.w3.org/2001/XMLSchema";
			}
			ReflectionHelper.CheckSerializableType(type, false);
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			registeredClrType = CreateTypeMapping(typeData, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.Namespace);
			registeredClrType.MultiReferenceType = true;
			ClassMap classMap = (ClassMap)(registeredClrType.ObjectMap = new ClassMap());
			ICollection reflectionMembers = GetReflectionMembers(type);
			foreach (XmlReflectionMember item in reflectionMembers)
			{
				if (!item.SoapAttributes.SoapIgnore)
				{
					classMap.AddMember(CreateMapMember(item, defaultNamespace));
				}
			}
			SoapIncludeAttribute[] array = (SoapIncludeAttribute[])type.GetCustomAttributes(typeof(SoapIncludeAttribute), false);
			for (int i = 0; i < array.Length; i++)
			{
				Type type2 = array[i].Type;
				ImportTypeMapping(type2);
			}
			if (type == typeof(object) && includedTypes != null)
			{
				foreach (Type includedType in includedTypes)
				{
					registeredClrType.DerivedTypes.Add(ImportTypeMapping(includedType));
				}
			}
			if (type.BaseType != null)
			{
				XmlTypeMapping xmlTypeMapping = ImportClassMapping(type.BaseType, defaultNamespace);
				if (type.BaseType != typeof(object))
				{
					registeredClrType.BaseMap = xmlTypeMapping;
				}
				RegisterDerivedMap(xmlTypeMapping, registeredClrType);
			}
			return registeredClrType;
		}

		private void RegisterDerivedMap(XmlTypeMapping map, XmlTypeMapping derivedMap)
		{
			map.DerivedTypes.Add(derivedMap);
			map.DerivedTypes.AddRange(derivedMap.DerivedTypes);
			if (map.BaseMap != null)
			{
				RegisterDerivedMap(map.BaseMap, derivedMap);
				return;
			}
			XmlTypeMapping xmlTypeMapping = ImportTypeMapping(typeof(object));
			if (xmlTypeMapping != map)
			{
				xmlTypeMapping.DerivedTypes.Add(derivedMap);
			}
		}

		private string GetTypeNamespace(TypeData typeData, string defaultNamespace)
		{
			string text = defaultNamespace;
			SoapAttributes soapAttributes = null;
			if (!typeData.IsListType && attributeOverrides != null)
			{
				soapAttributes = attributeOverrides[typeData.Type];
			}
			if (soapAttributes == null)
			{
				soapAttributes = new SoapAttributes(typeData.Type);
			}
			if (soapAttributes.SoapType != null && soapAttributes.SoapType.Namespace != null && soapAttributes.SoapType.Namespace != string.Empty)
			{
				text = soapAttributes.SoapType.Namespace;
			}
			if (text == null)
			{
				return string.Empty;
			}
			return text;
		}

		private XmlTypeMapping ImportListMapping(TypeData typeData, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, "http://schemas.xmlsoap.org/soap/encoding/");
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			ListMap listMap = new ListMap();
			TypeData listItemTypeData = typeData.ListItemTypeData;
			registeredClrType = CreateTypeMapping(typeData, "Array", "http://schemas.xmlsoap.org/soap/encoding/");
			helper.RegisterClrType(registeredClrType, type, "http://schemas.xmlsoap.org/soap/encoding/");
			registeredClrType.MultiReferenceType = true;
			registeredClrType.ObjectMap = listMap;
			XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(null, listItemTypeData);
			if (xmlTypeMapElementInfo.TypeData.IsComplexType)
			{
				xmlTypeMapElementInfo.MappedType = ImportTypeMapping(typeData.ListItemType, defaultNamespace);
				xmlTypeMapElementInfo.TypeData = xmlTypeMapElementInfo.MappedType.TypeData;
			}
			xmlTypeMapElementInfo.ElementName = "Item";
			xmlTypeMapElementInfo.Namespace = string.Empty;
			xmlTypeMapElementInfo.IsNullable = true;
			XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
			xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo);
			listMap.ItemInfo = xmlTypeMapElementInfoList;
			XmlTypeMapping xmlTypeMapping = ImportTypeMapping(typeof(object), defaultNamespace);
			xmlTypeMapping.DerivedTypes.Add(registeredClrType);
			SoapIncludeAttribute[] array = (SoapIncludeAttribute[])type.GetCustomAttributes(typeof(SoapIncludeAttribute), false);
			for (int i = 0; i < array.Length; i++)
			{
				Type type2 = array[i].Type;
				xmlTypeMapping.DerivedTypes.Add(ImportTypeMapping(type2, defaultNamespace));
			}
			return registeredClrType;
		}

		private XmlTypeMapping ImportPrimitiveMapping(TypeData typeData, string defaultNamespace)
		{
			if (typeData.SchemaType == SchemaTypes.Primitive)
			{
				defaultNamespace = ((!typeData.IsXsdType) ? "http://microsoft.com/wsdl/types/" : "http://www.w3.org/2001/XMLSchema");
			}
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			registeredClrType = CreateTypeMapping(typeData, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.Namespace);
			return registeredClrType;
		}

		private XmlTypeMapping ImportEnumMapping(TypeData typeData, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			ReflectionHelper.CheckSerializableType(type, false);
			registeredClrType = CreateTypeMapping(typeData, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.Namespace);
			registeredClrType.MultiReferenceType = true;
			string[] names = Enum.GetNames(type);
			EnumMap.EnumMapMember[] array = new EnumMap.EnumMapMember[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				FieldInfo field = type.GetField(names[i]);
				string name = names[i];
				object[] customAttributes = field.GetCustomAttributes(typeof(SoapEnumAttribute), false);
				if (customAttributes.Length > 0)
				{
					name = ((SoapEnumAttribute)customAttributes[0]).Name;
				}
				long value = ((IConvertible)field.GetValue(null)).ToInt64(CultureInfo.InvariantCulture);
				array[i] = new EnumMap.EnumMapMember(XmlConvert.EncodeLocalName(name), names[i], value);
			}
			bool isFlags = type.IsDefined(typeof(FlagsAttribute), false);
			registeredClrType.ObjectMap = new EnumMap(array, isFlags);
			ImportTypeMapping(typeof(object), defaultNamespace).DerivedTypes.Add(registeredClrType);
			return registeredClrType;
		}

		private ICollection GetReflectionMembers(Type type)
		{
			ArrayList arrayList = new ArrayList();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo.CanRead && (propertyInfo.CanWrite || (TypeTranslator.GetTypeData(propertyInfo.PropertyType).SchemaType == SchemaTypes.Array && !propertyInfo.PropertyType.IsArray)))
				{
					SoapAttributes soapAttributes = attributeOverrides[type, propertyInfo.Name];
					if (soapAttributes == null)
					{
						soapAttributes = new SoapAttributes(propertyInfo);
					}
					if (!soapAttributes.SoapIgnore)
					{
						XmlReflectionMember value = new XmlReflectionMember(propertyInfo.Name, propertyInfo.PropertyType, soapAttributes);
						arrayList.Add(value);
					}
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array2 = fields;
			foreach (FieldInfo fieldInfo in array2)
			{
				SoapAttributes soapAttributes2 = attributeOverrides[type, fieldInfo.Name];
				if (soapAttributes2 == null)
				{
					soapAttributes2 = new SoapAttributes(fieldInfo);
				}
				if (!soapAttributes2.SoapIgnore)
				{
					XmlReflectionMember value2 = new XmlReflectionMember(fieldInfo.Name, fieldInfo.FieldType, soapAttributes2);
					arrayList.Add(value2);
				}
			}
			return arrayList;
		}

		private XmlTypeMapMember CreateMapMember(XmlReflectionMember rmember, string defaultNamespace)
		{
			SoapAttributes soapAttributes = rmember.SoapAttributes;
			TypeData typeData = TypeTranslator.GetTypeData(rmember.MemberType);
			XmlTypeMapMember xmlTypeMapMember;
			if (soapAttributes.SoapAttribute != null)
			{
				if (typeData.SchemaType != SchemaTypes.Enum && typeData.SchemaType != SchemaTypes.Primitive)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot serialize member '{0}' of type {1}. SoapAttribute cannot be used to encode complex types.", rmember.MemberName, typeData.FullTypeName));
				}
				if (soapAttributes.SoapElement != null)
				{
					throw new Exception("SoapAttributeAttribute and SoapElementAttribute cannot be applied to the same member");
				}
				XmlTypeMapMemberAttribute xmlTypeMapMemberAttribute = new XmlTypeMapMemberAttribute();
				if (soapAttributes.SoapAttribute.AttributeName.Length == 0)
				{
					xmlTypeMapMemberAttribute.AttributeName = XmlConvert.EncodeLocalName(rmember.MemberName);
				}
				else
				{
					xmlTypeMapMemberAttribute.AttributeName = XmlConvert.EncodeLocalName(soapAttributes.SoapAttribute.AttributeName);
				}
				xmlTypeMapMemberAttribute.Namespace = ((soapAttributes.SoapAttribute.Namespace == null) ? string.Empty : soapAttributes.SoapAttribute.Namespace);
				if (typeData.IsComplexType)
				{
					xmlTypeMapMemberAttribute.MappedType = ImportTypeMapping(typeData.Type, defaultNamespace);
				}
				typeData = TypeTranslator.GetTypeData(rmember.MemberType, soapAttributes.SoapAttribute.DataType);
				xmlTypeMapMember = xmlTypeMapMemberAttribute;
				xmlTypeMapMember.DefaultValue = GetDefaultValue(typeData, soapAttributes.SoapDefaultValue);
			}
			else
			{
				xmlTypeMapMember = ((typeData.SchemaType != SchemaTypes.Array) ? new XmlTypeMapMemberElement() : new XmlTypeMapMemberList());
				if (soapAttributes.SoapElement != null && soapAttributes.SoapElement.DataType.Length != 0)
				{
					typeData = TypeTranslator.GetTypeData(rmember.MemberType, soapAttributes.SoapElement.DataType);
				}
				XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
				XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(xmlTypeMapMember, typeData);
				xmlTypeMapElementInfo.ElementName = XmlConvert.EncodeLocalName((soapAttributes.SoapElement == null || soapAttributes.SoapElement.ElementName.Length == 0) ? rmember.MemberName : soapAttributes.SoapElement.ElementName);
				xmlTypeMapElementInfo.Namespace = string.Empty;
				xmlTypeMapElementInfo.IsNullable = soapAttributes.SoapElement != null && soapAttributes.SoapElement.IsNullable;
				if (typeData.IsComplexType)
				{
					xmlTypeMapElementInfo.MappedType = ImportTypeMapping(typeData.Type, defaultNamespace);
				}
				xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo);
				((XmlTypeMapMemberElement)xmlTypeMapMember).ElementInfo = xmlTypeMapElementInfoList;
			}
			xmlTypeMapMember.TypeData = typeData;
			xmlTypeMapMember.Name = rmember.MemberName;
			xmlTypeMapMember.IsReturnValue = rmember.IsReturnValue;
			return xmlTypeMapMember;
		}

		public void IncludeType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (includedTypes == null)
			{
				includedTypes = new ArrayList();
			}
			if (!includedTypes.Contains(type))
			{
				includedTypes.Add(type);
			}
		}

		public void IncludeTypes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(typeof(SoapIncludeAttribute), true);
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				SoapIncludeAttribute soapIncludeAttribute = (SoapIncludeAttribute)array[i];
				IncludeType(soapIncludeAttribute.Type);
			}
		}

		private Exception CreateTypeException(Type type)
		{
			return new NotSupportedException("The type " + type.FullName + " may not be serialized with SOAP-encoded messages. Set the Use for your message to Literal");
		}

		private Exception CreateStructException(Type type)
		{
			return new NotSupportedException("Cannot serialize " + type.FullName + ". Nested structs are not supported with encoded SOAP");
		}

		private object GetDefaultValue(TypeData typeData, object defaultValue)
		{
			if (defaultValue == DBNull.Value || typeData.SchemaType != SchemaTypes.Enum)
			{
				return defaultValue;
			}
			if (typeData.Type != defaultValue.GetType())
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Enum {0} cannot be converted to {1}.", defaultValue.GetType().FullName, typeData.FullTypeName);
				throw new InvalidOperationException(message);
			}
			string text = Enum.Format(typeData.Type, defaultValue, "g");
			string text2 = Enum.Format(typeData.Type, defaultValue, "d");
			if (text == text2)
			{
				string message2 = string.Format(CultureInfo.InvariantCulture, "Value '{0}' cannot be converted to {1}.", defaultValue, defaultValue.GetType().FullName);
				throw new InvalidOperationException(message2);
			}
			return defaultValue;
		}
	}
}
