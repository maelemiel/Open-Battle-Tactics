using System.Collections;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemaExporter
	{
		private class XmlSchemaObjectContainer
		{
			private readonly XmlSchemaObject _xmlSchemaObject;

			public XmlSchemaObjectCollection Items
			{
				get
				{
					if (_xmlSchemaObject is XmlSchema)
					{
						return ((XmlSchema)_xmlSchemaObject).Items;
					}
					return ((XmlSchemaGroupBase)_xmlSchemaObject).Items;
				}
			}

			public XmlSchemaObjectContainer(XmlSchema schema)
			{
				_xmlSchemaObject = schema;
			}

			public XmlSchemaObjectContainer(XmlSchemaGroupBase group)
			{
				_xmlSchemaObject = group;
			}
		}

		private XmlSchemas schemas;

		private Hashtable exportedMaps = new Hashtable();

		private Hashtable exportedElements = new Hashtable();

		private bool encodedFormat;

		private XmlDocument xmlDoc;

		private XmlDocument Document
		{
			get
			{
				if (xmlDoc == null)
				{
					xmlDoc = new XmlDocument();
				}
				return xmlDoc;
			}
		}

		public XmlSchemaExporter(XmlSchemas schemas)
		{
			this.schemas = schemas;
		}

		internal XmlSchemaExporter(XmlSchemas schemas, bool encodedFormat)
		{
			this.encodedFormat = encodedFormat;
			this.schemas = schemas;
		}

		[System.MonoTODO]
		public string ExportAnyType(string ns)
		{
			throw new NotImplementedException();
		}

		[System.MonoNotSupported("")]
		public string ExportAnyType(XmlMembersMapping members)
		{
			throw new NotImplementedException();
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			ExportMembersMapping(xmlMembersMapping, true);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping, bool exportEnclosingType)
		{
			ClassMap classMap = (ClassMap)xmlMembersMapping.ObjectMap;
			if (xmlMembersMapping.HasWrapperElement && exportEnclosingType)
			{
				XmlSchema schema = GetSchema(xmlMembersMapping.Namespace);
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence particle;
				XmlSchemaAnyAttribute anyAttribute;
				ExportMembersMapSchema(schema, classMap, null, xmlSchemaComplexType.Attributes, out particle, out anyAttribute);
				xmlSchemaComplexType.Particle = particle;
				xmlSchemaComplexType.AnyAttribute = anyAttribute;
				if (encodedFormat)
				{
					xmlSchemaComplexType.Name = xmlMembersMapping.ElementName;
					schema.Items.Add(xmlSchemaComplexType);
				}
				else
				{
					XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
					xmlSchemaElement.Name = xmlMembersMapping.ElementName;
					xmlSchemaElement.SchemaType = xmlSchemaComplexType;
					schema.Items.Add(xmlSchemaElement);
				}
			}
			else
			{
				ICollection elementMembers = classMap.ElementMembers;
				if (elementMembers != null)
				{
					foreach (XmlTypeMapMemberElement item in elementMembers)
					{
						if (item is XmlTypeMapMemberAnyElement && item.TypeData.IsListType)
						{
							XmlSchema schema2 = GetSchema(xmlMembersMapping.Namespace);
							XmlSchemaParticle schemaArrayElement = GetSchemaArrayElement(schema2, item.ElementInfo);
							if (schemaArrayElement is XmlSchemaAny)
							{
								XmlSchemaComplexType xmlSchemaComplexType2 = FindComplexType(schema2.Items, "any");
								if (xmlSchemaComplexType2 == null)
								{
									xmlSchemaComplexType2 = new XmlSchemaComplexType();
									xmlSchemaComplexType2.Name = "any";
									xmlSchemaComplexType2.IsMixed = true;
									XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)(xmlSchemaComplexType2.Particle = new XmlSchemaSequence());
									xmlSchemaSequence.Items.Add(schemaArrayElement);
									schema2.Items.Add(xmlSchemaComplexType2);
								}
								continue;
							}
						}
						XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)item.ElementInfo[0];
						XmlSchema schema3;
						if (encodedFormat)
						{
							schema3 = GetSchema(xmlMembersMapping.Namespace);
							ImportNamespace(schema3, "http://schemas.xmlsoap.org/soap/encoding/");
						}
						else
						{
							schema3 = GetSchema(xmlTypeMapElementInfo.Namespace);
						}
						XmlSchemaElement xmlSchemaElement2 = FindElement(schema3.Items, xmlTypeMapElementInfo.ElementName);
						XmlSchemaObjectContainer container = null;
						if (!encodedFormat)
						{
							container = new XmlSchemaObjectContainer(schema3);
						}
						Type type = item.GetType();
						if (item is XmlTypeMapMemberFlatList)
						{
							throw new InvalidOperationException("Unwrapped arrays not supported as parameters");
						}
						XmlSchemaElement xmlSchemaElement3 = ((type != typeof(XmlTypeMapMemberElement)) ? ((XmlSchemaElement)GetSchemaElement(schema3, xmlTypeMapElementInfo, false, container)) : ((XmlSchemaElement)GetSchemaElement(schema3, xmlTypeMapElementInfo, item.DefaultValue, false, container)));
						if (xmlSchemaElement2 != null)
						{
							if (!xmlSchemaElement2.SchemaTypeName.Equals(xmlSchemaElement3.SchemaTypeName))
							{
								string text = "The XML element named '" + xmlTypeMapElementInfo.ElementName + "' ";
								string text2 = text;
								text = text2 + "from namespace '" + schema3.TargetNamespace + "' references distinct types " + xmlSchemaElement3.SchemaTypeName.Name + " and " + xmlSchemaElement2.SchemaTypeName.Name + ". ";
								text += "Use XML attributes to specify another XML name or namespace for the element or types.";
								throw new InvalidOperationException(text);
							}
							schema3.Items.Remove(xmlSchemaElement3);
						}
					}
				}
			}
			CompileSchemas();
		}

		[System.MonoTODO]
		public XmlQualifiedName ExportTypeMapping(XmlMembersMapping xmlMembersMapping)
		{
			throw new NotImplementedException();
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
		{
			if (!xmlTypeMapping.IncludeInSchema || IsElementExported(xmlTypeMapping))
			{
				return;
			}
			if (encodedFormat)
			{
				ExportClassSchema(xmlTypeMapping);
				XmlSchema schema = GetSchema(xmlTypeMapping.XmlTypeNamespace);
				ImportNamespace(schema, "http://schemas.xmlsoap.org/soap/encoding/");
			}
			else
			{
				XmlSchema schema2 = GetSchema(xmlTypeMapping.Namespace);
				XmlTypeMapElementInfo xmlTypeMapElementInfo = new XmlTypeMapElementInfo(null, xmlTypeMapping.TypeData);
				xmlTypeMapElementInfo.Namespace = xmlTypeMapping.Namespace;
				xmlTypeMapElementInfo.ElementName = xmlTypeMapping.ElementName;
				if (xmlTypeMapping.TypeData.IsComplexType)
				{
					xmlTypeMapElementInfo.MappedType = xmlTypeMapping;
				}
				xmlTypeMapElementInfo.IsNullable = xmlTypeMapping.IsNullable;
				GetSchemaElement(schema2, xmlTypeMapElementInfo, false, new XmlSchemaObjectContainer(schema2));
				SetElementExported(xmlTypeMapping);
			}
			CompileSchemas();
		}

		private void ExportXmlSerializableSchema(XmlSchema currentSchema, XmlSerializableMapping map)
		{
			if (IsMapExported(map))
			{
				return;
			}
			SetMapExported(map);
			if (map.Schema != null)
			{
				string targetNamespace = map.Schema.TargetNamespace;
				XmlSchema xmlSchema = schemas[targetNamespace];
				if (xmlSchema == null)
				{
					schemas.Add(map.Schema);
					ImportNamespace(currentSchema, targetNamespace);
				}
				else if (xmlSchema != map.Schema && !CanBeDuplicated(xmlSchema, map.Schema))
				{
					throw new InvalidOperationException("The namespace '" + targetNamespace + "' defined by the class '" + map.TypeFullName + "' is a duplicate.");
				}
			}
		}

		private static bool CanBeDuplicated(XmlSchema existingSchema, XmlSchema schema)
		{
			if (XmlSchemas.IsDataSet(existingSchema) && XmlSchemas.IsDataSet(schema) && existingSchema.Id == schema.Id)
			{
				return true;
			}
			return false;
		}

		private void ExportClassSchema(XmlTypeMapping map)
		{
			if (IsMapExported(map))
			{
				return;
			}
			SetMapExported(map);
			if (map.TypeData.Type == typeof(object))
			{
				foreach (XmlTypeMapping derivedType in map.DerivedTypes)
				{
					if (derivedType.TypeData.SchemaType == SchemaTypes.Class)
					{
						ExportClassSchema(derivedType);
					}
				}
				return;
			}
			XmlSchema schema = GetSchema(map.XmlTypeNamespace);
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			xmlSchemaComplexType.Name = map.XmlType;
			schema.Items.Add(xmlSchemaComplexType);
			ClassMap classMap = (ClassMap)map.ObjectMap;
			if (classMap.HasSimpleContent)
			{
				XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent());
				XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = (XmlSchemaSimpleContentExtension)(xmlSchemaSimpleContent.Content = new XmlSchemaSimpleContentExtension());
				XmlSchemaSequence particle;
				XmlSchemaAnyAttribute anyAttribute;
				ExportMembersMapSchema(schema, classMap, map.BaseMap, xmlSchemaSimpleContentExtension.Attributes, out particle, out anyAttribute);
				xmlSchemaSimpleContentExtension.AnyAttribute = anyAttribute;
				if (map.BaseMap == null)
				{
					xmlSchemaSimpleContentExtension.BaseTypeName = classMap.SimpleContentBaseType;
				}
				else
				{
					xmlSchemaSimpleContentExtension.BaseTypeName = new XmlQualifiedName(map.BaseMap.XmlType, map.BaseMap.XmlTypeNamespace);
					ImportNamespace(schema, map.BaseMap.XmlTypeNamespace);
					ExportClassSchema(map.BaseMap);
				}
			}
			else if (map.BaseMap != null && map.BaseMap.IncludeInSchema)
			{
				XmlSchemaComplexContent xmlSchemaComplexContent = new XmlSchemaComplexContent();
				XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
				xmlSchemaComplexContentExtension.BaseTypeName = new XmlQualifiedName(map.BaseMap.XmlType, map.BaseMap.XmlTypeNamespace);
				xmlSchemaComplexContent.Content = xmlSchemaComplexContentExtension;
				xmlSchemaComplexType.ContentModel = xmlSchemaComplexContent;
				XmlSchemaSequence particle2;
				XmlSchemaAnyAttribute anyAttribute2;
				ExportMembersMapSchema(schema, classMap, map.BaseMap, xmlSchemaComplexContentExtension.Attributes, out particle2, out anyAttribute2);
				xmlSchemaComplexContentExtension.Particle = particle2;
				xmlSchemaComplexContentExtension.AnyAttribute = anyAttribute2;
				xmlSchemaComplexType.IsMixed = HasMixedContent(map);
				xmlSchemaComplexContent.IsMixed = BaseHasMixedContent(map);
				ImportNamespace(schema, map.BaseMap.XmlTypeNamespace);
				ExportClassSchema(map.BaseMap);
			}
			else
			{
				XmlSchemaSequence particle3;
				XmlSchemaAnyAttribute anyAttribute3;
				ExportMembersMapSchema(schema, classMap, map.BaseMap, xmlSchemaComplexType.Attributes, out particle3, out anyAttribute3);
				xmlSchemaComplexType.Particle = particle3;
				xmlSchemaComplexType.AnyAttribute = anyAttribute3;
				xmlSchemaComplexType.IsMixed = classMap.XmlTextCollector != null;
			}
			foreach (XmlTypeMapping derivedType2 in map.DerivedTypes)
			{
				if (derivedType2.TypeData.SchemaType == SchemaTypes.Class)
				{
					ExportClassSchema(derivedType2);
				}
			}
		}

		private bool BaseHasMixedContent(XmlTypeMapping map)
		{
			ClassMap classMap = (ClassMap)map.ObjectMap;
			return classMap.XmlTextCollector != null && map.BaseMap != null && DefinedInBaseMap(map.BaseMap, classMap.XmlTextCollector);
		}

		private bool HasMixedContent(XmlTypeMapping map)
		{
			ClassMap classMap = (ClassMap)map.ObjectMap;
			return classMap.XmlTextCollector != null && (map.BaseMap == null || !DefinedInBaseMap(map.BaseMap, classMap.XmlTextCollector));
		}

		private void ExportMembersMapSchema(XmlSchema schema, ClassMap map, XmlTypeMapping baseMap, XmlSchemaObjectCollection outAttributes, out XmlSchemaSequence particle, out XmlSchemaAnyAttribute anyAttribute)
		{
			particle = null;
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			ICollection elementMembers = map.ElementMembers;
			if (elementMembers != null && !map.HasSimpleContent)
			{
				foreach (XmlTypeMapMemberElement item in elementMembers)
				{
					if (baseMap != null && DefinedInBaseMap(baseMap, item))
					{
						continue;
					}
					Type type = item.GetType();
					if (type == typeof(XmlTypeMapMemberFlatList))
					{
						XmlSchemaParticle schemaArrayElement = GetSchemaArrayElement(schema, item.ElementInfo);
						if (schemaArrayElement != null)
						{
							xmlSchemaSequence.Items.Add(schemaArrayElement);
						}
					}
					else if (type == typeof(XmlTypeMapMemberAnyElement))
					{
						xmlSchemaSequence.Items.Add(GetSchemaArrayElement(schema, item.ElementInfo));
					}
					else if (type == typeof(XmlTypeMapMemberElement))
					{
						GetSchemaElement(schema, (XmlTypeMapElementInfo)item.ElementInfo[0], item.DefaultValue, true, new XmlSchemaObjectContainer(xmlSchemaSequence));
					}
					else
					{
						GetSchemaElement(schema, (XmlTypeMapElementInfo)item.ElementInfo[0], true, new XmlSchemaObjectContainer(xmlSchemaSequence));
					}
				}
			}
			if (xmlSchemaSequence.Items.Count > 0)
			{
				particle = xmlSchemaSequence;
			}
			ICollection attributeMembers = map.AttributeMembers;
			if (attributeMembers != null)
			{
				foreach (XmlTypeMapMemberAttribute item2 in attributeMembers)
				{
					if (baseMap == null || !DefinedInBaseMap(baseMap, item2))
					{
						outAttributes.Add(GetSchemaAttribute(schema, item2, true));
					}
				}
			}
			XmlTypeMapMember defaultAnyAttributeMember = map.DefaultAnyAttributeMember;
			if (defaultAnyAttributeMember != null)
			{
				anyAttribute = new XmlSchemaAnyAttribute();
			}
			else
			{
				anyAttribute = null;
			}
		}

		private XmlSchemaElement FindElement(XmlSchemaObjectCollection col, string name)
		{
			foreach (XmlSchemaObject item in col)
			{
				XmlSchemaElement xmlSchemaElement = item as XmlSchemaElement;
				if (xmlSchemaElement != null && xmlSchemaElement.Name == name)
				{
					return xmlSchemaElement;
				}
			}
			return null;
		}

		private XmlSchemaComplexType FindComplexType(XmlSchemaObjectCollection col, string name)
		{
			foreach (XmlSchemaObject item in col)
			{
				XmlSchemaComplexType xmlSchemaComplexType = item as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null && xmlSchemaComplexType.Name == name)
				{
					return xmlSchemaComplexType;
				}
			}
			return null;
		}

		private XmlSchemaAttribute GetSchemaAttribute(XmlSchema currentSchema, XmlTypeMapMemberAttribute attinfo, bool isTypeMember)
		{
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			if (attinfo.DefaultValue != DBNull.Value)
			{
				xmlSchemaAttribute.DefaultValue = ExportDefaultValue(attinfo.TypeData, attinfo.MappedType, attinfo.DefaultValue);
			}
			else if (!attinfo.IsOptionalValueType && attinfo.TypeData.IsValueType)
			{
				xmlSchemaAttribute.Use = XmlSchemaUse.Required;
			}
			ImportNamespace(currentSchema, attinfo.Namespace);
			XmlSchema xmlSchema = ((attinfo.Namespace.Length != 0 || attinfo.Form == XmlSchemaForm.Qualified) ? GetSchema(attinfo.Namespace) : currentSchema);
			if (currentSchema == xmlSchema || encodedFormat)
			{
				xmlSchemaAttribute.Name = attinfo.AttributeName;
				if (isTypeMember)
				{
					xmlSchemaAttribute.Form = attinfo.Form;
				}
				if (attinfo.TypeData.SchemaType == SchemaTypes.Enum)
				{
					ImportNamespace(currentSchema, attinfo.DataTypeNamespace);
					ExportEnumSchema(attinfo.MappedType);
					xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName(attinfo.TypeData.XmlType, attinfo.DataTypeNamespace);
				}
				else if (attinfo.TypeData.SchemaType == SchemaTypes.Array && TypeTranslator.IsPrimitive(attinfo.TypeData.ListItemType))
				{
					xmlSchemaAttribute.SchemaType = GetSchemaSimpleListType(attinfo.TypeData);
				}
				else
				{
					xmlSchemaAttribute.SchemaTypeName = new XmlQualifiedName(attinfo.TypeData.XmlType, attinfo.DataTypeNamespace);
				}
			}
			else
			{
				xmlSchemaAttribute.RefName = new XmlQualifiedName(attinfo.AttributeName, attinfo.Namespace);
				foreach (XmlSchemaObject item in xmlSchema.Items)
				{
					if (item is XmlSchemaAttribute && ((XmlSchemaAttribute)item).Name == attinfo.AttributeName)
					{
						return xmlSchemaAttribute;
					}
				}
				xmlSchema.Items.Add(GetSchemaAttribute(xmlSchema, attinfo, false));
			}
			return xmlSchemaAttribute;
		}

		private XmlSchemaParticle GetSchemaElement(XmlSchema currentSchema, XmlTypeMapElementInfo einfo, bool isTypeMember)
		{
			return GetSchemaElement(currentSchema, einfo, DBNull.Value, isTypeMember, null);
		}

		private XmlSchemaParticle GetSchemaElement(XmlSchema currentSchema, XmlTypeMapElementInfo einfo, bool isTypeMember, XmlSchemaObjectContainer container)
		{
			return GetSchemaElement(currentSchema, einfo, DBNull.Value, isTypeMember, container);
		}

		private XmlSchemaParticle GetSchemaElement(XmlSchema currentSchema, XmlTypeMapElementInfo einfo, object defaultValue, bool isTypeMember, XmlSchemaObjectContainer container)
		{
			if (einfo.IsTextElement)
			{
				return null;
			}
			if (einfo.IsUnnamedAnyElement)
			{
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = 1m;
				if (container != null)
				{
					container.Items.Add(xmlSchemaAny);
				}
				return xmlSchemaAny;
			}
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.IsNillable = einfo.IsNullable;
			if (container != null)
			{
				container.Items.Add(xmlSchemaElement);
			}
			if (isTypeMember)
			{
				xmlSchemaElement.MaxOccurs = 1m;
				xmlSchemaElement.MinOccurs = (einfo.IsNullable ? 1 : 0);
				if ((defaultValue == DBNull.Value && einfo.TypeData.IsValueType && einfo.Member != null && !einfo.Member.IsOptionalValueType) || encodedFormat)
				{
					xmlSchemaElement.MinOccurs = 1m;
				}
			}
			XmlSchema xmlSchema = null;
			if (!encodedFormat)
			{
				xmlSchema = GetSchema(einfo.Namespace);
				ImportNamespace(currentSchema, einfo.Namespace);
			}
			if (currentSchema == xmlSchema || encodedFormat || !isTypeMember)
			{
				if (isTypeMember)
				{
					xmlSchemaElement.IsNillable = einfo.IsNullable;
				}
				xmlSchemaElement.Name = einfo.ElementName;
				if (defaultValue != DBNull.Value)
				{
					xmlSchemaElement.DefaultValue = ExportDefaultValue(einfo.TypeData, einfo.MappedType, defaultValue);
				}
				if (einfo.Form != XmlSchemaForm.Qualified)
				{
					xmlSchemaElement.Form = einfo.Form;
				}
				switch (einfo.TypeData.SchemaType)
				{
				case SchemaTypes.XmlNode:
					xmlSchemaElement.SchemaType = GetSchemaXmlNodeType();
					break;
				case SchemaTypes.XmlSerializable:
					SetSchemaXmlSerializableType(einfo.MappedType as XmlSerializableMapping, xmlSchemaElement);
					ExportXmlSerializableSchema(currentSchema, einfo.MappedType as XmlSerializableMapping);
					break;
				case SchemaTypes.Enum:
					xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(einfo.MappedType.XmlType, einfo.MappedType.XmlTypeNamespace);
					ImportNamespace(currentSchema, einfo.MappedType.XmlTypeNamespace);
					ExportEnumSchema(einfo.MappedType);
					break;
				case SchemaTypes.Array:
				{
					XmlQualifiedName xmlQualifiedName = (xmlSchemaElement.SchemaTypeName = ExportArraySchema(einfo.MappedType, currentSchema.TargetNamespace));
					ImportNamespace(currentSchema, xmlQualifiedName.Namespace);
					break;
				}
				case SchemaTypes.Class:
					if (einfo.MappedType.TypeData.Type != typeof(object))
					{
						xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(einfo.MappedType.XmlType, einfo.MappedType.XmlTypeNamespace);
						ImportNamespace(currentSchema, einfo.MappedType.XmlTypeNamespace);
					}
					else if (encodedFormat)
					{
						xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(einfo.MappedType.XmlType, einfo.MappedType.XmlTypeNamespace);
					}
					ExportClassSchema(einfo.MappedType);
					break;
				case SchemaTypes.Primitive:
					xmlSchemaElement.SchemaTypeName = new XmlQualifiedName(einfo.TypeData.XmlType, einfo.DataTypeNamespace);
					if (!einfo.TypeData.IsXsdType)
					{
						ImportNamespace(currentSchema, einfo.MappedType.XmlTypeNamespace);
						ExportDerivedSchema(einfo.MappedType);
					}
					break;
				}
			}
			else
			{
				xmlSchemaElement.RefName = new XmlQualifiedName(einfo.ElementName, einfo.Namespace);
				foreach (XmlSchemaObject item in xmlSchema.Items)
				{
					if (item is XmlSchemaElement && ((XmlSchemaElement)item).Name == einfo.ElementName)
					{
						return xmlSchemaElement;
					}
				}
				GetSchemaElement(xmlSchema, einfo, defaultValue, false, new XmlSchemaObjectContainer(xmlSchema));
			}
			return xmlSchemaElement;
		}

		private void ImportNamespace(XmlSchema schema, string ns)
		{
			if (ns == null || ns.Length == 0 || ns == schema.TargetNamespace || ns == "http://www.w3.org/2001/XMLSchema")
			{
				return;
			}
			foreach (XmlSchemaObject include in schema.Includes)
			{
				if (include is XmlSchemaImport && ((XmlSchemaImport)include).Namespace == ns)
				{
					return;
				}
			}
			XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
			xmlSchemaImport.Namespace = ns;
			schema.Includes.Add(xmlSchemaImport);
		}

		private bool DefinedInBaseMap(XmlTypeMapping map, XmlTypeMapMember member)
		{
			if (((ClassMap)map.ObjectMap).FindMember(member.Name) != null)
			{
				return true;
			}
			if (map.BaseMap != null)
			{
				return DefinedInBaseMap(map.BaseMap, member);
			}
			return false;
		}

		private XmlSchemaType GetSchemaXmlNodeType()
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			xmlSchemaComplexType.IsMixed = true;
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			xmlSchemaSequence.Items.Add(new XmlSchemaAny());
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			return xmlSchemaComplexType;
		}

		private void SetSchemaXmlSerializableType(XmlSerializableMapping map, XmlSchemaElement elem)
		{
			if (map.SchemaType != null && map.Schema != null)
			{
				elem.SchemaType = map.SchemaType;
				return;
			}
			if (map.SchemaType == null && map.SchemaTypeName != null)
			{
				elem.SchemaTypeName = map.SchemaTypeName;
				elem.Name = map.SchemaTypeName.Name;
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			if (map.Schema == null)
			{
				XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
				xmlSchemaElement.RefName = new XmlQualifiedName("schema", "http://www.w3.org/2001/XMLSchema");
				xmlSchemaSequence.Items.Add(xmlSchemaElement);
				xmlSchemaSequence.Items.Add(new XmlSchemaAny());
			}
			else
			{
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = map.Schema.TargetNamespace;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
			}
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			elem.SchemaType = xmlSchemaComplexType;
		}

		private XmlSchemaSimpleType GetSchemaSimpleListType(TypeData typeData)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList();
			TypeData typeData2 = TypeTranslator.GetTypeData(typeData.ListItemType);
			xmlSchemaSimpleTypeList.ItemTypeName = new XmlQualifiedName(typeData2.XmlType, "http://www.w3.org/2001/XMLSchema");
			xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeList;
			return xmlSchemaSimpleType;
		}

		private XmlSchemaParticle GetSchemaArrayElement(XmlSchema currentSchema, XmlTypeMapElementInfoList infos)
		{
			int num = infos.Count;
			if (num > 0 && ((XmlTypeMapElementInfo)infos[0]).IsTextElement)
			{
				num--;
			}
			switch (num)
			{
			case 0:
				return null;
			case 1:
			{
				XmlSchemaParticle schemaElement = GetSchemaElement(currentSchema, (XmlTypeMapElementInfo)infos[infos.Count - 1], true);
				schemaElement.MinOccursString = "0";
				schemaElement.MaxOccursString = "unbounded";
				return schemaElement;
			}
			default:
			{
				XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
				xmlSchemaChoice.MinOccursString = "0";
				xmlSchemaChoice.MaxOccursString = "unbounded";
				{
					foreach (XmlTypeMapElementInfo info in infos)
					{
						if (!info.IsTextElement)
						{
							xmlSchemaChoice.Items.Add(GetSchemaElement(currentSchema, info, true));
						}
					}
					return xmlSchemaChoice;
				}
			}
			}
		}

		private string ExportDefaultValue(TypeData typeData, XmlTypeMapping map, object defaultValue)
		{
			if (typeData.SchemaType == SchemaTypes.Enum)
			{
				EnumMap enumMap = (EnumMap)map.ObjectMap;
				return enumMap.GetXmlName(map.TypeFullName, defaultValue);
			}
			return XmlCustomFormatter.ToXmlString(typeData, defaultValue);
		}

		private void ExportDerivedSchema(XmlTypeMapping map)
		{
			if (IsMapExported(map))
			{
				return;
			}
			SetMapExported(map);
			XmlSchema schema = GetSchema(map.XmlTypeNamespace);
			for (int i = 0; i < schema.Items.Count; i++)
			{
				XmlSchemaSimpleType xmlSchemaSimpleType = schema.Items[i] as XmlSchemaSimpleType;
				if (xmlSchemaSimpleType != null && xmlSchemaSimpleType.Name == map.ElementName)
				{
					return;
				}
			}
			XmlSchemaSimpleType xmlSchemaSimpleType2 = new XmlSchemaSimpleType();
			xmlSchemaSimpleType2.Name = map.ElementName;
			schema.Items.Add(xmlSchemaSimpleType2);
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
			xmlSchemaSimpleTypeRestriction.BaseTypeName = new XmlQualifiedName(map.TypeData.MappedType.XmlType, "http://www.w3.org/2001/XMLSchema");
			XmlSchemaPatternFacet xmlSchemaPatternFacet = map.TypeData.XmlSchemaPatternFacet;
			if (xmlSchemaPatternFacet != null)
			{
				xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaPatternFacet);
			}
			xmlSchemaSimpleType2.Content = xmlSchemaSimpleTypeRestriction;
		}

		private void ExportEnumSchema(XmlTypeMapping map)
		{
			if (!IsMapExported(map))
			{
				SetMapExported(map);
				XmlSchema schema = GetSchema(map.XmlTypeNamespace);
				XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
				xmlSchemaSimpleType.Name = map.ElementName;
				schema.Items.Add(xmlSchemaSimpleType);
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
				xmlSchemaSimpleTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
				EnumMap enumMap = (EnumMap)map.ObjectMap;
				EnumMap.EnumMapMember[] members = enumMap.Members;
				foreach (EnumMap.EnumMapMember enumMapMember in members)
				{
					XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = new XmlSchemaEnumerationFacet();
					xmlSchemaEnumerationFacet.Value = enumMapMember.XmlName;
					xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaEnumerationFacet);
				}
				if (enumMap.IsFlags)
				{
					XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList();
					XmlSchemaSimpleType xmlSchemaSimpleType2 = new XmlSchemaSimpleType();
					xmlSchemaSimpleType2.Content = xmlSchemaSimpleTypeRestriction;
					xmlSchemaSimpleTypeList.ItemType = xmlSchemaSimpleType2;
					xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeList;
				}
				else
				{
					xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeRestriction;
				}
			}
		}

		private XmlQualifiedName ExportArraySchema(XmlTypeMapping map, string defaultNamespace)
		{
			ListMap listMap = (ListMap)map.ObjectMap;
			if (encodedFormat)
			{
				string localName;
				string ns;
				listMap.GetArrayType(-1, out localName, out ns);
				string text = ((!(ns == "http://www.w3.org/2001/XMLSchema")) ? ns : defaultNamespace);
				if (IsMapExported(map))
				{
					return new XmlQualifiedName(listMap.GetSchemaArrayName(), text);
				}
				SetMapExported(map);
				XmlSchema schema = GetSchema(text);
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = listMap.GetSchemaArrayName();
				schema.Items.Add(xmlSchemaComplexType);
				XmlSchemaComplexContent xmlSchemaComplexContent = new XmlSchemaComplexContent();
				xmlSchemaComplexContent.IsMixed = false;
				xmlSchemaComplexType.ContentModel = xmlSchemaComplexContent;
				XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)(xmlSchemaComplexContent.Content = new XmlSchemaComplexContentRestriction());
				xmlSchemaComplexContentRestriction.BaseTypeName = new XmlQualifiedName("Array", "http://schemas.xmlsoap.org/soap/encoding/");
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaComplexContentRestriction.Attributes.Add(xmlSchemaAttribute);
				xmlSchemaAttribute.RefName = new XmlQualifiedName("arrayType", "http://schemas.xmlsoap.org/soap/encoding/");
				XmlAttribute xmlAttribute = Document.CreateAttribute("arrayType", "http://schemas.xmlsoap.org/wsdl/");
				xmlAttribute.Value = ns + ((!(ns != string.Empty)) ? string.Empty : ":") + localName;
				xmlSchemaAttribute.UnhandledAttributes = new XmlAttribute[1] { xmlAttribute };
				ImportNamespace(schema, "http://schemas.xmlsoap.org/wsdl/");
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)listMap.ItemInfo[0];
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					switch (xmlTypeMapElementInfo.TypeData.SchemaType)
					{
					case SchemaTypes.Enum:
						ExportEnumSchema(xmlTypeMapElementInfo.MappedType);
						break;
					case SchemaTypes.Array:
						ExportArraySchema(xmlTypeMapElementInfo.MappedType, text);
						break;
					case SchemaTypes.Class:
						ExportClassSchema(xmlTypeMapElementInfo.MappedType);
						break;
					}
				}
				return new XmlQualifiedName(listMap.GetSchemaArrayName(), text);
			}
			if (IsMapExported(map))
			{
				return new XmlQualifiedName(map.XmlType, map.XmlTypeNamespace);
			}
			SetMapExported(map);
			XmlSchema schema2 = GetSchema(map.XmlTypeNamespace);
			XmlSchemaComplexType xmlSchemaComplexType2 = new XmlSchemaComplexType();
			xmlSchemaComplexType2.Name = map.ElementName;
			schema2.Items.Add(xmlSchemaComplexType2);
			XmlSchemaParticle schemaArrayElement = GetSchemaArrayElement(schema2, listMap.ItemInfo);
			if (schemaArrayElement is XmlSchemaChoice)
			{
				xmlSchemaComplexType2.Particle = schemaArrayElement;
			}
			else
			{
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				xmlSchemaSequence.Items.Add(schemaArrayElement);
				xmlSchemaComplexType2.Particle = xmlSchemaSequence;
			}
			return new XmlQualifiedName(map.XmlType, map.XmlTypeNamespace);
		}

		private bool IsMapExported(XmlTypeMapping map)
		{
			if (exportedMaps.ContainsKey(GetMapKey(map)))
			{
				return true;
			}
			return false;
		}

		private void SetMapExported(XmlTypeMapping map)
		{
			exportedMaps[GetMapKey(map)] = map;
		}

		private bool IsElementExported(XmlTypeMapping map)
		{
			if (exportedElements.ContainsKey(GetMapKey(map)))
			{
				return true;
			}
			if (map.TypeData.Type == typeof(object))
			{
				return true;
			}
			return false;
		}

		private void SetElementExported(XmlTypeMapping map)
		{
			exportedElements[GetMapKey(map)] = map;
		}

		private string GetMapKey(XmlTypeMapping map)
		{
			if (map.TypeData.IsListType)
			{
				return GetArrayKeyName(map.TypeData) + " " + map.XmlType + " " + map.XmlTypeNamespace;
			}
			return map.TypeData.FullTypeName + " " + map.XmlType + " " + map.XmlTypeNamespace;
		}

		private string GetArrayKeyName(TypeData td)
		{
			TypeData listItemTypeData = td.ListItemTypeData;
			return "*arrayof*" + ((!listItemTypeData.IsListType) ? listItemTypeData.FullTypeName : GetArrayKeyName(listItemTypeData));
		}

		private void CompileSchemas()
		{
		}

		private XmlSchema GetSchema(string ns)
		{
			XmlSchema xmlSchema = schemas[ns];
			if (xmlSchema == null)
			{
				xmlSchema = new XmlSchema();
				if (ns != null && ns.Length > 0)
				{
					xmlSchema.TargetNamespace = ns;
				}
				if (!encodedFormat)
				{
					xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
				}
				schemas.Add(xmlSchema);
			}
			return xmlSchema;
		}
	}
}
