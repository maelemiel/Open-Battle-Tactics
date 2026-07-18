using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlReflectionImporter
	{
		private string initialDefaultNamespace;

		private XmlAttributeOverrides attributeOverrides;

		private ArrayList includedTypes;

		private ReflectionHelper helper = new ReflectionHelper();

		private int arrayChoiceCount = 1;

		private ArrayList relatedMaps = new ArrayList();

		private bool allowPrivateTypes;

		private static readonly string errSimple = "Cannot serialize object of type '{0}'. Base type '{1}' has simpleContent and can be only extended by adding XmlAttribute elements. Please consider changing XmlText member of the base class to string array";

		private static readonly string errSimple2 = "Cannot serialize object of type '{0}'. Consider changing type of XmlText member '{1}' from '{2}' to string or string array";

		internal bool AllowPrivateTypes
		{
			get
			{
				return allowPrivateTypes;
			}
			set
			{
				allowPrivateTypes = value;
			}
		}

		public XmlReflectionImporter()
			: this(null, null)
		{
		}

		public XmlReflectionImporter(string defaultNamespace)
			: this(null, defaultNamespace)
		{
		}

		public XmlReflectionImporter(XmlAttributeOverrides attributeOverrides)
			: this(attributeOverrides, null)
		{
		}

		public XmlReflectionImporter(XmlAttributeOverrides attributeOverrides, string defaultNamespace)
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
				this.attributeOverrides = new XmlAttributeOverrides();
			}
			else
			{
				this.attributeOverrides = attributeOverrides;
			}
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement)
		{
			return ImportMembersMapping(elementName, ns, members, hasWrapperElement, true);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors)
		{
			return ImportMembersMapping(elementName, ns, members, hasWrapperElement, writeAccessors, true);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors, bool validate)
		{
			return ImportMembersMapping(elementName, ns, members, hasWrapperElement, writeAccessors, validate, XmlMappingAccess.Read | XmlMappingAccess.Write);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool writeAccessors, bool validate, XmlMappingAccess access)
		{
			XmlMemberMapping[] array = new XmlMemberMapping[members.Length];
			for (int i = 0; i < members.Length; i++)
			{
				XmlTypeMapMember mapMem = CreateMapMember(null, members[i], ns);
				array[i] = new XmlMemberMapping(members[i].MemberName, ns, mapMem, false);
			}
			elementName = XmlConvert.EncodeLocalName(elementName);
			XmlMembersMapping xmlMembersMapping = new XmlMembersMapping(elementName, ns, hasWrapperElement, false, array);
			xmlMembersMapping.RelatedMaps = relatedMaps;
			xmlMembersMapping.Format = SerializationFormat.Literal;
			Type[] array2 = ((includedTypes == null) ? null : ((Type[])includedTypes.ToArray(typeof(Type))));
			xmlMembersMapping.Source = new MembersSerializationSource(elementName, hasWrapperElement, members, false, true, ns, array2);
			if (allowPrivateTypes)
			{
				xmlMembersMapping.Source.CanBeGenerated = false;
			}
			return xmlMembersMapping;
		}

		public XmlTypeMapping ImportTypeMapping(Type type)
		{
			return ImportTypeMapping(type, null, null);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, string defaultNamespace)
		{
			return ImportTypeMapping(type, null, defaultNamespace);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, XmlRootAttribute group)
		{
			return ImportTypeMapping(type, group, null);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, XmlRootAttribute root, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type == typeof(void))
			{
				throw new NotSupportedException("The type " + type.FullName + " may not be serialized.");
			}
			return ImportTypeMapping(TypeTranslator.GetTypeData(type), root, defaultNamespace);
		}

		internal XmlTypeMapping ImportTypeMapping(TypeData typeData, string defaultNamespace)
		{
			return ImportTypeMapping(typeData, null, defaultNamespace);
		}

		private XmlTypeMapping ImportTypeMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			if (typeData == null)
			{
				throw new ArgumentNullException("typeData");
			}
			if (typeData.Type == null)
			{
				throw new ArgumentException("Specified TypeData instance does not have Type set.");
			}
			if (defaultNamespace == null)
			{
				defaultNamespace = initialDefaultNamespace;
			}
			if (defaultNamespace == null)
			{
				defaultNamespace = string.Empty;
			}
			try
			{
				XmlTypeMapping xmlTypeMapping;
				switch (typeData.SchemaType)
				{
				case SchemaTypes.Class:
					xmlTypeMapping = ImportClassMapping(typeData, root, defaultNamespace);
					break;
				case SchemaTypes.Array:
					xmlTypeMapping = ImportListMapping(typeData, root, defaultNamespace, null, 0);
					break;
				case SchemaTypes.XmlNode:
					xmlTypeMapping = ImportXmlNodeMapping(typeData, root, defaultNamespace);
					break;
				case SchemaTypes.Primitive:
					xmlTypeMapping = ImportPrimitiveMapping(typeData, root, defaultNamespace);
					break;
				case SchemaTypes.Enum:
					xmlTypeMapping = ImportEnumMapping(typeData, root, defaultNamespace);
					break;
				case SchemaTypes.XmlSerializable:
					xmlTypeMapping = ImportXmlSerializableMapping(typeData, root, defaultNamespace);
					break;
				default:
					throw new NotSupportedException("Type " + typeData.Type.FullName + " not supported for XML stialization");
				}
				xmlTypeMapping.SetKey(typeData.Type.ToString());
				xmlTypeMapping.RelatedMaps = relatedMaps;
				xmlTypeMapping.Format = SerializationFormat.Literal;
				Type[] array = ((includedTypes == null) ? null : ((Type[])includedTypes.ToArray(typeof(Type))));
				xmlTypeMapping.Source = new XmlTypeSerializationSource(typeData.Type, root, attributeOverrides, defaultNamespace, array);
				if (allowPrivateTypes)
				{
					xmlTypeMapping.Source.CanBeGenerated = false;
				}
				return xmlTypeMapping;
			}
			catch (InvalidOperationException innerException)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There was an error reflecting type '{0}'.", typeData.Type.FullName), innerException);
			}
		}

		private XmlTypeMapping CreateTypeMapping(TypeData typeData, XmlRootAttribute root, string defaultXmlType, string defaultNamespace)
		{
			string text = defaultNamespace;
			string text2 = null;
			bool includeInSchema = true;
			XmlAttributes xmlAttributes = null;
			bool isNullable = CanBeNull(typeData);
			if (defaultXmlType == null)
			{
				defaultXmlType = typeData.XmlType;
			}
			if (!typeData.IsListType)
			{
				if (attributeOverrides != null)
				{
					xmlAttributes = attributeOverrides[typeData.Type];
				}
				if (xmlAttributes != null && typeData.SchemaType == SchemaTypes.Primitive)
				{
					throw new InvalidOperationException("XmlRoot and XmlType attributes may not be specified for the type " + typeData.FullTypeName);
				}
			}
			if (xmlAttributes == null)
			{
				xmlAttributes = new XmlAttributes(typeData.Type);
			}
			if (xmlAttributes.XmlRoot != null && root == null)
			{
				root = xmlAttributes.XmlRoot;
			}
			if (xmlAttributes.XmlType != null)
			{
				if (xmlAttributes.XmlType.Namespace != null)
				{
					text2 = xmlAttributes.XmlType.Namespace;
				}
				if (xmlAttributes.XmlType.TypeName != null && xmlAttributes.XmlType.TypeName != string.Empty)
				{
					defaultXmlType = XmlConvert.EncodeLocalName(xmlAttributes.XmlType.TypeName);
				}
				includeInSchema = xmlAttributes.XmlType.IncludeInSchema;
			}
			string elementName = defaultXmlType;
			if (root != null)
			{
				if (root.ElementName.Length != 0)
				{
					elementName = XmlConvert.EncodeLocalName(root.ElementName);
				}
				if (root.Namespace != null)
				{
					text = root.Namespace;
				}
				isNullable = root.IsNullable;
			}
			if (text == null)
			{
				text = string.Empty;
			}
			if (text2 == null)
			{
				text2 = text;
			}
			XmlTypeMapping xmlTypeMapping;
			switch (typeData.SchemaType)
			{
			case SchemaTypes.XmlSerializable:
				xmlTypeMapping = new XmlSerializableMapping(elementName, text, typeData, defaultXmlType, text2);
				break;
			case SchemaTypes.Primitive:
				xmlTypeMapping = (typeData.IsXsdType ? new XmlTypeMapping(elementName, text, typeData, defaultXmlType, text2) : new XmlTypeMapping(elementName, text, typeData, defaultXmlType, "http://microsoft.com/wsdl/types/"));
				break;
			default:
				xmlTypeMapping = new XmlTypeMapping(elementName, text, typeData, defaultXmlType, text2);
				break;
			}
			xmlTypeMapping.IncludeInSchema = includeInSchema;
			xmlTypeMapping.IsNullable = isNullable;
			relatedMaps.Add(xmlTypeMapping);
			return xmlTypeMapping;
		}

		private XmlTypeMapping ImportClassMapping(Type type, XmlRootAttribute root, string defaultNamespace)
		{
			TypeData typeData = TypeTranslator.GetTypeData(type);
			return ImportClassMapping(typeData, root, defaultNamespace);
		}

		private XmlTypeMapping ImportClassMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, root, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			if (!allowPrivateTypes)
			{
				ReflectionHelper.CheckSerializableType(type, false);
			}
			registeredClrType = CreateTypeMapping(typeData, root, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.XmlTypeNamespace);
			helper.RegisterSchemaType(registeredClrType, registeredClrType.XmlType, registeredClrType.XmlTypeNamespace);
			ClassMap classMap = (ClassMap)(registeredClrType.ObjectMap = new ClassMap());
			ICollection reflectionMembers = GetReflectionMembers(type);
			foreach (XmlReflectionMember item in reflectionMembers)
			{
				string xmlTypeNamespace = registeredClrType.XmlTypeNamespace;
				if (!item.XmlAttributes.XmlIgnore)
				{
					if (item.DeclaringType != null && item.DeclaringType != type)
					{
						XmlTypeMapping xmlTypeMapping = ImportClassMapping(item.DeclaringType, root, defaultNamespace);
						xmlTypeNamespace = xmlTypeMapping.XmlTypeNamespace;
					}
					try
					{
						XmlTypeMapMember xmlTypeMapMember = CreateMapMember(type, item, xmlTypeNamespace);
						xmlTypeMapMember.CheckOptionalValueType(type);
						classMap.AddMember(xmlTypeMapMember);
					}
					catch (Exception innerException)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There was an error reflecting field '{0}'.", item.MemberName), innerException);
					}
				}
			}
			if (type == typeof(object) && includedTypes != null)
			{
				foreach (Type includedType in includedTypes)
				{
					registeredClrType.DerivedTypes.Add(ImportTypeMapping(includedType, defaultNamespace));
				}
			}
			if (type.BaseType != null)
			{
				XmlTypeMapping xmlTypeMapping2 = ImportClassMapping(type.BaseType, root, defaultNamespace);
				ClassMap classMap2 = xmlTypeMapping2.ObjectMap as ClassMap;
				if (type.BaseType != typeof(object))
				{
					registeredClrType.BaseMap = xmlTypeMapping2;
					if (!classMap2.HasSimpleContent)
					{
						classMap.SetCanBeSimpleType(false);
					}
				}
				RegisterDerivedMap(xmlTypeMapping2, registeredClrType);
				if (classMap2.HasSimpleContent && classMap.ElementMembers != null && classMap.ElementMembers.Count != 1)
				{
					throw new InvalidOperationException(string.Format(errSimple, registeredClrType.TypeData.TypeName, registeredClrType.BaseMap.TypeData.TypeName));
				}
			}
			ImportIncludedTypes(type, defaultNamespace);
			if (classMap.XmlTextCollector != null && !classMap.HasSimpleContent)
			{
				XmlTypeMapMember xmlTextCollector = classMap.XmlTextCollector;
				if (xmlTextCollector.TypeData.Type != typeof(string) && xmlTextCollector.TypeData.Type != typeof(string[]) && xmlTextCollector.TypeData.Type != typeof(object[]) && xmlTextCollector.TypeData.Type != typeof(XmlNode[]))
				{
					throw new InvalidOperationException(string.Format(errSimple2, registeredClrType.TypeData.TypeName, xmlTextCollector.Name, xmlTextCollector.TypeData.TypeName));
				}
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

		private string GetTypeNamespace(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			string text = null;
			XmlAttributes xmlAttributes = null;
			if (!typeData.IsListType && attributeOverrides != null)
			{
				xmlAttributes = attributeOverrides[typeData.Type];
			}
			if (xmlAttributes == null)
			{
				xmlAttributes = new XmlAttributes(typeData.Type);
			}
			if (xmlAttributes.XmlType != null && xmlAttributes.XmlType.Namespace != null && xmlAttributes.XmlType.Namespace.Length != 0 && typeData.SchemaType != SchemaTypes.Enum)
			{
				text = xmlAttributes.XmlType.Namespace;
			}
			if (text != null && text.Length != 0)
			{
				return text;
			}
			if (xmlAttributes.XmlRoot != null && root == null)
			{
				root = xmlAttributes.XmlRoot;
			}
			if (root != null && root.Namespace != null && root.Namespace.Length != 0)
			{
				return root.Namespace;
			}
			if (defaultNamespace == null)
			{
				return string.Empty;
			}
			return defaultNamespace;
		}

		private XmlTypeMapping ImportListMapping(Type type, XmlRootAttribute root, string defaultNamespace, XmlAttributes atts, int nestingLevel)
		{
			TypeData typeData = TypeTranslator.GetTypeData(type);
			return ImportListMapping(typeData, root, defaultNamespace, atts, nestingLevel);
		}

		private XmlTypeMapping ImportListMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace, XmlAttributes atts, int nestingLevel)
		{
			Type type = typeData.Type;
			ListMap listMap = new ListMap();
			if (!allowPrivateTypes)
			{
				ReflectionHelper.CheckSerializableType(type, true);
			}
			if (atts == null)
			{
				atts = new XmlAttributes();
			}
			Type listItemType = typeData.ListItemType;
			bool flag = type.IsArray && TypeTranslator.GetTypeData(listItemType).SchemaType == SchemaTypes.Array && listItemType.IsArray;
			XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
			foreach (XmlArrayItemAttribute xmlArrayItem in atts.XmlArrayItems)
			{
				if (xmlArrayItem.Namespace != null && xmlArrayItem.Form == XmlSchemaForm.Unqualified)
				{
					throw new InvalidOperationException("XmlArrayItemAttribute.Form must not be Unqualified when it has an explicit Namespace value.");
				}
				if (xmlArrayItem.NestingLevel == nestingLevel)
				{
					Type type2 = ((xmlArrayItem.Type == null) ? listItemType : xmlArrayItem.Type);
					XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(null, TypeTranslator.GetTypeData(type2, xmlArrayItem.DataType));
					xmlTypeMapElementInfo.Namespace = ((xmlArrayItem.Namespace == null) ? defaultNamespace : xmlArrayItem.Namespace);
					if (xmlTypeMapElementInfo.Namespace == null)
					{
						xmlTypeMapElementInfo.Namespace = string.Empty;
					}
					xmlTypeMapElementInfo.Form = xmlArrayItem.Form;
					if (xmlArrayItem.Form == XmlSchemaForm.Unqualified)
					{
						xmlTypeMapElementInfo.Namespace = string.Empty;
					}
					xmlTypeMapElementInfo.IsNullable = xmlArrayItem.IsNullable && CanBeNull(xmlTypeMapElementInfo.TypeData);
					xmlTypeMapElementInfo.NestingLevel = xmlArrayItem.NestingLevel;
					if (flag)
					{
						xmlTypeMapElementInfo.MappedType = ImportListMapping(type2, null, xmlTypeMapElementInfo.Namespace, atts, nestingLevel + 1);
					}
					else if (xmlTypeMapElementInfo.TypeData.IsComplexType)
					{
						xmlTypeMapElementInfo.MappedType = ImportTypeMapping(type2, null, xmlTypeMapElementInfo.Namespace);
					}
					if (xmlArrayItem.ElementName.Length != 0)
					{
						xmlTypeMapElementInfo.ElementName = XmlConvert.EncodeLocalName(xmlArrayItem.ElementName);
					}
					else if (xmlTypeMapElementInfo.MappedType != null)
					{
						xmlTypeMapElementInfo.ElementName = xmlTypeMapElementInfo.MappedType.ElementName;
					}
					else
					{
						xmlTypeMapElementInfo.ElementName = TypeTranslator.GetTypeData(type2).XmlType;
					}
					xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo);
				}
			}
			if (xmlTypeMapElementInfoList.Count == 0)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo2 = new XmlTypeMapElementInfo(null, TypeTranslator.GetTypeData(listItemType));
				if (flag)
				{
					xmlTypeMapElementInfo2.MappedType = ImportListMapping(listItemType, null, defaultNamespace, atts, nestingLevel + 1);
				}
				else if (xmlTypeMapElementInfo2.TypeData.IsComplexType)
				{
					xmlTypeMapElementInfo2.MappedType = ImportTypeMapping(listItemType, null, defaultNamespace);
				}
				if (xmlTypeMapElementInfo2.MappedType != null)
				{
					xmlTypeMapElementInfo2.ElementName = xmlTypeMapElementInfo2.MappedType.XmlType;
				}
				else
				{
					xmlTypeMapElementInfo2.ElementName = TypeTranslator.GetTypeData(listItemType).XmlType;
				}
				xmlTypeMapElementInfo2.Namespace = ((defaultNamespace == null) ? string.Empty : defaultNamespace);
				xmlTypeMapElementInfo2.IsNullable = CanBeNull(xmlTypeMapElementInfo2.TypeData);
				xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo2);
			}
			listMap.ItemInfo = xmlTypeMapElementInfoList;
			string text;
			if (xmlTypeMapElementInfoList.Count > 1)
			{
				text = "ArrayOfChoice" + arrayChoiceCount++;
			}
			else
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo3 = (XmlTypeMapElementInfo)xmlTypeMapElementInfoList[0];
				text = ((xmlTypeMapElementInfo3.MappedType == null) ? TypeTranslator.GetArrayName(xmlTypeMapElementInfo3.ElementName) : TypeTranslator.GetArrayName(xmlTypeMapElementInfo3.MappedType.XmlType));
			}
			int num = 1;
			string text2 = text;
			do
			{
				XmlTypeMapping registeredSchemaType = helper.GetRegisteredSchemaType(text2, defaultNamespace);
				if (registeredSchemaType == null)
				{
					num = -1;
					continue;
				}
				if (listMap.Equals(registeredSchemaType.ObjectMap) && typeData.Type == registeredSchemaType.TypeData.Type)
				{
					return registeredSchemaType;
				}
				text2 = text + num++;
			}
			while (num != -1);
			XmlTypeMapping xmlTypeMapping = CreateTypeMapping(typeData, root, text2, defaultNamespace);
			xmlTypeMapping.ObjectMap = listMap;
			XmlIncludeAttribute[] array = (XmlIncludeAttribute[])type.GetCustomAttributes(typeof(XmlIncludeAttribute), false);
			XmlTypeMapping xmlTypeMapping2 = ImportTypeMapping(typeof(object));
			for (int i = 0; i < array.Length; i++)
			{
				Type type3 = array[i].Type;
				xmlTypeMapping2.DerivedTypes.Add(ImportTypeMapping(type3, null, defaultNamespace));
			}
			helper.RegisterSchemaType(xmlTypeMapping, text2, defaultNamespace);
			ImportTypeMapping(typeof(object)).DerivedTypes.Add(xmlTypeMapping);
			return xmlTypeMapping;
		}

		private XmlTypeMapping ImportXmlNodeMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, root, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			registeredClrType = CreateTypeMapping(typeData, root, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.XmlTypeNamespace);
			if (type.BaseType != null)
			{
				XmlTypeMapping xmlTypeMapping = ImportTypeMapping(type.BaseType, root, defaultNamespace);
				if (type.BaseType != typeof(object))
				{
					registeredClrType.BaseMap = xmlTypeMapping;
				}
				RegisterDerivedMap(xmlTypeMapping, registeredClrType);
			}
			return registeredClrType;
		}

		private XmlTypeMapping ImportPrimitiveMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, root, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			registeredClrType = CreateTypeMapping(typeData, root, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.XmlTypeNamespace);
			return registeredClrType;
		}

		private XmlTypeMapping ImportEnumMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, root, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			if (!allowPrivateTypes)
			{
				ReflectionHelper.CheckSerializableType(type, false);
			}
			registeredClrType = CreateTypeMapping(typeData, root, null, defaultNamespace);
			registeredClrType.IsNullable = false;
			helper.RegisterClrType(registeredClrType, type, registeredClrType.XmlTypeNamespace);
			string[] names = Enum.GetNames(type);
			ArrayList arrayList = new ArrayList();
			string[] array = names;
			foreach (string text in array)
			{
				FieldInfo field = type.GetField(text);
				string text2 = null;
				if (!field.IsDefined(typeof(XmlIgnoreAttribute), false))
				{
					object[] customAttributes = field.GetCustomAttributes(typeof(XmlEnumAttribute), false);
					if (customAttributes.Length > 0)
					{
						text2 = ((XmlEnumAttribute)customAttributes[0]).Name;
					}
					if (text2 == null)
					{
						text2 = text;
					}
					long value = ((IConvertible)field.GetValue(null)).ToInt64(CultureInfo.InvariantCulture);
					arrayList.Add(new EnumMap.EnumMapMember(text2, text, value));
				}
			}
			bool isFlags = type.IsDefined(typeof(FlagsAttribute), false);
			registeredClrType.ObjectMap = new EnumMap((EnumMap.EnumMapMember[])arrayList.ToArray(typeof(EnumMap.EnumMapMember)), isFlags);
			ImportTypeMapping(typeof(object)).DerivedTypes.Add(registeredClrType);
			return registeredClrType;
		}

		private XmlTypeMapping ImportXmlSerializableMapping(TypeData typeData, XmlRootAttribute root, string defaultNamespace)
		{
			Type type = typeData.Type;
			XmlTypeMapping registeredClrType = helper.GetRegisteredClrType(type, GetTypeNamespace(typeData, root, defaultNamespace));
			if (registeredClrType != null)
			{
				return registeredClrType;
			}
			if (!allowPrivateTypes)
			{
				ReflectionHelper.CheckSerializableType(type, false);
			}
			registeredClrType = CreateTypeMapping(typeData, root, null, defaultNamespace);
			helper.RegisterClrType(registeredClrType, type, registeredClrType.XmlTypeNamespace);
			return registeredClrType;
		}

		private void ImportIncludedTypes(Type type, string defaultNamespace)
		{
			XmlIncludeAttribute[] array = (XmlIncludeAttribute[])type.GetCustomAttributes(typeof(XmlIncludeAttribute), false);
			for (int i = 0; i < array.Length; i++)
			{
				Type type2 = array[i].Type;
				ImportTypeMapping(type2, null, defaultNamespace);
			}
		}

		private ICollection GetReflectionMembers(Type type)
		{
			Type type2 = type;
			ArrayList arrayList = new ArrayList();
			arrayList.Add(type2);
			while (type2 != typeof(object))
			{
				type2 = type2.BaseType;
				arrayList.Insert(0, type2);
			}
			ArrayList arrayList2 = new ArrayList();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			type2 = null;
			int num = 0;
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (type2 != fieldInfo.DeclaringType)
				{
					type2 = fieldInfo.DeclaringType;
					num = 0;
				}
				arrayList2.Insert(num++, fieldInfo);
			}
			ArrayList arrayList3 = new ArrayList();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			type2 = null;
			num = 0;
			PropertyInfo[] array2 = properties;
			foreach (PropertyInfo propertyInfo in array2)
			{
				if (type2 != propertyInfo.DeclaringType)
				{
					type2 = propertyInfo.DeclaringType;
					num = 0;
				}
				if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length <= 0)
				{
					arrayList3.Insert(num++, propertyInfo);
				}
			}
			ArrayList arrayList4 = new ArrayList();
			int num2 = 0;
			int num3 = 0;
			foreach (Type item in arrayList)
			{
				while (num2 < arrayList2.Count)
				{
					FieldInfo fieldInfo2 = (FieldInfo)arrayList2[num2];
					if (fieldInfo2.DeclaringType == item)
					{
						num2++;
						XmlAttributes xmlAttributes = attributeOverrides[type, fieldInfo2.Name];
						if (xmlAttributes == null)
						{
							xmlAttributes = new XmlAttributes(fieldInfo2);
						}
						if (!xmlAttributes.XmlIgnore)
						{
							XmlReflectionMember xmlReflectionMember = new XmlReflectionMember(fieldInfo2.Name, fieldInfo2.FieldType, xmlAttributes);
							xmlReflectionMember.DeclaringType = fieldInfo2.DeclaringType;
							arrayList4.Add(xmlReflectionMember);
						}
						continue;
					}
					break;
				}
				while (num3 < arrayList3.Count)
				{
					PropertyInfo propertyInfo2 = (PropertyInfo)arrayList3[num3];
					if (propertyInfo2.DeclaringType == item)
					{
						num3++;
						XmlAttributes xmlAttributes2 = attributeOverrides[type, propertyInfo2.Name];
						if (xmlAttributes2 == null)
						{
							xmlAttributes2 = new XmlAttributes(propertyInfo2);
						}
						if (!xmlAttributes2.XmlIgnore && (propertyInfo2.CanWrite || (TypeTranslator.GetTypeData(propertyInfo2.PropertyType).SchemaType == SchemaTypes.Array && !propertyInfo2.PropertyType.IsArray)))
						{
							XmlReflectionMember xmlReflectionMember2 = new XmlReflectionMember(propertyInfo2.Name, propertyInfo2.PropertyType, xmlAttributes2);
							xmlReflectionMember2.DeclaringType = propertyInfo2.DeclaringType;
							arrayList4.Add(xmlReflectionMember2);
						}
						continue;
					}
					break;
				}
			}
			return arrayList4;
		}

		private XmlTypeMapMember CreateMapMember(Type declaringType, XmlReflectionMember rmember, string defaultNamespace)
		{
			XmlAttributes xmlAttributes = rmember.XmlAttributes;
			TypeData typeData = TypeTranslator.GetTypeData(rmember.MemberType);
			if (xmlAttributes.XmlArray != null)
			{
				if (xmlAttributes.XmlArray.Namespace != null && xmlAttributes.XmlArray.Form == XmlSchemaForm.Unqualified)
				{
					throw new InvalidOperationException("XmlArrayAttribute.Form must not be Unqualified when it has an explicit Namespace value.");
				}
				if (typeData.SchemaType != SchemaTypes.Array && (typeData.SchemaType != SchemaTypes.Primitive || typeData.Type != typeof(byte[])))
				{
					throw new InvalidOperationException("XmlArrayAttribute can be applied to members of array or collection type.");
				}
			}
			XmlTypeMapMember xmlTypeMapMember;
			if (xmlAttributes.XmlAnyAttribute != null)
			{
				if (!(rmember.MemberType.FullName == "System.Xml.XmlAttribute[]") && !(rmember.MemberType.FullName == "System.Xml.XmlNode[]"))
				{
					throw new InvalidOperationException("XmlAnyAttributeAttribute can only be applied to members of type XmlAttribute[] or XmlNode[]");
				}
				xmlTypeMapMember = new XmlTypeMapMemberAnyAttribute();
			}
			else if (xmlAttributes.XmlAnyElements != null && xmlAttributes.XmlAnyElements.Count > 0)
			{
				if (!(rmember.MemberType.FullName == "System.Xml.XmlElement[]") && !(rmember.MemberType.FullName == "System.Xml.XmlNode[]") && !(rmember.MemberType.FullName == "System.Xml.XmlElement"))
				{
					throw new InvalidOperationException("XmlAnyElementAttribute can only be applied to members of type XmlElement, XmlElement[] or XmlNode[]");
				}
				XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = new XmlTypeMapMemberAnyElement();
				xmlTypeMapMemberAnyElement.ElementInfo = ImportAnyElementInfo(defaultNamespace, rmember, xmlTypeMapMemberAnyElement, xmlAttributes);
				xmlTypeMapMember = xmlTypeMapMemberAnyElement;
			}
			else if (xmlAttributes.Xmlns)
			{
				XmlTypeMapMemberNamespaces xmlTypeMapMemberNamespaces = new XmlTypeMapMemberNamespaces();
				xmlTypeMapMember = xmlTypeMapMemberNamespaces;
			}
			else if (xmlAttributes.XmlAttribute != null)
			{
				if (xmlAttributes.XmlElements != null && xmlAttributes.XmlElements.Count > 0)
				{
					throw new Exception("XmlAttributeAttribute and XmlElementAttribute cannot be applied to the same member");
				}
				XmlTypeMapMemberAttribute xmlTypeMapMemberAttribute = new XmlTypeMapMemberAttribute();
				if (xmlAttributes.XmlAttribute.AttributeName.Length == 0)
				{
					xmlTypeMapMemberAttribute.AttributeName = rmember.MemberName;
				}
				else
				{
					xmlTypeMapMemberAttribute.AttributeName = xmlAttributes.XmlAttribute.AttributeName;
				}
				xmlTypeMapMemberAttribute.AttributeName = XmlConvert.EncodeLocalName(xmlTypeMapMemberAttribute.AttributeName);
				if (typeData.IsComplexType)
				{
					xmlTypeMapMemberAttribute.MappedType = ImportTypeMapping(typeData.Type, null, defaultNamespace);
				}
				if (xmlAttributes.XmlAttribute.Namespace != null && xmlAttributes.XmlAttribute.Namespace != defaultNamespace)
				{
					if (xmlAttributes.XmlAttribute.Form == XmlSchemaForm.Unqualified)
					{
						throw new InvalidOperationException("The Form property may not be 'Unqualified' when an explicit Namespace property is present");
					}
					xmlTypeMapMemberAttribute.Form = XmlSchemaForm.Qualified;
					xmlTypeMapMemberAttribute.Namespace = xmlAttributes.XmlAttribute.Namespace;
				}
				else
				{
					xmlTypeMapMemberAttribute.Form = xmlAttributes.XmlAttribute.Form;
					if (xmlAttributes.XmlAttribute.Form == XmlSchemaForm.Qualified)
					{
						xmlTypeMapMemberAttribute.Namespace = defaultNamespace;
					}
					else
					{
						xmlTypeMapMemberAttribute.Namespace = string.Empty;
					}
				}
				typeData = TypeTranslator.GetTypeData(rmember.MemberType, xmlAttributes.XmlAttribute.DataType);
				xmlTypeMapMember = xmlTypeMapMemberAttribute;
			}
			else if (typeData.SchemaType == SchemaTypes.Array)
			{
				if (xmlAttributes.XmlElements.Count > 1 || (xmlAttributes.XmlElements.Count == 1 && xmlAttributes.XmlElements[0].Type != typeData.Type) || xmlAttributes.XmlText != null)
				{
					if (xmlAttributes.XmlArray != null)
					{
						throw new InvalidOperationException("XmlArrayAttribute cannot be used with members which also attributed with XmlElementAttribute or XmlTextAttribute.");
					}
					XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = new XmlTypeMapMemberFlatList();
					xmlTypeMapMemberFlatList.ListMap = new ListMap();
					xmlTypeMapMemberFlatList.ListMap.ItemInfo = ImportElementInfo(declaringType, XmlConvert.EncodeLocalName(rmember.MemberName), defaultNamespace, typeData.ListItemType, xmlTypeMapMemberFlatList, xmlAttributes);
					xmlTypeMapMemberFlatList.ElementInfo = xmlTypeMapMemberFlatList.ListMap.ItemInfo;
					xmlTypeMapMemberFlatList.ListMap.ChoiceMember = xmlTypeMapMemberFlatList.ChoiceMember;
					xmlTypeMapMember = xmlTypeMapMemberFlatList;
				}
				else
				{
					XmlTypeMapMemberList xmlTypeMapMemberList = new XmlTypeMapMemberList();
					xmlTypeMapMemberList.ElementInfo = new XmlTypeMapElementInfoList();
					XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(xmlTypeMapMemberList, typeData);
					xmlTypeMapElementInfo.ElementName = XmlConvert.EncodeLocalName((xmlAttributes.XmlArray == null || xmlAttributes.XmlArray.ElementName.Length == 0) ? rmember.MemberName : xmlAttributes.XmlArray.ElementName);
					xmlTypeMapElementInfo.Namespace = ((xmlAttributes.XmlArray == null || xmlAttributes.XmlArray.Namespace == null) ? defaultNamespace : xmlAttributes.XmlArray.Namespace);
					xmlTypeMapElementInfo.MappedType = ImportListMapping(rmember.MemberType, null, xmlTypeMapElementInfo.Namespace, xmlAttributes, 0);
					xmlTypeMapElementInfo.IsNullable = xmlAttributes.XmlArray != null && xmlAttributes.XmlArray.IsNullable;
					xmlTypeMapElementInfo.Form = ((xmlAttributes.XmlArray == null) ? XmlSchemaForm.Qualified : xmlAttributes.XmlArray.Form);
					if (xmlAttributes.XmlArray != null && xmlAttributes.XmlArray.Form == XmlSchemaForm.Unqualified)
					{
						xmlTypeMapElementInfo.Namespace = string.Empty;
					}
					xmlTypeMapMemberList.ElementInfo.Add(xmlTypeMapElementInfo);
					xmlTypeMapMember = xmlTypeMapMemberList;
				}
			}
			else
			{
				XmlTypeMapMemberElement xmlTypeMapMemberElement = new XmlTypeMapMemberElement();
				xmlTypeMapMemberElement.ElementInfo = ImportElementInfo(declaringType, XmlConvert.EncodeLocalName(rmember.MemberName), defaultNamespace, rmember.MemberType, xmlTypeMapMemberElement, xmlAttributes);
				xmlTypeMapMember = xmlTypeMapMemberElement;
			}
			xmlTypeMapMember.DefaultValue = GetDefaultValue(typeData, xmlAttributes.XmlDefaultValue);
			xmlTypeMapMember.TypeData = typeData;
			xmlTypeMapMember.Name = rmember.MemberName;
			xmlTypeMapMember.IsReturnValue = rmember.IsReturnValue;
			return xmlTypeMapMember;
		}

		private XmlTypeMapElementInfoList ImportElementInfo(Type cls, string defaultName, string defaultNamespace, Type defaultType, XmlTypeMapMemberElement member, XmlAttributes atts)
		{
			EnumMap enumMap = null;
			Type type = null;
			XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
			ImportTextElementInfo(xmlTypeMapElementInfoList, defaultType, member, atts, defaultNamespace);
			if (atts.XmlChoiceIdentifier != null)
			{
				if (cls == null)
				{
					throw new InvalidOperationException("XmlChoiceIdentifierAttribute not supported in this context.");
				}
				member.ChoiceMember = atts.XmlChoiceIdentifier.MemberName;
				MemberInfo[] member2 = cls.GetMember(member.ChoiceMember, BindingFlags.Instance | BindingFlags.Public);
				if (member2.Length == 0)
				{
					throw new InvalidOperationException("Choice member '" + member.ChoiceMember + "' not found in class '" + cls);
				}
				if (member2[0] is PropertyInfo)
				{
					PropertyInfo propertyInfo = (PropertyInfo)member2[0];
					if (!propertyInfo.CanWrite || !propertyInfo.CanRead)
					{
						throw new InvalidOperationException("Choice property '" + member.ChoiceMember + "' must be read/write.");
					}
					type = propertyInfo.PropertyType;
				}
				else
				{
					type = ((FieldInfo)member2[0]).FieldType;
				}
				member.ChoiceTypeData = TypeTranslator.GetTypeData(type);
				if (type.IsArray)
				{
					type = type.GetElementType();
				}
				enumMap = ImportTypeMapping(type).ObjectMap as EnumMap;
				if (enumMap == null)
				{
					throw new InvalidOperationException("The member '" + member2[0].Name + "' is not a valid target for XmlChoiceIdentifierAttribute.");
				}
			}
			if (atts.XmlElements.Count == 0 && xmlTypeMapElementInfoList.Count == 0)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, TypeTranslator.GetTypeData(defaultType));
				xmlTypeMapElementInfo.ElementName = defaultName;
				xmlTypeMapElementInfo.Namespace = defaultNamespace;
				if (xmlTypeMapElementInfo.TypeData.IsComplexType)
				{
					xmlTypeMapElementInfo.MappedType = ImportTypeMapping(defaultType, null, defaultNamespace);
				}
				xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo);
			}
			bool flag = atts.XmlElements.Count > 1;
			foreach (XmlElementAttribute xmlElement in atts.XmlElements)
			{
				Type type2 = ((xmlElement.Type == null) ? defaultType : xmlElement.Type);
				XmlTypeMapElementInfo xmlTypeMapElementInfo2 = new XmlTypeMapElementInfo(member, TypeTranslator.GetTypeData(type2, xmlElement.DataType));
				xmlTypeMapElementInfo2.Form = xmlElement.Form;
				if (xmlTypeMapElementInfo2.Form != XmlSchemaForm.Unqualified)
				{
					xmlTypeMapElementInfo2.Namespace = ((xmlElement.Namespace == null) ? defaultNamespace : xmlElement.Namespace);
				}
				xmlTypeMapElementInfo2.IsNullable = xmlElement.IsNullable;
				if (xmlTypeMapElementInfo2.IsNullable && !xmlTypeMapElementInfo2.TypeData.IsNullable)
				{
					throw new InvalidOperationException("IsNullable may not be 'true' for value type " + xmlTypeMapElementInfo2.TypeData.FullTypeName + " in member '" + defaultName + "'");
				}
				if (xmlTypeMapElementInfo2.TypeData.IsComplexType)
				{
					if (xmlElement.DataType.Length != 0)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "'{0}' is an invalid value for '{1}.{2}' of type '{3}'. The property may only be specified for primitive types.", xmlElement.DataType, cls.FullName, defaultName, xmlTypeMapElementInfo2.TypeData.FullTypeName));
					}
					xmlTypeMapElementInfo2.MappedType = ImportTypeMapping(type2, null, xmlTypeMapElementInfo2.Namespace);
				}
				if (xmlElement.ElementName.Length != 0)
				{
					xmlTypeMapElementInfo2.ElementName = XmlConvert.EncodeLocalName(xmlElement.ElementName);
				}
				else if (flag)
				{
					if (xmlTypeMapElementInfo2.MappedType != null)
					{
						xmlTypeMapElementInfo2.ElementName = xmlTypeMapElementInfo2.MappedType.ElementName;
					}
					else
					{
						xmlTypeMapElementInfo2.ElementName = TypeTranslator.GetTypeData(type2).XmlType;
					}
				}
				else
				{
					xmlTypeMapElementInfo2.ElementName = defaultName;
				}
				if (enumMap != null)
				{
					string enumName = enumMap.GetEnumName(type.FullName, xmlTypeMapElementInfo2.ElementName);
					if (enumName == null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type {0} is missing enumeration value '{1}' for element '{1} from namespace '{2}'.", type, xmlTypeMapElementInfo2.ElementName, xmlTypeMapElementInfo2.Namespace));
					}
					xmlTypeMapElementInfo2.ChoiceValue = Enum.Parse(type, enumName);
				}
				xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo2);
			}
			return xmlTypeMapElementInfoList;
		}

		private XmlTypeMapElementInfoList ImportAnyElementInfo(string defaultNamespace, XmlReflectionMember rmember, XmlTypeMapMemberElement member, XmlAttributes atts)
		{
			XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
			ImportTextElementInfo(xmlTypeMapElementInfoList, rmember.MemberType, member, atts, defaultNamespace);
			foreach (XmlAnyElementAttribute xmlAnyElement in atts.XmlAnyElements)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, TypeTranslator.GetTypeData(typeof(XmlElement)));
				if (xmlAnyElement.Name.Length != 0)
				{
					xmlTypeMapElementInfo.ElementName = XmlConvert.EncodeLocalName(xmlAnyElement.Name);
					xmlTypeMapElementInfo.Namespace = ((xmlAnyElement.Namespace == null) ? string.Empty : xmlAnyElement.Namespace);
				}
				else
				{
					xmlTypeMapElementInfo.IsUnnamedAnyElement = true;
					xmlTypeMapElementInfo.Namespace = defaultNamespace;
					if (xmlAnyElement.Namespace != null)
					{
						throw new InvalidOperationException("The element " + rmember.MemberName + " has been attributed with an XmlAnyElementAttribute and a namespace '" + xmlAnyElement.Namespace + "', but no name. When a namespace is supplied, a name is also required. Supply a name or remove the namespace.");
					}
				}
				xmlTypeMapElementInfoList.Add(xmlTypeMapElementInfo);
			}
			return xmlTypeMapElementInfoList;
		}

		private void ImportTextElementInfo(XmlTypeMapElementInfoList list, Type defaultType, XmlTypeMapMemberElement member, XmlAttributes atts, string defaultNamespace)
		{
			if (atts.XmlText == null)
			{
				return;
			}
			member.IsXmlTextCollector = true;
			if (atts.XmlText.Type != null)
			{
				TypeData typeData = TypeTranslator.GetTypeData(defaultType);
				if ((typeData.SchemaType == SchemaTypes.Primitive || typeData.SchemaType == SchemaTypes.Enum) && atts.XmlText.Type != defaultType)
				{
					throw new InvalidOperationException("The type for XmlText may not be specified for primitive types.");
				}
				defaultType = atts.XmlText.Type;
			}
			if (defaultType == typeof(XmlNode))
			{
				defaultType = typeof(XmlText);
			}
			XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, TypeTranslator.GetTypeData(defaultType, atts.XmlText.DataType));
			if (xmlTypeMapElementInfo.TypeData.SchemaType != SchemaTypes.Primitive && xmlTypeMapElementInfo.TypeData.SchemaType != SchemaTypes.Enum && xmlTypeMapElementInfo.TypeData.SchemaType != SchemaTypes.XmlNode && (xmlTypeMapElementInfo.TypeData.SchemaType != SchemaTypes.Array || xmlTypeMapElementInfo.TypeData.ListItemTypeData.SchemaType != SchemaTypes.XmlNode))
			{
				throw new InvalidOperationException("XmlText cannot be used to encode complex types");
			}
			if (xmlTypeMapElementInfo.TypeData.IsComplexType)
			{
				xmlTypeMapElementInfo.MappedType = ImportTypeMapping(defaultType, null, defaultNamespace);
			}
			xmlTypeMapElementInfo.IsTextElement = true;
			xmlTypeMapElementInfo.WrappedElement = false;
			list.Add(xmlTypeMapElementInfo);
		}

		private bool CanBeNull(TypeData type)
		{
			return !type.Type.IsValueType || type.IsNullable;
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
			if (relatedMaps.Count <= 0)
			{
				return;
			}
			foreach (XmlTypeMapping item in (ArrayList)relatedMaps.Clone())
			{
				if (item.TypeData.Type == typeof(object))
				{
					item.DerivedTypes.Add(ImportTypeMapping(type));
				}
			}
		}

		public void IncludeTypes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(typeof(XmlIncludeAttribute), true);
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				XmlIncludeAttribute xmlIncludeAttribute = (XmlIncludeAttribute)array[i];
				IncludeType(xmlIncludeAttribute.Type);
			}
		}

		private object GetDefaultValue(TypeData typeData, object defaultValue)
		{
			if (defaultValue == DBNull.Value || typeData.SchemaType != SchemaTypes.Enum)
			{
				return defaultValue;
			}
			string text = Enum.Format(typeData.Type, defaultValue, "g");
			string text2 = Enum.Format(typeData.Type, defaultValue, "d");
			if (text == text2)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Value '{0}' cannot be converted to {1}.", defaultValue, defaultValue.GetType().FullName);
				throw new InvalidOperationException(message);
			}
			return defaultValue;
		}
	}
}
