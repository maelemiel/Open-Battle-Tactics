using System.Collections;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemaImporter
	{
		private class MapFixup
		{
			public XmlTypeMapping Map;

			public XmlSchemaComplexType SchemaType;

			public XmlQualifiedName TypeName;
		}

		private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private XmlSchemas schemas;

		private CodeIdentifiers typeIdentifiers;

		private CodeIdentifiers elemIdentifiers = new CodeIdentifiers();

		private Hashtable mappedTypes = new Hashtable();

		private Hashtable primitiveDerivedMappedTypes = new Hashtable();

		private Hashtable dataMappedTypes = new Hashtable();

		private Queue pendingMaps = new Queue();

		private Hashtable sharedAnonymousTypes = new Hashtable();

		private bool encodedFormat;

		private XmlReflectionImporter auxXmlRefImporter;

		private SoapReflectionImporter auxSoapRefImporter;

		private bool anyTypeImported;

		private CodeGenerationOptions options;

		private static readonly XmlQualifiedName anyType = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");

		private static readonly XmlQualifiedName arrayType = new XmlQualifiedName("Array", "http://schemas.xmlsoap.org/soap/encoding/");

		private static readonly XmlQualifiedName arrayTypeRefName = new XmlQualifiedName("arrayType", "http://schemas.xmlsoap.org/soap/encoding/");

		private XmlSchemaElement anyElement;

		internal bool UseEncodedFormat
		{
			get
			{
				return encodedFormat;
			}
			set
			{
				encodedFormat = value;
			}
		}

		public XmlSchemaImporter(XmlSchemas schemas)
		{
			this.schemas = schemas;
			typeIdentifiers = new CodeIdentifiers();
			InitializeExtensions();
		}

		public XmlSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers)
			: this(schemas)
		{
			this.typeIdentifiers = typeIdentifiers;
		}

		public XmlSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, ImportContext context)
		{
			this.schemas = schemas;
			this.options = options;
			if (context != null)
			{
				typeIdentifiers = context.TypeIdentifiers;
				InitSharedData(context);
			}
			else
			{
				typeIdentifiers = new CodeIdentifiers();
			}
			InitializeExtensions();
		}

		public XmlSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers, CodeGenerationOptions options)
		{
			this.typeIdentifiers = typeIdentifiers;
			this.schemas = schemas;
			this.options = options;
			InitializeExtensions();
		}

		private void InitSharedData(ImportContext context)
		{
			if (context.ShareTypes)
			{
				mappedTypes = context.MappedTypes;
				dataMappedTypes = context.DataMappedTypes;
				sharedAnonymousTypes = context.SharedAnonymousTypes;
			}
		}

		private void InitializeExtensions()
		{
		}

		public XmlMembersMapping ImportAnyType(XmlQualifiedName typeName, string elementName)
		{
			if (typeName == XmlQualifiedName.Empty)
			{
				XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = new XmlTypeMapMemberAnyElement();
				xmlTypeMapMemberAnyElement.Name = typeName.Name;
				xmlTypeMapMemberAnyElement.TypeData = TypeTranslator.GetTypeData(typeof(XmlNode));
				xmlTypeMapMemberAnyElement.ElementInfo.Add(CreateElementInfo(typeName.Namespace, xmlTypeMapMemberAnyElement, typeName.Name, xmlTypeMapMemberAnyElement.TypeData, true, XmlSchemaForm.None));
				return new XmlMembersMapping(new XmlMemberMapping[1]
				{
					new XmlMemberMapping(typeName.Name, typeName.Namespace, xmlTypeMapMemberAnyElement, encodedFormat)
				});
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)schemas.Find(typeName, typeof(XmlSchemaComplexType));
			if (xmlSchemaComplexType == null)
			{
				throw new InvalidOperationException(string.Concat("Referenced type '", typeName, "' not found"));
			}
			if (!CanBeAnyElement(xmlSchemaComplexType))
			{
				throw new InvalidOperationException(string.Concat("The type '", typeName, "' is not valid for a collection of any elements"));
			}
			ClassMap classMap = new ClassMap();
			CodeIdentifiers classIds = new CodeIdentifiers();
			bool isMixed = xmlSchemaComplexType.IsMixed;
			ImportSequenceContent(typeName, classMap, ((XmlSchemaSequence)xmlSchemaComplexType.Particle).Items, classIds, false, ref isMixed);
			XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement2 = (XmlTypeMapMemberAnyElement)classMap.AllMembers[0];
			xmlTypeMapMemberAnyElement2.Name = typeName.Name;
			return new XmlMembersMapping(new XmlMemberMapping[1]
			{
				new XmlMemberMapping(typeName.Name, typeName.Namespace, xmlTypeMapMemberAnyElement2, encodedFormat)
			});
		}

		public XmlTypeMapping ImportDerivedTypeMapping(XmlQualifiedName name, Type baseType)
		{
			return ImportDerivedTypeMapping(name, baseType, true);
		}

		public XmlTypeMapping ImportDerivedTypeMapping(XmlQualifiedName name, Type baseType, bool baseTypeCanBeIndirect)
		{
			XmlQualifiedName qname;
			XmlSchemaType stype;
			if (encodedFormat)
			{
				qname = name;
				stype = schemas.Find(name, typeof(XmlSchemaComplexType)) as XmlSchemaComplexType;
				if (stype == null)
				{
					throw new InvalidOperationException(string.Concat("Schema type '", name, "' not found or not valid"));
				}
			}
			else if (!LocateElement(name, out qname, out stype))
			{
				return null;
			}
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(qname, baseType);
			if (registeredTypeMapping != null)
			{
				SetMapBaseType(registeredTypeMapping, baseType);
				registeredTypeMapping.UpdateRoot(name);
				return registeredTypeMapping;
			}
			registeredTypeMapping = CreateTypeMapping(qname, SchemaTypes.Class, name);
			if (stype != null)
			{
				registeredTypeMapping.Documentation = GetDocumentation(stype);
				RegisterMapFixup(registeredTypeMapping, qname, (XmlSchemaComplexType)stype);
			}
			else
			{
				ClassMap classMap = new ClassMap();
				CodeIdentifiers classIds = new CodeIdentifiers();
				registeredTypeMapping.ObjectMap = classMap;
				AddTextMember(qname, classMap, classIds);
			}
			BuildPendingMaps();
			SetMapBaseType(registeredTypeMapping, baseType);
			return registeredTypeMapping;
		}

		private void SetMapBaseType(XmlTypeMapping map, Type baseType)
		{
			XmlTypeMapping xmlTypeMapping = null;
			while (map != null)
			{
				if (map.TypeData.Type == baseType)
				{
					return;
				}
				xmlTypeMapping = map;
				map = map.BaseMap;
			}
			XmlTypeMapping xmlTypeMapping2 = (xmlTypeMapping.BaseMap = ReflectType(baseType));
			xmlTypeMapping2.DerivedTypes.Add(xmlTypeMapping);
			xmlTypeMapping2.DerivedTypes.AddRange(xmlTypeMapping.DerivedTypes);
			ClassMap classMap = (ClassMap)xmlTypeMapping2.ObjectMap;
			ClassMap classMap2 = (ClassMap)xmlTypeMapping.ObjectMap;
			foreach (XmlTypeMapMember allMember in classMap.AllMembers)
			{
				classMap2.AddMember(allMember);
			}
			foreach (XmlTypeMapping derivedType in xmlTypeMapping.DerivedTypes)
			{
				classMap2 = (ClassMap)derivedType.ObjectMap;
				foreach (XmlTypeMapMember allMember2 in classMap.AllMembers)
				{
					classMap2.AddMember(allMember2);
				}
			}
		}

		public XmlMembersMapping ImportMembersMapping(XmlQualifiedName name)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)schemas.Find(name, typeof(XmlSchemaElement));
			if (xmlSchemaElement == null)
			{
				throw new InvalidOperationException(string.Concat("Schema element '", name, "' not found or not valid"));
			}
			XmlSchemaComplexType xmlSchemaComplexType;
			if (xmlSchemaElement.SchemaType != null)
			{
				xmlSchemaComplexType = xmlSchemaElement.SchemaType as XmlSchemaComplexType;
			}
			else
			{
				if (xmlSchemaElement.SchemaTypeName.IsEmpty)
				{
					return null;
				}
				object obj = schemas.Find(xmlSchemaElement.SchemaTypeName, typeof(XmlSchemaComplexType));
				if (obj == null)
				{
					if (IsPrimitiveTypeNamespace(xmlSchemaElement.SchemaTypeName.Namespace))
					{
						return null;
					}
					throw new InvalidOperationException(string.Concat("Schema type '", xmlSchemaElement.SchemaTypeName, "' not found"));
				}
				xmlSchemaComplexType = obj as XmlSchemaComplexType;
			}
			if (xmlSchemaComplexType == null)
			{
				throw new InvalidOperationException(string.Concat("Schema element '", name, "' not found or not valid"));
			}
			XmlMemberMapping[] mapping = ImportMembersMappingComposite(xmlSchemaComplexType, name);
			return new XmlMembersMapping(name.Name, name.Namespace, mapping);
		}

		public XmlMembersMapping ImportMembersMapping(XmlQualifiedName[] names)
		{
			XmlMemberMapping[] array = new XmlMemberMapping[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)schemas.Find(names[i], typeof(XmlSchemaElement));
				if (xmlSchemaElement == null)
				{
					throw new InvalidOperationException(string.Concat("Schema element '", names[i], "' not found"));
				}
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("Message", names[i].Namespace);
				XmlTypeMapping map;
				TypeData elementTypeData = GetElementTypeData(xmlQualifiedName, xmlSchemaElement, names[i], out map);
				array[i] = ImportMemberMapping(xmlSchemaElement.Name, xmlQualifiedName.Namespace, xmlSchemaElement.IsNillable, elementTypeData, map);
			}
			BuildPendingMaps();
			return new XmlMembersMapping(array);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members)
		{
			throw new NotImplementedException();
		}

		public XmlTypeMapping ImportSchemaType(XmlQualifiedName typeName)
		{
			return ImportSchemaType(typeName, typeof(object));
		}

		public XmlTypeMapping ImportSchemaType(XmlQualifiedName typeName, Type baseType)
		{
			return ImportSchemaType(typeName, typeof(object), false);
		}

		[System.MonoTODO("baseType and baseTypeCanBeIndirect are ignored")]
		public XmlTypeMapping ImportSchemaType(XmlQualifiedName typeName, Type baseType, bool baseTypeCanBeIndirect)
		{
			XmlSchemaType stype = ((XmlSchemaType)schemas.Find(typeName, typeof(XmlSchemaComplexType))) ?? ((XmlSchemaType)schemas.Find(typeName, typeof(XmlSchemaSimpleType)));
			return ImportTypeCommon(typeName, typeName, stype, true);
		}

		internal XmlMembersMapping ImportEncodedMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement)
		{
			XmlMemberMapping[] array = new XmlMemberMapping[members.Length];
			for (int i = 0; i < members.Length; i++)
			{
				TypeData typeData = GetTypeData(members[i].MemberType, null, false);
				XmlTypeMapping typeMapping = GetTypeMapping(typeData);
				array[i] = ImportMemberMapping(members[i].MemberName, members[i].MemberType.Namespace, true, typeData, typeMapping);
			}
			BuildPendingMaps();
			return new XmlMembersMapping(name, ns, hasWrapperElement, false, array);
		}

		internal XmlMembersMapping ImportEncodedMembersMapping(string name, string ns, SoapSchemaMember member)
		{
			XmlSchemaComplexType xmlSchemaComplexType = schemas.Find(member.MemberType, typeof(XmlSchemaComplexType)) as XmlSchemaComplexType;
			if (xmlSchemaComplexType == null)
			{
				throw new InvalidOperationException(string.Concat("Schema type '", member.MemberType, "' not found or not valid"));
			}
			XmlMemberMapping[] mapping = ImportMembersMappingComposite(xmlSchemaComplexType, member.MemberType);
			return new XmlMembersMapping(name, ns, mapping);
		}

		private XmlMemberMapping[] ImportMembersMappingComposite(XmlSchemaComplexType stype, XmlQualifiedName refer)
		{
			if (stype.Particle == null)
			{
				return new XmlMemberMapping[0];
			}
			ClassMap classMap = new ClassMap();
			XmlSchemaSequence xmlSchemaSequence = stype.Particle as XmlSchemaSequence;
			if (xmlSchemaSequence == null)
			{
				throw new InvalidOperationException(string.Concat("Schema element '", refer, "' cannot be imported as XmlMembersMapping"));
			}
			CodeIdentifiers classIds = new CodeIdentifiers();
			ImportParticleComplexContent(refer, classMap, xmlSchemaSequence, classIds, false);
			ImportAttributes(refer, classMap, stype.Attributes, stype.AnyAttribute, classIds);
			BuildPendingMaps();
			int num = 0;
			XmlMemberMapping[] array = new XmlMemberMapping[classMap.AllMembers.Count];
			foreach (XmlTypeMapMember allMember in classMap.AllMembers)
			{
				array[num++] = new XmlMemberMapping(allMember.Name, refer.Namespace, allMember, encodedFormat);
			}
			return array;
		}

		private XmlMemberMapping ImportMemberMapping(string name, string ns, bool isNullable, TypeData type, XmlTypeMapping emap)
		{
			XmlTypeMapMemberElement xmlTypeMapMemberElement = ((!type.IsListType) ? new XmlTypeMapMemberElement() : new XmlTypeMapMemberList());
			xmlTypeMapMemberElement.Name = name;
			xmlTypeMapMemberElement.TypeData = type;
			xmlTypeMapMemberElement.ElementInfo.Add(CreateElementInfo(ns, xmlTypeMapMemberElement, name, type, isNullable, XmlSchemaForm.None, emap));
			return new XmlMemberMapping(name, ns, xmlTypeMapMemberElement, encodedFormat);
		}

		[System.MonoTODO]
		public XmlMembersMapping ImportMembersMapping(XmlQualifiedName[] names, Type baseType, bool baseTypeCanBeIndirect)
		{
			throw new NotImplementedException();
		}

		public XmlTypeMapping ImportTypeMapping(XmlQualifiedName name)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)schemas.Find(name, typeof(XmlSchemaElement));
			XmlQualifiedName qname;
			XmlSchemaType stype;
			if (!LocateElement(xmlSchemaElement, out qname, out stype))
			{
				throw new InvalidOperationException(string.Format("'{0}' is missing.", name));
			}
			return ImportTypeCommon(name, qname, stype, xmlSchemaElement.IsNillable);
		}

		private XmlTypeMapping ImportTypeCommon(XmlQualifiedName name, XmlQualifiedName qname, XmlSchemaType stype, bool isNullable)
		{
			if (stype == null)
			{
				if (qname == anyType)
				{
					XmlTypeMapping typeMapping = GetTypeMapping(TypeTranslator.GetTypeData(typeof(object)));
					BuildPendingMaps();
					return typeMapping;
				}
				TypeData primitiveTypeData = TypeTranslator.GetPrimitiveTypeData(qname.Name);
				return ReflectType(primitiveTypeData, name.Namespace);
			}
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(qname);
			if (registeredTypeMapping != null)
			{
				return registeredTypeMapping;
			}
			if (stype is XmlSchemaSimpleType)
			{
				return ImportClassSimpleType(stype.QualifiedName, (XmlSchemaSimpleType)stype, name);
			}
			registeredTypeMapping = CreateTypeMapping(qname, SchemaTypes.Class, name);
			registeredTypeMapping.Documentation = GetDocumentation(stype);
			registeredTypeMapping.IsNullable = isNullable;
			RegisterMapFixup(registeredTypeMapping, qname, (XmlSchemaComplexType)stype);
			BuildPendingMaps();
			return registeredTypeMapping;
		}

		private bool LocateElement(XmlQualifiedName name, out XmlQualifiedName qname, out XmlSchemaType stype)
		{
			XmlSchemaElement elem = (XmlSchemaElement)schemas.Find(name, typeof(XmlSchemaElement));
			return LocateElement(elem, out qname, out stype);
		}

		private bool LocateElement(XmlSchemaElement elem, out XmlQualifiedName qname, out XmlSchemaType stype)
		{
			qname = null;
			stype = null;
			if (elem == null)
			{
				return false;
			}
			if (elem.SchemaType != null)
			{
				stype = elem.SchemaType;
				qname = elem.QualifiedName;
			}
			else
			{
				if (elem.ElementType == XmlSchemaComplexType.AnyType)
				{
					qname = anyType;
					return true;
				}
				if (elem.SchemaTypeName.IsEmpty)
				{
					return false;
				}
				object obj = schemas.Find(elem.SchemaTypeName, typeof(XmlSchemaComplexType));
				if (obj == null)
				{
					obj = schemas.Find(elem.SchemaTypeName, typeof(XmlSchemaSimpleType));
				}
				if (obj == null)
				{
					if (IsPrimitiveTypeNamespace(elem.SchemaTypeName.Namespace))
					{
						qname = elem.SchemaTypeName;
						return true;
					}
					throw new InvalidOperationException(string.Concat("Schema type '", elem.SchemaTypeName, "' not found"));
				}
				stype = (XmlSchemaType)obj;
				qname = stype.QualifiedName;
				XmlSchemaType xmlSchemaType = stype.BaseSchemaType as XmlSchemaType;
				if (xmlSchemaType != null && xmlSchemaType.QualifiedName == elem.SchemaTypeName)
				{
					throw new InvalidOperationException("Cannot import schema for type '" + elem.SchemaTypeName.Name + "' from namespace '" + elem.SchemaTypeName.Namespace + "'. Redefine not supported");
				}
			}
			return true;
		}

		private XmlTypeMapping ImportType(XmlQualifiedName name, XmlQualifiedName root, bool throwOnError)
		{
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(name);
			if (registeredTypeMapping != null)
			{
				registeredTypeMapping.UpdateRoot(root);
				return registeredTypeMapping;
			}
			XmlSchemaType xmlSchemaType = (XmlSchemaType)schemas.Find(name, typeof(XmlSchemaComplexType));
			if (xmlSchemaType == null)
			{
				xmlSchemaType = (XmlSchemaType)schemas.Find(name, typeof(XmlSchemaSimpleType));
			}
			if (xmlSchemaType == null)
			{
				if (throwOnError)
				{
					if (name.Namespace == "http://schemas.xmlsoap.org/soap/encoding/")
					{
						throw new InvalidOperationException(string.Concat("Referenced type '", name, "' valid only for encoded SOAP."));
					}
					throw new InvalidOperationException(string.Concat("Referenced type '", name, "' not found."));
				}
				return null;
			}
			return ImportType(name, xmlSchemaType, root);
		}

		private XmlTypeMapping ImportClass(XmlQualifiedName name)
		{
			XmlTypeMapping xmlTypeMapping = ImportType(name, null, true);
			if (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Class)
			{
				return xmlTypeMapping;
			}
			XmlSchemaComplexType stype = schemas.Find(name, typeof(XmlSchemaComplexType)) as XmlSchemaComplexType;
			return CreateClassMap(name, stype, new XmlQualifiedName(xmlTypeMapping.ElementName, xmlTypeMapping.Namespace));
		}

		private XmlTypeMapping ImportType(XmlQualifiedName name, XmlSchemaType stype, XmlQualifiedName root)
		{
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(name);
			if (registeredTypeMapping != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = stype as XmlSchemaComplexType;
				if (registeredTypeMapping.TypeData.SchemaType != SchemaTypes.Class || xmlSchemaComplexType == null || !CanBeArray(name, xmlSchemaComplexType))
				{
					registeredTypeMapping.UpdateRoot(root);
					return registeredTypeMapping;
				}
			}
			if (stype is XmlSchemaComplexType)
			{
				return ImportClassComplexType(name, (XmlSchemaComplexType)stype, root);
			}
			if (stype is XmlSchemaSimpleType)
			{
				return ImportClassSimpleType(name, (XmlSchemaSimpleType)stype, root);
			}
			throw new NotSupportedException("Schema type not supported: " + stype.GetType());
		}

		private XmlTypeMapping ImportClassComplexType(XmlQualifiedName typeQName, XmlSchemaComplexType stype, XmlQualifiedName root)
		{
			Type anyElementType = GetAnyElementType(stype);
			if (anyElementType != null)
			{
				return GetTypeMapping(TypeTranslator.GetTypeData(anyElementType));
			}
			if (CanBeArray(typeQName, stype))
			{
				TypeData arrayTypeData;
				ListMap listMap = BuildArrayMap(typeQName, stype, out arrayTypeData);
				if (listMap != null)
				{
					XmlTypeMapping xmlTypeMapping = CreateArrayTypeMapping(typeQName, arrayTypeData);
					xmlTypeMapping.ObjectMap = listMap;
					return xmlTypeMapping;
				}
			}
			else if (CanBeIXmlSerializable(stype))
			{
				return ImportXmlSerializableMapping(typeQName.Namespace);
			}
			return CreateClassMap(typeQName, stype, root);
		}

		private XmlTypeMapping CreateClassMap(XmlQualifiedName typeQName, XmlSchemaComplexType stype, XmlQualifiedName root)
		{
			XmlTypeMapping xmlTypeMapping = CreateTypeMapping(typeQName, SchemaTypes.Class, root);
			xmlTypeMapping.Documentation = GetDocumentation(stype);
			RegisterMapFixup(xmlTypeMapping, typeQName, stype);
			return xmlTypeMapping;
		}

		private void RegisterMapFixup(XmlTypeMapping map, XmlQualifiedName typeQName, XmlSchemaComplexType stype)
		{
			MapFixup mapFixup = new MapFixup();
			mapFixup.Map = map;
			mapFixup.SchemaType = stype;
			mapFixup.TypeName = typeQName;
			pendingMaps.Enqueue(mapFixup);
		}

		private void BuildPendingMaps()
		{
			while (pendingMaps.Count > 0)
			{
				MapFixup mapFixup = (MapFixup)pendingMaps.Dequeue();
				if (mapFixup.Map.ObjectMap == null)
				{
					BuildClassMap(mapFixup.Map, mapFixup.TypeName, mapFixup.SchemaType);
					if (mapFixup.Map.ObjectMap == null)
					{
						pendingMaps.Enqueue(mapFixup);
					}
				}
			}
		}

		private void BuildPendingMap(XmlTypeMapping map)
		{
			if (map.ObjectMap != null)
			{
				return;
			}
			foreach (MapFixup pendingMap in pendingMaps)
			{
				if (pendingMap.Map == map)
				{
					BuildClassMap(pendingMap.Map, pendingMap.TypeName, pendingMap.SchemaType);
					return;
				}
			}
			throw new InvalidOperationException("Can't complete map of type " + map.XmlType + " : " + map.Namespace);
		}

		private void BuildClassMap(XmlTypeMapping map, XmlQualifiedName typeQName, XmlSchemaComplexType stype)
		{
			CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
			codeIdentifiers.AddReserved(map.TypeData.TypeName);
			ClassMap cmap = (ClassMap)(map.ObjectMap = new ClassMap());
			bool isMixed = stype.IsMixed;
			if (stype.Particle != null)
			{
				ImportParticleComplexContent(typeQName, cmap, stype.Particle, codeIdentifiers, isMixed);
			}
			else if (stype.ContentModel is XmlSchemaSimpleContent)
			{
				ImportSimpleContent(typeQName, map, (XmlSchemaSimpleContent)stype.ContentModel, codeIdentifiers, isMixed);
			}
			else if (stype.ContentModel is XmlSchemaComplexContent)
			{
				ImportComplexContent(typeQName, map, (XmlSchemaComplexContent)stype.ContentModel, codeIdentifiers, isMixed);
			}
			ImportAttributes(typeQName, cmap, stype.Attributes, stype.AnyAttribute, codeIdentifiers);
			ImportExtensionTypes(typeQName);
			if (isMixed)
			{
				AddTextMember(typeQName, cmap, codeIdentifiers);
			}
			AddObjectDerivedMap(map);
		}

		private void ImportAttributes(XmlQualifiedName typeQName, ClassMap cmap, XmlSchemaObjectCollection atts, XmlSchemaAnyAttribute anyat, CodeIdentifiers classIds)
		{
			atts = CollectAttributeUsesNonOverlap(atts, cmap);
			if (anyat != null)
			{
				XmlTypeMapMemberAnyAttribute xmlTypeMapMemberAnyAttribute = new XmlTypeMapMemberAnyAttribute();
				xmlTypeMapMemberAnyAttribute.Name = classIds.AddUnique("AnyAttribute", xmlTypeMapMemberAnyAttribute);
				xmlTypeMapMemberAnyAttribute.TypeData = TypeTranslator.GetTypeData(typeof(XmlAttribute[]));
				cmap.AddMember(xmlTypeMapMemberAnyAttribute);
			}
			foreach (XmlSchemaObject att in atts)
			{
				if (att is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)att;
					string ns;
					XmlSchemaAttribute refAttribute = GetRefAttribute(typeQName, xmlSchemaAttribute, out ns);
					XmlTypeMapMemberAttribute xmlTypeMapMemberAttribute = new XmlTypeMapMemberAttribute();
					xmlTypeMapMemberAttribute.Name = classIds.AddUnique(CodeIdentifier.MakeValid(refAttribute.Name), xmlTypeMapMemberAttribute);
					xmlTypeMapMemberAttribute.Documentation = GetDocumentation(xmlSchemaAttribute);
					xmlTypeMapMemberAttribute.AttributeName = refAttribute.Name;
					xmlTypeMapMemberAttribute.Namespace = ns;
					xmlTypeMapMemberAttribute.Form = refAttribute.Form;
					xmlTypeMapMemberAttribute.TypeData = GetAttributeTypeData(typeQName, xmlSchemaAttribute);
					if (refAttribute.DefaultValue != null)
					{
						xmlTypeMapMemberAttribute.DefaultValue = ImportDefaultValue(xmlTypeMapMemberAttribute.TypeData, refAttribute.DefaultValue);
					}
					else if (xmlTypeMapMemberAttribute.TypeData.IsValueType)
					{
						xmlTypeMapMemberAttribute.IsOptionalValueType = refAttribute.ValidatedUse != XmlSchemaUse.Required;
					}
					if (xmlTypeMapMemberAttribute.TypeData.IsComplexType)
					{
						xmlTypeMapMemberAttribute.MappedType = GetTypeMapping(xmlTypeMapMemberAttribute.TypeData);
					}
					cmap.AddMember(xmlTypeMapMemberAttribute);
				}
				else if (att is XmlSchemaAttributeGroupRef)
				{
					XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = (XmlSchemaAttributeGroupRef)att;
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = FindRefAttributeGroup(xmlSchemaAttributeGroupRef.RefName);
					ImportAttributes(typeQName, cmap, xmlSchemaAttributeGroup.Attributes, xmlSchemaAttributeGroup.AnyAttribute, classIds);
				}
			}
		}

		private XmlSchemaObjectCollection CollectAttributeUsesNonOverlap(XmlSchemaObjectCollection src, ClassMap map)
		{
			XmlSchemaObjectCollection xmlSchemaObjectCollection = new XmlSchemaObjectCollection();
			foreach (XmlSchemaAttribute item in src)
			{
				if (map.GetAttribute(item.QualifiedName.Name, item.QualifiedName.Namespace) == null)
				{
					xmlSchemaObjectCollection.Add(item);
				}
			}
			return xmlSchemaObjectCollection;
		}

		private ListMap BuildArrayMap(XmlQualifiedName typeQName, XmlSchemaComplexType stype, out TypeData arrayTypeData)
		{
			if (encodedFormat)
			{
				XmlSchemaComplexContent xmlSchemaComplexContent = stype.ContentModel as XmlSchemaComplexContent;
				XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = xmlSchemaComplexContent.Content as XmlSchemaComplexContentRestriction;
				XmlSchemaAttribute xmlSchemaAttribute = FindArrayAttribute(xmlSchemaComplexContentRestriction.Attributes);
				if (xmlSchemaAttribute != null)
				{
					XmlAttribute[] unhandledAttributes = xmlSchemaAttribute.UnhandledAttributes;
					if (unhandledAttributes == null || unhandledAttributes.Length == 0)
					{
						throw new InvalidOperationException("arrayType attribute not specified in array declaration: " + typeQName);
					}
					XmlAttribute xmlAttribute = null;
					XmlAttribute[] array = unhandledAttributes;
					foreach (XmlAttribute xmlAttribute2 in array)
					{
						if (xmlAttribute2.LocalName == "arrayType" && xmlAttribute2.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/")
						{
							xmlAttribute = xmlAttribute2;
							break;
						}
					}
					if (xmlAttribute == null)
					{
						throw new InvalidOperationException("arrayType attribute not specified in array declaration: " + typeQName);
					}
					string type;
					string ns;
					string dimensions;
					TypeTranslator.ParseArrayType(xmlAttribute.Value, out type, out ns, out dimensions);
					return BuildEncodedArrayMap(type + dimensions, ns, out arrayTypeData);
				}
				XmlSchemaElement xmlSchemaElement = null;
				XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentRestriction.Particle as XmlSchemaSequence;
				if (xmlSchemaSequence != null && xmlSchemaSequence.Items.Count == 1)
				{
					xmlSchemaElement = xmlSchemaSequence.Items[0] as XmlSchemaElement;
				}
				else
				{
					XmlSchemaAll xmlSchemaAll = xmlSchemaComplexContentRestriction.Particle as XmlSchemaAll;
					if (xmlSchemaAll != null && xmlSchemaAll.Items.Count == 1)
					{
						xmlSchemaElement = xmlSchemaAll.Items[0] as XmlSchemaElement;
					}
				}
				if (xmlSchemaElement == null)
				{
					throw new InvalidOperationException("Unknown array format");
				}
				return BuildEncodedArrayMap(xmlSchemaElement.SchemaTypeName.Name + "[]", xmlSchemaElement.SchemaTypeName.Namespace, out arrayTypeData);
			}
			ClassMap classMap = new ClassMap();
			CodeIdentifiers classIds = new CodeIdentifiers();
			ImportParticleComplexContent(typeQName, classMap, stype.Particle, classIds, stype.IsMixed);
			XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = ((classMap.AllMembers.Count != 1) ? null : (classMap.AllMembers[0] as XmlTypeMapMemberFlatList));
			if (xmlTypeMapMemberFlatList != null && xmlTypeMapMemberFlatList.ChoiceMember == null)
			{
				arrayTypeData = xmlTypeMapMemberFlatList.TypeData;
				return xmlTypeMapMemberFlatList.ListMap;
			}
			arrayTypeData = null;
			return null;
		}

		private ListMap BuildEncodedArrayMap(string type, string ns, out TypeData arrayTypeData)
		{
			ListMap listMap = new ListMap();
			int num = type.LastIndexOf("[");
			if (num == -1)
			{
				throw new InvalidOperationException("Invalid arrayType value: " + type);
			}
			if (type.IndexOf(",", num) != -1)
			{
				throw new InvalidOperationException("Multidimensional arrays are not supported");
			}
			string text = type.Substring(0, num);
			TypeData arrayTypeData2;
			if (text.IndexOf("[") != -1)
			{
				ListMap objectMap = BuildEncodedArrayMap(text, ns, out arrayTypeData2);
				int dimensions = text.Split('[').Length - 1;
				string arrayName = TypeTranslator.GetArrayName(type, dimensions);
				XmlQualifiedName typeQName = new XmlQualifiedName(arrayName, ns);
				XmlTypeMapping xmlTypeMapping = CreateArrayTypeMapping(typeQName, arrayTypeData2);
				xmlTypeMapping.ObjectMap = objectMap;
			}
			else
			{
				arrayTypeData2 = GetTypeData(new XmlQualifiedName(text, ns), null, false);
			}
			arrayTypeData = arrayTypeData2.ListTypeData;
			listMap.ItemInfo = new XmlTypeMapElementInfoList();
			listMap.ItemInfo.Add(CreateElementInfo(string.Empty, null, "Item", arrayTypeData2, true, XmlSchemaForm.None));
			return listMap;
		}

		private XmlSchemaAttribute FindArrayAttribute(XmlSchemaObjectCollection atts)
		{
			foreach (XmlSchemaObject att in atts)
			{
				XmlSchemaAttribute xmlSchemaAttribute = att as XmlSchemaAttribute;
				if (xmlSchemaAttribute != null && xmlSchemaAttribute.RefName == arrayTypeRefName)
				{
					return xmlSchemaAttribute;
				}
				XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = att as XmlSchemaAttributeGroupRef;
				if (xmlSchemaAttributeGroupRef != null)
				{
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = FindRefAttributeGroup(xmlSchemaAttributeGroupRef.RefName);
					xmlSchemaAttribute = FindArrayAttribute(xmlSchemaAttributeGroup.Attributes);
					if (xmlSchemaAttribute != null)
					{
						return xmlSchemaAttribute;
					}
				}
			}
			return null;
		}

		private void ImportParticleComplexContent(XmlQualifiedName typeQName, ClassMap cmap, XmlSchemaParticle particle, CodeIdentifiers classIds, bool isMixed)
		{
			ImportParticleContent(typeQName, cmap, particle, classIds, false, ref isMixed);
			if (isMixed)
			{
				AddTextMember(typeQName, cmap, classIds);
			}
		}

		private void AddTextMember(XmlQualifiedName typeQName, ClassMap cmap, CodeIdentifiers classIds)
		{
			if (cmap.XmlTextCollector == null)
			{
				XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = new XmlTypeMapMemberFlatList();
				xmlTypeMapMemberFlatList.Name = classIds.AddUnique("Text", xmlTypeMapMemberFlatList);
				xmlTypeMapMemberFlatList.TypeData = TypeTranslator.GetTypeData(typeof(string[]));
				xmlTypeMapMemberFlatList.ElementInfo.Add(CreateTextElementInfo(typeQName.Namespace, xmlTypeMapMemberFlatList, xmlTypeMapMemberFlatList.TypeData.ListItemTypeData));
				xmlTypeMapMemberFlatList.IsXmlTextCollector = true;
				xmlTypeMapMemberFlatList.ListMap = new ListMap();
				xmlTypeMapMemberFlatList.ListMap.ItemInfo = xmlTypeMapMemberFlatList.ElementInfo;
				cmap.AddMember(xmlTypeMapMemberFlatList);
			}
		}

		private void ImportParticleContent(XmlQualifiedName typeQName, ClassMap cmap, XmlSchemaParticle particle, CodeIdentifiers classIds, bool multiValue, ref bool isMixed)
		{
			if (particle == null)
			{
				return;
			}
			if (particle is XmlSchemaGroupRef)
			{
				particle = GetRefGroupParticle((XmlSchemaGroupRef)particle);
			}
			if (particle.MaxOccurs > 1m)
			{
				multiValue = true;
			}
			if (particle is XmlSchemaSequence)
			{
				ImportSequenceContent(typeQName, cmap, ((XmlSchemaSequence)particle).Items, classIds, multiValue, ref isMixed);
			}
			else if (particle is XmlSchemaChoice)
			{
				if (((XmlSchemaChoice)particle).Items.Count == 1)
				{
					ImportSequenceContent(typeQName, cmap, ((XmlSchemaChoice)particle).Items, classIds, multiValue, ref isMixed);
				}
				else
				{
					ImportChoiceContent(typeQName, cmap, (XmlSchemaChoice)particle, classIds, multiValue);
				}
			}
			else if (particle is XmlSchemaAll)
			{
				ImportSequenceContent(typeQName, cmap, ((XmlSchemaAll)particle).Items, classIds, multiValue, ref isMixed);
			}
		}

		private void ImportSequenceContent(XmlQualifiedName typeQName, ClassMap cmap, XmlSchemaObjectCollection items, CodeIdentifiers classIds, bool multiValue, ref bool isMixed)
		{
			foreach (XmlSchemaObject item in items)
			{
				if (item is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)item;
					XmlTypeMapping map;
					TypeData typeData = GetElementTypeData(typeQName, xmlSchemaElement, null, out map);
					string ns;
					XmlSchemaElement refElement = GetRefElement(typeQName, xmlSchemaElement, out ns);
					if (xmlSchemaElement.MaxOccurs == 1m && !multiValue)
					{
						XmlTypeMapMemberElement xmlTypeMapMemberElement = null;
						if (typeData.SchemaType != SchemaTypes.Array)
						{
							xmlTypeMapMemberElement = new XmlTypeMapMemberElement();
							if (refElement.DefaultValue != null)
							{
								xmlTypeMapMemberElement.DefaultValue = ImportDefaultValue(typeData, refElement.DefaultValue);
							}
						}
						else if (GetTypeMapping(typeData).IsSimpleType)
						{
							xmlTypeMapMemberElement = new XmlTypeMapMemberElement();
							typeData = TypeTranslator.GetTypeData(typeof(string));
						}
						else
						{
							xmlTypeMapMemberElement = new XmlTypeMapMemberList();
						}
						if (xmlSchemaElement.MinOccurs == 0m && typeData.IsValueType)
						{
							xmlTypeMapMemberElement.IsOptionalValueType = true;
						}
						xmlTypeMapMemberElement.Name = classIds.AddUnique(CodeIdentifier.MakeValid(refElement.Name), xmlTypeMapMemberElement);
						xmlTypeMapMemberElement.Documentation = GetDocumentation(xmlSchemaElement);
						xmlTypeMapMemberElement.TypeData = typeData;
						xmlTypeMapMemberElement.ElementInfo.Add(CreateElementInfo(ns, xmlTypeMapMemberElement, refElement.Name, typeData, refElement.IsNillable, refElement.Form, map));
						cmap.AddMember(xmlTypeMapMemberElement);
					}
					else
					{
						XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = new XmlTypeMapMemberFlatList();
						xmlTypeMapMemberFlatList.ListMap = new ListMap();
						xmlTypeMapMemberFlatList.Name = classIds.AddUnique(CodeIdentifier.MakeValid(refElement.Name), xmlTypeMapMemberFlatList);
						xmlTypeMapMemberFlatList.Documentation = GetDocumentation(xmlSchemaElement);
						xmlTypeMapMemberFlatList.TypeData = typeData.ListTypeData;
						xmlTypeMapMemberFlatList.ElementInfo.Add(CreateElementInfo(ns, xmlTypeMapMemberFlatList, refElement.Name, typeData, refElement.IsNillable, refElement.Form, map));
						xmlTypeMapMemberFlatList.ListMap.ItemInfo = xmlTypeMapMemberFlatList.ElementInfo;
						cmap.AddMember(xmlTypeMapMemberFlatList);
					}
				}
				else if (item is XmlSchemaAny)
				{
					XmlSchemaAny xmlSchemaAny = (XmlSchemaAny)item;
					XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = new XmlTypeMapMemberAnyElement();
					xmlTypeMapMemberAnyElement.Name = classIds.AddUnique("Any", xmlTypeMapMemberAnyElement);
					xmlTypeMapMemberAnyElement.Documentation = GetDocumentation(xmlSchemaAny);
					Type type = ((!(xmlSchemaAny.MaxOccurs != 1m) && !multiValue) ? ((!isMixed) ? typeof(XmlElement) : typeof(XmlNode)) : ((!isMixed) ? typeof(XmlElement[]) : typeof(XmlNode[])));
					xmlTypeMapMemberAnyElement.TypeData = TypeTranslator.GetTypeData(type);
					XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(xmlTypeMapMemberAnyElement, xmlTypeMapMemberAnyElement.TypeData);
					xmlTypeMapElementInfo.IsUnnamedAnyElement = true;
					xmlTypeMapMemberAnyElement.ElementInfo.Add(xmlTypeMapElementInfo);
					if (isMixed)
					{
						xmlTypeMapElementInfo = CreateTextElementInfo(typeQName.Namespace, xmlTypeMapMemberAnyElement, xmlTypeMapMemberAnyElement.TypeData);
						xmlTypeMapMemberAnyElement.ElementInfo.Add(xmlTypeMapElementInfo);
						xmlTypeMapMemberAnyElement.IsXmlTextCollector = true;
						isMixed = false;
					}
					cmap.AddMember(xmlTypeMapMemberAnyElement);
				}
				else if (item is XmlSchemaParticle)
				{
					ImportParticleContent(typeQName, cmap, (XmlSchemaParticle)item, classIds, multiValue, ref isMixed);
				}
			}
		}

		private object ImportDefaultValue(TypeData typeData, string value)
		{
			if (typeData.SchemaType == SchemaTypes.Enum)
			{
				XmlTypeMapping typeMapping = GetTypeMapping(typeData);
				EnumMap enumMap = (EnumMap)typeMapping.ObjectMap;
				string enumName = enumMap.GetEnumName(typeMapping.TypeFullName, value);
				if (enumName == null)
				{
					throw new InvalidOperationException("'" + value + "' is not a valid enumeration value");
				}
				return enumName;
			}
			return XmlCustomFormatter.FromXmlString(typeData, value);
		}

		private void ImportChoiceContent(XmlQualifiedName typeQName, ClassMap cmap, XmlSchemaChoice choice, CodeIdentifiers classIds, bool multiValue)
		{
			XmlTypeMapElementInfoList xmlTypeMapElementInfoList = new XmlTypeMapElementInfoList();
			multiValue = ImportChoices(typeQName, null, xmlTypeMapElementInfoList, choice.Items) || multiValue;
			if (xmlTypeMapElementInfoList.Count == 0)
			{
				return;
			}
			if (choice.MaxOccurs > 1m)
			{
				multiValue = true;
			}
			XmlTypeMapMemberElement xmlTypeMapMemberElement;
			if (multiValue)
			{
				xmlTypeMapMemberElement = new XmlTypeMapMemberFlatList();
				xmlTypeMapMemberElement.Name = classIds.AddUnique("Items", xmlTypeMapMemberElement);
				ListMap listMap = new ListMap();
				listMap.ItemInfo = xmlTypeMapElementInfoList;
				((XmlTypeMapMemberFlatList)xmlTypeMapMemberElement).ListMap = listMap;
			}
			else
			{
				xmlTypeMapMemberElement = new XmlTypeMapMemberElement();
				xmlTypeMapMemberElement.Name = classIds.AddUnique("Item", xmlTypeMapMemberElement);
			}
			TypeData typeData = null;
			bool flag = false;
			bool flag2 = true;
			Hashtable hashtable = new Hashtable();
			for (int num = xmlTypeMapElementInfoList.Count - 1; num >= 0; num--)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)xmlTypeMapElementInfoList[num];
				if (cmap.GetElement(xmlTypeMapElementInfo.ElementName, xmlTypeMapElementInfo.Namespace) != null || xmlTypeMapElementInfoList.IndexOfElement(xmlTypeMapElementInfo.ElementName, xmlTypeMapElementInfo.Namespace) != num)
				{
					xmlTypeMapElementInfoList.RemoveAt(num);
				}
				else
				{
					if (hashtable.ContainsKey(xmlTypeMapElementInfo.TypeData))
					{
						flag = true;
					}
					else
					{
						hashtable.Add(xmlTypeMapElementInfo.TypeData, xmlTypeMapElementInfo);
					}
					TypeData typeData2 = xmlTypeMapElementInfo.TypeData;
					if (typeData2.SchemaType == SchemaTypes.Class)
					{
						XmlTypeMapping xmlTypeMapping = GetTypeMapping(typeData2);
						BuildPendingMap(xmlTypeMapping);
						while (xmlTypeMapping.BaseMap != null)
						{
							xmlTypeMapping = xmlTypeMapping.BaseMap;
							BuildPendingMap(xmlTypeMapping);
							typeData2 = xmlTypeMapping.TypeData;
						}
					}
					if (typeData == null)
					{
						typeData = typeData2;
					}
					else if (typeData != typeData2)
					{
						flag2 = false;
					}
				}
			}
			if (!flag2)
			{
				typeData = TypeTranslator.GetTypeData(typeof(object));
			}
			if (flag)
			{
				XmlTypeMapMemberElement xmlTypeMapMemberElement2 = new XmlTypeMapMemberElement();
				xmlTypeMapMemberElement2.Ignore = true;
				xmlTypeMapMemberElement2.Name = classIds.AddUnique(xmlTypeMapMemberElement.Name + "ElementName", xmlTypeMapMemberElement2);
				xmlTypeMapMemberElement.ChoiceMember = xmlTypeMapMemberElement2.Name;
				XmlTypeMapping xmlTypeMapping2 = CreateTypeMapping(new XmlQualifiedName(xmlTypeMapMemberElement.Name + "ChoiceType", typeQName.Namespace), SchemaTypes.Enum, null);
				xmlTypeMapping2.IncludeInSchema = false;
				CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
				EnumMap.EnumMapMember[] array = new EnumMap.EnumMapMember[xmlTypeMapElementInfoList.Count];
				for (int i = 0; i < xmlTypeMapElementInfoList.Count; i++)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo2 = (XmlTypeMapElementInfo)xmlTypeMapElementInfoList[i];
					string xmlName = ((xmlTypeMapElementInfo2.Namespace == null || !(xmlTypeMapElementInfo2.Namespace != string.Empty) || !(xmlTypeMapElementInfo2.Namespace != typeQName.Namespace)) ? xmlTypeMapElementInfo2.ElementName : (xmlTypeMapElementInfo2.Namespace + ":" + xmlTypeMapElementInfo2.ElementName));
					string enumName = codeIdentifiers.AddUnique(CodeIdentifier.MakeValid(xmlTypeMapElementInfo2.ElementName), xmlTypeMapElementInfo2);
					array[i] = new EnumMap.EnumMapMember(xmlName, enumName);
				}
				xmlTypeMapping2.ObjectMap = new EnumMap(array, false);
				xmlTypeMapMemberElement2.TypeData = ((!multiValue) ? xmlTypeMapping2.TypeData : xmlTypeMapping2.TypeData.ListTypeData);
				xmlTypeMapMemberElement2.ElementInfo.Add(CreateElementInfo(typeQName.Namespace, xmlTypeMapMemberElement2, xmlTypeMapMemberElement2.Name, xmlTypeMapMemberElement2.TypeData, false, XmlSchemaForm.None));
				cmap.AddMember(xmlTypeMapMemberElement2);
			}
			if (typeData != null)
			{
				if (multiValue)
				{
					typeData = typeData.ListTypeData;
				}
				xmlTypeMapMemberElement.ElementInfo = xmlTypeMapElementInfoList;
				xmlTypeMapMemberElement.Documentation = GetDocumentation(choice);
				xmlTypeMapMemberElement.TypeData = typeData;
				cmap.AddMember(xmlTypeMapMemberElement);
			}
		}

		private bool ImportChoices(XmlQualifiedName typeQName, XmlTypeMapMember member, XmlTypeMapElementInfoList choices, XmlSchemaObjectCollection items)
		{
			bool flag = false;
			foreach (XmlSchemaObject item in items)
			{
				XmlSchemaObject xmlSchemaObject = item;
				if (xmlSchemaObject is XmlSchemaGroupRef)
				{
					xmlSchemaObject = GetRefGroupParticle((XmlSchemaGroupRef)xmlSchemaObject);
				}
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaObject;
					XmlTypeMapping map;
					TypeData elementTypeData = GetElementTypeData(typeQName, xmlSchemaElement, null, out map);
					string ns;
					XmlSchemaElement refElement = GetRefElement(typeQName, xmlSchemaElement, out ns);
					choices.Add(CreateElementInfo(ns, member, refElement.Name, elementTypeData, refElement.IsNillable, refElement.Form, map));
					if (xmlSchemaElement.MaxOccurs > 1m)
					{
						flag = true;
					}
				}
				else if (xmlSchemaObject is XmlSchemaAny)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, TypeTranslator.GetTypeData(typeof(XmlElement)));
					xmlTypeMapElementInfo.IsUnnamedAnyElement = true;
					choices.Add(xmlTypeMapElementInfo);
				}
				else if (xmlSchemaObject is XmlSchemaChoice)
				{
					flag = ImportChoices(typeQName, member, choices, ((XmlSchemaChoice)xmlSchemaObject).Items) || flag;
				}
				else if (xmlSchemaObject is XmlSchemaSequence)
				{
					flag = ImportChoices(typeQName, member, choices, ((XmlSchemaSequence)xmlSchemaObject).Items) || flag;
				}
			}
			return flag;
		}

		private void ImportSimpleContent(XmlQualifiedName typeQName, XmlTypeMapping map, XmlSchemaSimpleContent content, CodeIdentifiers classIds, bool isMixed)
		{
			XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = content.Content as XmlSchemaSimpleContentExtension;
			ClassMap classMap = (ClassMap)map.ObjectMap;
			XmlQualifiedName contentBaseType = GetContentBaseType(content.Content);
			TypeData typeData = null;
			if (!IsPrimitiveTypeNamespace(contentBaseType.Namespace))
			{
				XmlTypeMapping xmlTypeMapping = ImportType(contentBaseType, null, true);
				BuildPendingMap(xmlTypeMapping);
				if (xmlTypeMapping.IsSimpleType)
				{
					typeData = xmlTypeMapping.TypeData;
				}
				else
				{
					ClassMap classMap2 = (ClassMap)xmlTypeMapping.ObjectMap;
					foreach (XmlTypeMapMember allMember in classMap2.AllMembers)
					{
						classMap.AddMember(allMember);
					}
					map.BaseMap = xmlTypeMapping;
					xmlTypeMapping.DerivedTypes.Add(map);
				}
			}
			else
			{
				typeData = FindBuiltInType(contentBaseType);
			}
			if (typeData != null)
			{
				XmlTypeMapMemberElement xmlTypeMapMemberElement = new XmlTypeMapMemberElement();
				xmlTypeMapMemberElement.Name = classIds.AddUnique("Value", xmlTypeMapMemberElement);
				xmlTypeMapMemberElement.TypeData = typeData;
				xmlTypeMapMemberElement.ElementInfo.Add(CreateTextElementInfo(typeQName.Namespace, xmlTypeMapMemberElement, xmlTypeMapMemberElement.TypeData));
				xmlTypeMapMemberElement.IsXmlTextCollector = true;
				classMap.AddMember(xmlTypeMapMemberElement);
			}
			if (xmlSchemaSimpleContentExtension != null)
			{
				ImportAttributes(typeQName, classMap, xmlSchemaSimpleContentExtension.Attributes, xmlSchemaSimpleContentExtension.AnyAttribute, classIds);
			}
		}

		private TypeData FindBuiltInType(XmlQualifiedName qname)
		{
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)schemas.Find(qname, typeof(XmlSchemaComplexType));
			if (xmlSchemaComplexType != null)
			{
				XmlSchemaSimpleContent xmlSchemaSimpleContent = xmlSchemaComplexType.ContentModel as XmlSchemaSimpleContent;
				if (xmlSchemaSimpleContent == null)
				{
					throw new InvalidOperationException("Invalid schema");
				}
				return FindBuiltInType(GetContentBaseType(xmlSchemaSimpleContent.Content));
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)schemas.Find(qname, typeof(XmlSchemaSimpleType));
			if (xmlSchemaSimpleType != null)
			{
				return FindBuiltInType(qname, xmlSchemaSimpleType);
			}
			if (IsPrimitiveTypeNamespace(qname.Namespace))
			{
				return TypeTranslator.GetPrimitiveTypeData(qname.Name);
			}
			throw new InvalidOperationException(string.Concat("Definition of type '", qname, "' not found"));
		}

		private TypeData FindBuiltInType(XmlQualifiedName qname, XmlSchemaSimpleType st)
		{
			if (CanBeEnum(st) && qname != null)
			{
				return ImportType(qname, null, true).TypeData;
			}
			if (st.Content is XmlSchemaSimpleTypeRestriction)
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)st.Content;
				XmlQualifiedName contentBaseType = GetContentBaseType(xmlSchemaSimpleTypeRestriction);
				if (contentBaseType == XmlQualifiedName.Empty && xmlSchemaSimpleTypeRestriction.BaseType != null)
				{
					return FindBuiltInType(qname, xmlSchemaSimpleTypeRestriction.BaseType);
				}
				return FindBuiltInType(contentBaseType);
			}
			if (st.Content is XmlSchemaSimpleTypeList)
			{
				return FindBuiltInType(GetContentBaseType(st.Content)).ListTypeData;
			}
			if (st.Content is XmlSchemaSimpleTypeUnion)
			{
				return FindBuiltInType(new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"));
			}
			return null;
		}

		private XmlQualifiedName GetContentBaseType(XmlSchemaObject ob)
		{
			if (ob is XmlSchemaSimpleContentExtension)
			{
				return ((XmlSchemaSimpleContentExtension)ob).BaseTypeName;
			}
			if (ob is XmlSchemaSimpleContentRestriction)
			{
				return ((XmlSchemaSimpleContentRestriction)ob).BaseTypeName;
			}
			if (ob is XmlSchemaSimpleTypeRestriction)
			{
				return ((XmlSchemaSimpleTypeRestriction)ob).BaseTypeName;
			}
			if (ob is XmlSchemaSimpleTypeList)
			{
				return ((XmlSchemaSimpleTypeList)ob).ItemTypeName;
			}
			return null;
		}

		private void ImportComplexContent(XmlQualifiedName typeQName, XmlTypeMapping map, XmlSchemaComplexContent content, CodeIdentifiers classIds, bool isMixed)
		{
			ClassMap classMap = (ClassMap)map.ObjectMap;
			XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = content.Content as XmlSchemaComplexContentExtension;
			XmlQualifiedName xmlQualifiedName = ((xmlSchemaComplexContentExtension == null) ? ((XmlSchemaComplexContentRestriction)content.Content).BaseTypeName : xmlSchemaComplexContentExtension.BaseTypeName);
			if (xmlQualifiedName == typeQName)
			{
				throw new InvalidOperationException("Cannot import schema for type '" + typeQName.Name + "' from namespace '" + typeQName.Namespace + "'. Redefine not supported");
			}
			XmlTypeMapping xmlTypeMapping = ImportClass(xmlQualifiedName);
			BuildPendingMap(xmlTypeMapping);
			ClassMap classMap2 = (ClassMap)xmlTypeMapping.ObjectMap;
			foreach (XmlTypeMapMember allMember in classMap2.AllMembers)
			{
				classMap.AddMember(allMember);
			}
			if (classMap2.XmlTextCollector != null)
			{
				isMixed = false;
			}
			else if (content.IsMixed)
			{
				isMixed = true;
			}
			map.BaseMap = xmlTypeMapping;
			xmlTypeMapping.DerivedTypes.Add(map);
			if (xmlSchemaComplexContentExtension != null)
			{
				ImportParticleComplexContent(typeQName, classMap, xmlSchemaComplexContentExtension.Particle, classIds, isMixed);
				ImportAttributes(typeQName, classMap, xmlSchemaComplexContentExtension.Attributes, xmlSchemaComplexContentExtension.AnyAttribute, classIds);
			}
			else if (isMixed)
			{
				ImportParticleComplexContent(typeQName, classMap, null, classIds, true);
			}
		}

		private void ImportExtensionTypes(XmlQualifiedName qname)
		{
			foreach (XmlSchema schema in schemas)
			{
				foreach (XmlSchemaObject item in schema.Items)
				{
					XmlSchemaComplexType xmlSchemaComplexType = item as XmlSchemaComplexType;
					if (xmlSchemaComplexType != null && xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
					{
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = xmlSchemaComplexType.ContentModel.Content as XmlSchemaComplexContentExtension;
						XmlQualifiedName xmlQualifiedName = ((xmlSchemaComplexContentExtension == null) ? ((XmlSchemaComplexContentRestriction)xmlSchemaComplexType.ContentModel.Content).BaseTypeName : xmlSchemaComplexContentExtension.BaseTypeName);
						if (xmlQualifiedName == qname)
						{
							ImportType(new XmlQualifiedName(xmlSchemaComplexType.Name, schema.TargetNamespace), xmlSchemaComplexType, null);
						}
					}
				}
			}
		}

		private XmlTypeMapping ImportClassSimpleType(XmlQualifiedName typeQName, XmlSchemaSimpleType stype, XmlQualifiedName root)
		{
			if (CanBeEnum(stype))
			{
				CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
				XmlTypeMapping xmlTypeMapping = CreateTypeMapping(typeQName, SchemaTypes.Enum, root);
				xmlTypeMapping.Documentation = GetDocumentation(stype);
				bool isFlags = false;
				if (stype.Content is XmlSchemaSimpleTypeList)
				{
					stype = ((XmlSchemaSimpleTypeList)stype.Content).ItemType;
					isFlags = true;
				}
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)stype.Content;
				codeIdentifiers.AddReserved(xmlTypeMapping.TypeData.TypeName);
				EnumMap.EnumMapMember[] array = new EnumMap.EnumMapMember[xmlSchemaSimpleTypeRestriction.Facets.Count];
				for (int i = 0; i < xmlSchemaSimpleTypeRestriction.Facets.Count; i++)
				{
					XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = (XmlSchemaEnumerationFacet)xmlSchemaSimpleTypeRestriction.Facets[i];
					string enumName = codeIdentifiers.AddUnique(CodeIdentifier.MakeValid(xmlSchemaEnumerationFacet.Value), xmlSchemaEnumerationFacet);
					array[i] = new EnumMap.EnumMapMember(xmlSchemaEnumerationFacet.Value, enumName);
					array[i].Documentation = GetDocumentation(xmlSchemaEnumerationFacet);
				}
				xmlTypeMapping.ObjectMap = new EnumMap(array, isFlags);
				xmlTypeMapping.IsSimpleType = true;
				return xmlTypeMapping;
			}
			if (stype.Content is XmlSchemaSimpleTypeList)
			{
				XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)stype.Content;
				TypeData typeData = FindBuiltInType(xmlSchemaSimpleTypeList.ItemTypeName, stype);
				ListMap listMap = new ListMap();
				listMap.ItemInfo = new XmlTypeMapElementInfoList();
				listMap.ItemInfo.Add(CreateElementInfo(typeQName.Namespace, null, "Item", typeData.ListItemTypeData, false, XmlSchemaForm.None));
				XmlTypeMapping xmlTypeMapping2 = CreateArrayTypeMapping(typeQName, typeData);
				xmlTypeMapping2.ObjectMap = listMap;
				xmlTypeMapping2.IsSimpleType = true;
				return xmlTypeMapping2;
			}
			TypeData typeData2 = FindBuiltInType(typeQName, stype);
			XmlTypeMapping typeMapping = GetTypeMapping(typeData2);
			typeMapping.IsSimpleType = true;
			return typeMapping;
		}

		private bool CanBeEnum(XmlSchemaSimpleType stype)
		{
			if (stype.Content is XmlSchemaSimpleTypeRestriction)
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)stype.Content;
				if (xmlSchemaSimpleTypeRestriction.Facets.Count == 0)
				{
					return false;
				}
				foreach (XmlSchemaObject facet in xmlSchemaSimpleTypeRestriction.Facets)
				{
					if (!(facet is XmlSchemaEnumerationFacet))
					{
						return false;
					}
				}
				return true;
			}
			if (stype.Content is XmlSchemaSimpleTypeList)
			{
				XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)stype.Content;
				return xmlSchemaSimpleTypeList.ItemType != null && CanBeEnum(xmlSchemaSimpleTypeList.ItemType);
			}
			return false;
		}

		private bool CanBeArray(XmlQualifiedName typeQName, XmlSchemaComplexType stype)
		{
			if (encodedFormat)
			{
				XmlSchemaComplexContent xmlSchemaComplexContent = stype.ContentModel as XmlSchemaComplexContent;
				if (xmlSchemaComplexContent == null)
				{
					return false;
				}
				XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = xmlSchemaComplexContent.Content as XmlSchemaComplexContentRestriction;
				if (xmlSchemaComplexContentRestriction == null)
				{
					return false;
				}
				return xmlSchemaComplexContentRestriction.BaseTypeName == arrayType;
			}
			if (stype.Attributes.Count > 0 || stype.AnyAttribute != null)
			{
				return false;
			}
			return !stype.IsMixed && CanBeArray(typeQName, stype.Particle, false);
		}

		private bool CanBeArray(XmlQualifiedName typeQName, XmlSchemaParticle particle, bool multiValue)
		{
			if (particle == null)
			{
				return false;
			}
			multiValue = multiValue || particle.MaxOccurs > 1m;
			if (particle is XmlSchemaGroupRef)
			{
				return CanBeArray(typeQName, GetRefGroupParticle((XmlSchemaGroupRef)particle), multiValue);
			}
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)particle;
				if (!xmlSchemaElement.RefName.IsEmpty)
				{
					return CanBeArray(typeQName, FindRefElement(xmlSchemaElement), multiValue);
				}
				return multiValue && !typeQName.Equals(((XmlSchemaElement)particle).SchemaTypeName);
			}
			if (particle is XmlSchemaAny)
			{
				return multiValue;
			}
			if (particle is XmlSchemaSequence)
			{
				XmlSchemaSequence xmlSchemaSequence = particle as XmlSchemaSequence;
				if (xmlSchemaSequence.Items.Count != 1)
				{
					return false;
				}
				return CanBeArray(typeQName, (XmlSchemaParticle)xmlSchemaSequence.Items[0], multiValue);
			}
			if (particle is XmlSchemaChoice)
			{
				ArrayList types = new ArrayList();
				if (!CheckChoiceType(typeQName, particle, types, ref multiValue))
				{
					return false;
				}
				return multiValue;
			}
			return false;
		}

		private bool CheckChoiceType(XmlQualifiedName typeQName, XmlSchemaParticle particle, ArrayList types, ref bool multiValue)
		{
			XmlQualifiedName xmlQualifiedName = null;
			multiValue = multiValue || particle.MaxOccurs > 1m;
			if (particle is XmlSchemaGroupRef)
			{
				return CheckChoiceType(typeQName, GetRefGroupParticle((XmlSchemaGroupRef)particle), types, ref multiValue);
			}
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement elem = (XmlSchemaElement)particle;
				string ns;
				XmlSchemaElement refElement = GetRefElement(typeQName, elem, out ns);
				if (refElement.SchemaType != null)
				{
					return true;
				}
				xmlQualifiedName = refElement.SchemaTypeName;
			}
			else if (particle is XmlSchemaAny)
			{
				xmlQualifiedName = anyType;
			}
			else
			{
				if (particle is XmlSchemaSequence)
				{
					XmlSchemaSequence xmlSchemaSequence = particle as XmlSchemaSequence;
					foreach (XmlSchemaParticle item in xmlSchemaSequence.Items)
					{
						if (!CheckChoiceType(typeQName, item, types, ref multiValue))
						{
							return false;
						}
					}
					return true;
				}
				if (particle is XmlSchemaChoice)
				{
					foreach (XmlSchemaParticle item2 in ((XmlSchemaChoice)particle).Items)
					{
						if (!CheckChoiceType(typeQName, item2, types, ref multiValue))
						{
							return false;
						}
					}
					return true;
				}
			}
			if (typeQName.Equals(xmlQualifiedName))
			{
				return false;
			}
			string text = ((!IsPrimitiveTypeNamespace(xmlQualifiedName.Namespace)) ? (xmlQualifiedName.Name + ":" + xmlQualifiedName.Namespace) : (TypeTranslator.GetPrimitiveTypeData(xmlQualifiedName.Name).FullTypeName + ":" + xmlQualifiedName.Namespace));
			if (types.Contains(text))
			{
				return false;
			}
			types.Add(text);
			return true;
		}

		private bool CanBeAnyElement(XmlSchemaComplexType stype)
		{
			XmlSchemaSequence xmlSchemaSequence = stype.Particle as XmlSchemaSequence;
			return xmlSchemaSequence != null && xmlSchemaSequence.Items.Count == 1 && xmlSchemaSequence.Items[0] is XmlSchemaAny;
		}

		private Type GetAnyElementType(XmlSchemaComplexType stype)
		{
			XmlSchemaSequence xmlSchemaSequence = stype.Particle as XmlSchemaSequence;
			if (xmlSchemaSequence == null || xmlSchemaSequence.Items.Count != 1 || !(xmlSchemaSequence.Items[0] is XmlSchemaAny))
			{
				return null;
			}
			if (encodedFormat)
			{
				return typeof(object);
			}
			XmlSchemaAny xmlSchemaAny = xmlSchemaSequence.Items[0] as XmlSchemaAny;
			if (xmlSchemaAny.MaxOccurs == 1m)
			{
				if (stype.IsMixed)
				{
					return typeof(XmlNode);
				}
				return typeof(XmlElement);
			}
			if (stype.IsMixed)
			{
				return typeof(XmlNode[]);
			}
			return typeof(XmlElement[]);
		}

		private bool CanBeIXmlSerializable(XmlSchemaComplexType stype)
		{
			XmlSchemaSequence xmlSchemaSequence = stype.Particle as XmlSchemaSequence;
			if (xmlSchemaSequence == null)
			{
				return false;
			}
			if (xmlSchemaSequence.Items.Count != 2)
			{
				return false;
			}
			XmlSchemaElement xmlSchemaElement = xmlSchemaSequence.Items[0] as XmlSchemaElement;
			if (xmlSchemaElement == null)
			{
				return false;
			}
			if (xmlSchemaElement.RefName != new XmlQualifiedName("schema", "http://www.w3.org/2001/XMLSchema"))
			{
				return false;
			}
			return xmlSchemaSequence.Items[1] is XmlSchemaAny;
		}

		private XmlTypeMapping ImportXmlSerializableMapping(string ns)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("System.Data.DataSet", ns);
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(xmlQualifiedName);
			if (registeredTypeMapping != null)
			{
				return registeredTypeMapping;
			}
			TypeData typeData = new TypeData("System.Data.DataSet", "System.Data.DataSet", "System.Data.DataSet", SchemaTypes.XmlSerializable, null);
			registeredTypeMapping = new XmlTypeMapping("System.Data.DataSet", string.Empty, typeData, "System.Data.DataSet", ns);
			registeredTypeMapping.IncludeInSchema = true;
			RegisterTypeMapping(xmlQualifiedName, typeData, registeredTypeMapping);
			return registeredTypeMapping;
		}

		private XmlTypeMapElementInfo CreateElementInfo(string ns, XmlTypeMapMember member, string name, TypeData typeData, bool isNillable, XmlSchemaForm form)
		{
			if (typeData.IsComplexType)
			{
				return CreateElementInfo(ns, member, name, typeData, isNillable, form, GetTypeMapping(typeData));
			}
			return CreateElementInfo(ns, member, name, typeData, isNillable, form, null);
		}

		private XmlTypeMapElementInfo CreateElementInfo(string ns, XmlTypeMapMember member, string name, TypeData typeData, bool isNillable, XmlSchemaForm form, XmlTypeMapping emap)
		{
			XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, typeData);
			xmlTypeMapElementInfo.ElementName = name;
			xmlTypeMapElementInfo.Namespace = ns;
			xmlTypeMapElementInfo.IsNullable = isNillable;
			xmlTypeMapElementInfo.Form = form;
			if (typeData.IsComplexType)
			{
				xmlTypeMapElementInfo.MappedType = emap;
			}
			return xmlTypeMapElementInfo;
		}

		private XmlTypeMapElementInfo CreateTextElementInfo(string ns, XmlTypeMapMember member, TypeData typeData)
		{
			XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(member, typeData);
			xmlTypeMapElementInfo.IsTextElement = true;
			xmlTypeMapElementInfo.WrappedElement = false;
			if (typeData.IsComplexType)
			{
				xmlTypeMapElementInfo.MappedType = GetTypeMapping(typeData);
			}
			return xmlTypeMapElementInfo;
		}

		private XmlTypeMapping CreateTypeMapping(XmlQualifiedName typeQName, SchemaTypes schemaType, XmlQualifiedName root)
		{
			string identifier = CodeIdentifier.MakeValid(typeQName.Name);
			identifier = typeIdentifiers.AddUnique(identifier, null);
			TypeData typeData = new TypeData(identifier, identifier, identifier, schemaType, null);
			string name;
			string ns;
			if (root != null)
			{
				name = root.Name;
				ns = root.Namespace;
			}
			else
			{
				name = typeQName.Name;
				ns = string.Empty;
			}
			XmlTypeMapping xmlTypeMapping = new XmlTypeMapping(name, ns, typeData, typeQName.Name, typeQName.Namespace);
			xmlTypeMapping.IncludeInSchema = true;
			RegisterTypeMapping(typeQName, typeData, xmlTypeMapping);
			return xmlTypeMapping;
		}

		private XmlTypeMapping CreateArrayTypeMapping(XmlQualifiedName typeQName, TypeData arrayTypeData)
		{
			XmlTypeMapping xmlTypeMapping = ((!encodedFormat) ? new XmlTypeMapping(arrayTypeData.XmlType, typeQName.Namespace, arrayTypeData, arrayTypeData.XmlType, typeQName.Namespace) : new XmlTypeMapping("Array", "http://schemas.xmlsoap.org/soap/encoding/", arrayTypeData, "Array", "http://schemas.xmlsoap.org/soap/encoding/"));
			xmlTypeMapping.IncludeInSchema = true;
			RegisterTypeMapping(typeQName, arrayTypeData, xmlTypeMapping);
			return xmlTypeMapping;
		}

		private XmlSchemaElement GetRefElement(XmlQualifiedName typeQName, XmlSchemaElement elem, out string ns)
		{
			if (!elem.RefName.IsEmpty)
			{
				ns = elem.RefName.Namespace;
				return FindRefElement(elem);
			}
			ns = typeQName.Namespace;
			return elem;
		}

		private XmlSchemaAttribute GetRefAttribute(XmlQualifiedName typeQName, XmlSchemaAttribute attr, out string ns)
		{
			if (!attr.RefName.IsEmpty)
			{
				ns = attr.RefName.Namespace;
				XmlSchemaAttribute xmlSchemaAttribute = FindRefAttribute(attr.RefName);
				if (xmlSchemaAttribute == null)
				{
					throw new InvalidOperationException(string.Concat("The attribute ", attr.RefName, " is missing"));
				}
				return xmlSchemaAttribute;
			}
			ns = ((!attr.ParentIsSchema) ? string.Empty : typeQName.Namespace);
			return attr;
		}

		private TypeData GetElementTypeData(XmlQualifiedName typeQName, XmlSchemaElement elem, XmlQualifiedName root, out XmlTypeMapping map)
		{
			bool sharedAnnType = false;
			map = null;
			if (!elem.RefName.IsEmpty)
			{
				XmlSchemaElement xmlSchemaElement = FindRefElement(elem);
				if (xmlSchemaElement == null)
				{
					throw new InvalidOperationException("Global element not found: " + elem.RefName);
				}
				root = elem.RefName;
				elem = xmlSchemaElement;
				sharedAnnType = true;
			}
			TypeData typeData;
			if (elem.SchemaTypeName.IsEmpty)
			{
				typeData = ((elem.SchemaType != null) ? GetTypeData(elem.SchemaType, typeQName, elem.Name, sharedAnnType, root) : TypeTranslator.GetTypeData(typeof(object)));
			}
			else
			{
				typeData = GetTypeData(elem.SchemaTypeName, root, elem.IsNillable);
				map = GetRegisteredTypeMapping(typeData);
			}
			if (map == null && typeData.IsComplexType)
			{
				map = GetTypeMapping(typeData);
			}
			return typeData;
		}

		private TypeData GetAttributeTypeData(XmlQualifiedName typeQName, XmlSchemaAttribute attr)
		{
			bool sharedAnnType = false;
			if (!attr.RefName.IsEmpty)
			{
				XmlSchemaAttribute xmlSchemaAttribute = FindRefAttribute(attr.RefName);
				if (xmlSchemaAttribute == null)
				{
					throw new InvalidOperationException("Global attribute not found: " + attr.RefName);
				}
				attr = xmlSchemaAttribute;
				sharedAnnType = true;
			}
			if (!attr.SchemaTypeName.IsEmpty)
			{
				return GetTypeData(attr.SchemaTypeName, null, false);
			}
			if (attr.SchemaType == null)
			{
				return TypeTranslator.GetTypeData(typeof(string));
			}
			return GetTypeData(attr.SchemaType, typeQName, attr.Name, sharedAnnType, null);
		}

		private TypeData GetTypeData(XmlQualifiedName typeQName, XmlQualifiedName root, bool isNullable)
		{
			if (IsPrimitiveTypeNamespace(typeQName.Namespace))
			{
				XmlTypeMapping xmlTypeMapping = ImportType(typeQName, root, false);
				if (xmlTypeMapping != null)
				{
					return xmlTypeMapping.TypeData;
				}
				return TypeTranslator.GetPrimitiveTypeData(typeQName.Name, isNullable);
			}
			if (encodedFormat && typeQName.Namespace == string.Empty)
			{
				return TypeTranslator.GetPrimitiveTypeData(typeQName.Name);
			}
			return ImportType(typeQName, root, true).TypeData;
		}

		private TypeData GetTypeData(XmlSchemaType stype, XmlQualifiedName typeQNname, string propertyName, bool sharedAnnType, XmlQualifiedName root)
		{
			string identifier;
			if (sharedAnnType)
			{
				TypeData typeData = sharedAnonymousTypes[stype] as TypeData;
				if (typeData != null)
				{
					return typeData;
				}
				identifier = propertyName;
			}
			else
			{
				identifier = typeQNname.Name + typeIdentifiers.MakeRightCase(propertyName);
			}
			identifier = elemIdentifiers.AddUnique(identifier, stype);
			XmlQualifiedName name = new XmlQualifiedName(identifier, typeQNname.Namespace);
			XmlTypeMapping xmlTypeMapping = ImportType(name, stype, root);
			if (sharedAnnType)
			{
				sharedAnonymousTypes[stype] = xmlTypeMapping.TypeData;
			}
			return xmlTypeMapping.TypeData;
		}

		private XmlTypeMapping GetTypeMapping(TypeData typeData)
		{
			if (typeData.Type == typeof(object) && !anyTypeImported)
			{
				ImportAllObjectTypes();
			}
			XmlTypeMapping registeredTypeMapping = GetRegisteredTypeMapping(typeData);
			if (registeredTypeMapping != null)
			{
				return registeredTypeMapping;
			}
			if (typeData.IsListType)
			{
				XmlTypeMapping typeMapping = GetTypeMapping(typeData.ListItemTypeData);
				registeredTypeMapping = new XmlTypeMapping(typeData.XmlType, typeMapping.Namespace, typeData, typeData.XmlType, typeMapping.Namespace);
				registeredTypeMapping.IncludeInSchema = true;
				ListMap listMap = new ListMap();
				listMap.ItemInfo = new XmlTypeMapElementInfoList();
				listMap.ItemInfo.Add(CreateElementInfo(typeMapping.Namespace, null, typeData.ListItemTypeData.XmlType, typeData.ListItemTypeData, false, XmlSchemaForm.None));
				registeredTypeMapping.ObjectMap = listMap;
				RegisterTypeMapping(new XmlQualifiedName(registeredTypeMapping.ElementName, registeredTypeMapping.Namespace), typeData, registeredTypeMapping);
				return registeredTypeMapping;
			}
			if (typeData.SchemaType == SchemaTypes.Primitive || typeData.Type == typeof(object) || typeof(XmlNode).IsAssignableFrom(typeData.Type))
			{
				return CreateSystemMap(typeData);
			}
			throw new InvalidOperationException("Map for type " + typeData.TypeName + " not found");
		}

		private void AddObjectDerivedMap(XmlTypeMapping map)
		{
			TypeData typeData = TypeTranslator.GetTypeData(typeof(object));
			XmlTypeMapping xmlTypeMapping = GetRegisteredTypeMapping(typeData);
			if (xmlTypeMapping == null)
			{
				xmlTypeMapping = CreateSystemMap(typeData);
			}
			xmlTypeMapping.DerivedTypes.Add(map);
		}

		private XmlTypeMapping CreateSystemMap(TypeData typeData)
		{
			XmlTypeMapping xmlTypeMapping = new XmlTypeMapping(typeData.XmlType, "http://www.w3.org/2001/XMLSchema", typeData, typeData.XmlType, "http://www.w3.org/2001/XMLSchema");
			xmlTypeMapping.IncludeInSchema = false;
			xmlTypeMapping.ObjectMap = new ClassMap();
			dataMappedTypes[typeData] = xmlTypeMapping;
			return xmlTypeMapping;
		}

		private void ImportAllObjectTypes()
		{
			anyTypeImported = true;
			foreach (XmlSchema schema in schemas)
			{
				foreach (XmlSchemaObject item in schema.Items)
				{
					XmlSchemaComplexType xmlSchemaComplexType = item as XmlSchemaComplexType;
					if (xmlSchemaComplexType != null)
					{
						ImportType(new XmlQualifiedName(xmlSchemaComplexType.Name, schema.TargetNamespace), xmlSchemaComplexType, null);
					}
				}
			}
		}

		private XmlTypeMapping GetRegisteredTypeMapping(XmlQualifiedName typeQName, Type baseType)
		{
			if (IsPrimitiveTypeNamespace(typeQName.Namespace))
			{
				return (XmlTypeMapping)primitiveDerivedMappedTypes[typeQName];
			}
			return (XmlTypeMapping)mappedTypes[typeQName];
		}

		private XmlTypeMapping GetRegisteredTypeMapping(XmlQualifiedName typeQName)
		{
			return (XmlTypeMapping)mappedTypes[typeQName];
		}

		private XmlTypeMapping GetRegisteredTypeMapping(TypeData typeData)
		{
			return (XmlTypeMapping)dataMappedTypes[typeData];
		}

		private void RegisterTypeMapping(XmlQualifiedName qname, TypeData typeData, XmlTypeMapping map)
		{
			dataMappedTypes[typeData] = map;
			if (IsPrimitiveTypeNamespace(qname.Namespace) && !map.IsSimpleType)
			{
				primitiveDerivedMappedTypes[qname] = map;
			}
			else
			{
				mappedTypes[qname] = map;
			}
		}

		private XmlSchemaParticle GetRefGroupParticle(XmlSchemaGroupRef refGroup)
		{
			XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)schemas.Find(refGroup.RefName, typeof(XmlSchemaGroup));
			return xmlSchemaGroup.Particle;
		}

		private XmlSchemaElement FindRefElement(XmlSchemaElement elem)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)schemas.Find(elem.RefName, typeof(XmlSchemaElement));
			if (xmlSchemaElement != null)
			{
				return xmlSchemaElement;
			}
			if (IsPrimitiveTypeNamespace(elem.RefName.Namespace))
			{
				if (anyElement != null)
				{
					return anyElement;
				}
				anyElement = new XmlSchemaElement();
				anyElement.Name = "any";
				anyElement.SchemaTypeName = anyType;
				return anyElement;
			}
			return null;
		}

		private XmlSchemaAttribute FindRefAttribute(XmlQualifiedName refName)
		{
			if (refName.Namespace == "http://www.w3.org/XML/1998/namespace")
			{
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = refName.Name;
				xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
				return xmlSchemaAttribute;
			}
			return (XmlSchemaAttribute)schemas.Find(refName, typeof(XmlSchemaAttribute));
		}

		private XmlSchemaAttributeGroup FindRefAttributeGroup(XmlQualifiedName refName)
		{
			XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)schemas.Find(refName, typeof(XmlSchemaAttributeGroup));
			foreach (XmlSchemaObject attribute in xmlSchemaAttributeGroup.Attributes)
			{
				if (attribute is XmlSchemaAttributeGroupRef && ((XmlSchemaAttributeGroupRef)attribute).RefName == refName)
				{
					throw new InvalidOperationException("Cannot import attribute group '" + refName.Name + "' from namespace '" + refName.Namespace + "'. Redefine not supported");
				}
			}
			return xmlSchemaAttributeGroup;
		}

		private XmlTypeMapping ReflectType(Type type)
		{
			TypeData typeData = TypeTranslator.GetTypeData(type);
			return ReflectType(typeData, null);
		}

		private XmlTypeMapping ReflectType(TypeData typeData, string ns)
		{
			if (!encodedFormat)
			{
				if (auxXmlRefImporter == null)
				{
					auxXmlRefImporter = new XmlReflectionImporter();
				}
				return auxXmlRefImporter.ImportTypeMapping(typeData, ns);
			}
			if (auxSoapRefImporter == null)
			{
				auxSoapRefImporter = new SoapReflectionImporter();
			}
			return auxSoapRefImporter.ImportTypeMapping(typeData, ns);
		}

		private string GetDocumentation(XmlSchemaAnnotated elem)
		{
			string text = string.Empty;
			XmlSchemaAnnotation annotation = elem.Annotation;
			if (annotation == null || annotation.Items == null)
			{
				return null;
			}
			foreach (XmlSchemaObject item in annotation.Items)
			{
				XmlSchemaDocumentation xmlSchemaDocumentation = item as XmlSchemaDocumentation;
				if (xmlSchemaDocumentation != null && xmlSchemaDocumentation.Markup != null && xmlSchemaDocumentation.Markup.Length > 0)
				{
					if (text != string.Empty)
					{
						text += "\n";
					}
					XmlNode[] markup = xmlSchemaDocumentation.Markup;
					foreach (XmlNode xmlNode in markup)
					{
						text += xmlNode.Value;
					}
				}
			}
			return text;
		}

		private bool IsPrimitiveTypeNamespace(string ns)
		{
			return ns == "http://www.w3.org/2001/XMLSchema" || (encodedFormat && ns == "http://schemas.xmlsoap.org/soap/encoding/");
		}
	}
}
