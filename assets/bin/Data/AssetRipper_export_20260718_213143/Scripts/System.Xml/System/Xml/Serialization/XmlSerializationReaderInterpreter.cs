using System.Collections;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class XmlSerializationReaderInterpreter : XmlSerializationReader
	{
		private class FixupCallbackInfo
		{
			private XmlSerializationReaderInterpreter _sri;

			private ClassMap _map;

			private bool _isValueList;

			public FixupCallbackInfo(XmlSerializationReaderInterpreter sri, ClassMap map, bool isValueList)
			{
				_sri = sri;
				_map = map;
				_isValueList = isValueList;
			}

			public void FixupMembers(object fixup)
			{
				_sri.FixupMembers(_map, fixup, _isValueList);
			}
		}

		private class ReaderCallbackInfo
		{
			private XmlSerializationReaderInterpreter _sri;

			private XmlTypeMapping _typeMap;

			public ReaderCallbackInfo(XmlSerializationReaderInterpreter sri, XmlTypeMapping typeMap)
			{
				_sri = sri;
				_typeMap = typeMap;
			}

			internal object ReadObject()
			{
				return _sri.ReadObject(_typeMap, true, true);
			}
		}

		private XmlMapping _typeMap;

		private SerializationFormat _format;

		private static readonly XmlQualifiedName AnyType = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");

		private static readonly object[] empty_array = new object[0];

		public XmlSerializationReaderInterpreter(XmlMapping typeMap)
		{
			_typeMap = typeMap;
			_format = typeMap.Format;
		}

		protected override void InitCallbacks()
		{
			ArrayList relatedMaps = _typeMap.RelatedMaps;
			if (relatedMaps == null)
			{
				return;
			}
			foreach (XmlTypeMapping item in relatedMaps)
			{
				if (item.TypeData.SchemaType == SchemaTypes.Class || item.TypeData.SchemaType == SchemaTypes.Enum)
				{
					ReaderCallbackInfo readerCallbackInfo = new ReaderCallbackInfo(this, item);
					AddReadCallback(item.XmlType, item.Namespace, item.TypeData.Type, readerCallbackInfo.ReadObject);
				}
			}
		}

		protected override void InitIDs()
		{
		}

		protected XmlTypeMapping GetTypeMap(Type type)
		{
			ArrayList relatedMaps = _typeMap.RelatedMaps;
			if (relatedMaps != null)
			{
				foreach (XmlTypeMapping item in relatedMaps)
				{
					if (item.TypeData.Type == type)
					{
						return item;
					}
				}
			}
			throw new InvalidOperationException(string.Concat("Type ", type, " not mapped"));
		}

		public object ReadRoot()
		{
			base.Reader.MoveToContent();
			if (_typeMap is XmlTypeMapping)
			{
				if (_format == SerializationFormat.Literal)
				{
					return ReadRoot((XmlTypeMapping)_typeMap);
				}
				return ReadEncodedObject((XmlTypeMapping)_typeMap);
			}
			return ReadMessage((XmlMembersMapping)_typeMap);
		}

		private object ReadEncodedObject(XmlTypeMapping typeMap)
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (!(base.Reader.LocalName == typeMap.ElementName) || !(base.Reader.NamespaceURI == typeMap.Namespace))
				{
					throw CreateUnknownNodeException();
				}
				result = ReadReferencedElement();
			}
			else
			{
				UnknownNode(null);
			}
			ReadReferencedElements();
			return result;
		}

		protected virtual object ReadMessage(XmlMembersMapping typeMap)
		{
			object[] array = new object[typeMap.Count];
			if (typeMap.HasWrapperElement)
			{
				ArrayList allMembers = ((ClassMap)typeMap.ObjectMap).AllMembers;
				for (int i = 0; i < allMembers.Count; i++)
				{
					XmlTypeMapMember xmlTypeMapMember = (XmlTypeMapMember)allMembers[i];
					if (!xmlTypeMapMember.IsReturnValue && xmlTypeMapMember.TypeData.IsValueType)
					{
						SetMemberValueFromAttr(xmlTypeMapMember, array, CreateInstance(xmlTypeMapMember.TypeData.Type), true);
					}
				}
				if (_format == SerializationFormat.Encoded)
				{
					while (base.Reader.NodeType == XmlNodeType.Element)
					{
						string attribute = base.Reader.GetAttribute("root", "http://schemas.xmlsoap.org/soap/encoding/");
						if (attribute == null || XmlConvert.ToBoolean(attribute))
						{
							break;
						}
						ReadReferencedElement();
						base.Reader.MoveToContent();
					}
				}
				while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.ReadState == ReadState.Interactive)
				{
					if (base.Reader.IsStartElement(typeMap.ElementName, typeMap.Namespace) || _format == SerializationFormat.Encoded)
					{
						ReadAttributeMembers((ClassMap)typeMap.ObjectMap, array, true);
						if (!base.Reader.IsEmptyElement)
						{
							base.Reader.ReadStartElement();
							ReadMembers((ClassMap)typeMap.ObjectMap, array, true, false);
							ReadEndElement();
							break;
						}
						base.Reader.Skip();
						base.Reader.MoveToContent();
					}
					else
					{
						UnknownNode(null);
						base.Reader.MoveToContent();
					}
				}
			}
			else
			{
				ReadMembers((ClassMap)typeMap.ObjectMap, array, true, _format == SerializationFormat.Encoded);
			}
			if (_format == SerializationFormat.Encoded)
			{
				ReadReferencedElements();
			}
			return array;
		}

		private object ReadRoot(XmlTypeMapping rootMap)
		{
			if (rootMap.TypeData.SchemaType == SchemaTypes.XmlNode)
			{
				return ReadXmlNodeElement(rootMap, true);
			}
			if (base.Reader.LocalName != rootMap.ElementName || base.Reader.NamespaceURI != rootMap.Namespace)
			{
				throw CreateUnknownNodeException();
			}
			return ReadObject(rootMap, rootMap.IsNullable, true);
		}

		protected virtual object ReadObject(XmlTypeMapping typeMap, bool isNullable, bool checkType)
		{
			switch (typeMap.TypeData.SchemaType)
			{
			case SchemaTypes.Class:
				return ReadClassInstance(typeMap, isNullable, checkType);
			case SchemaTypes.Array:
				return ReadListElement(typeMap, isNullable, null, true);
			case SchemaTypes.XmlNode:
				return ReadXmlNodeElement(typeMap, isNullable);
			case SchemaTypes.Primitive:
				return ReadPrimitiveElement(typeMap, isNullable);
			case SchemaTypes.Enum:
				return ReadEnumElement(typeMap, isNullable);
			case SchemaTypes.XmlSerializable:
				return ReadXmlSerializableElement(typeMap, isNullable);
			default:
				throw new Exception("Unsupported map type");
			}
		}

		protected virtual object ReadClassInstance(XmlTypeMapping typeMap, bool isNullable, bool checkType)
		{
			if (isNullable && ReadNull())
			{
				return null;
			}
			if (checkType)
			{
				XmlQualifiedName xsiType = GetXsiType();
				if (xsiType != null)
				{
					XmlTypeMapping realElementMap = typeMap.GetRealElementMap(xsiType.Name, xsiType.Namespace);
					if (realElementMap == null)
					{
						if (typeMap.TypeData.Type == typeof(object))
						{
							return ReadTypedPrimitive(xsiType);
						}
						throw CreateUnknownTypeException(xsiType);
					}
					if (realElementMap != typeMap)
					{
						return ReadObject(realElementMap, false, false);
					}
				}
				else if (typeMap.TypeData.Type == typeof(object))
				{
					return ReadTypedPrimitive(AnyType);
				}
			}
			object obj = Activator.CreateInstance(typeMap.TypeData.Type, true);
			base.Reader.MoveToElement();
			bool isEmptyElement = base.Reader.IsEmptyElement;
			ReadClassInstanceMembers(typeMap, obj);
			if (isEmptyElement)
			{
				base.Reader.Skip();
			}
			else
			{
				ReadEndElement();
			}
			return obj;
		}

		protected virtual void ReadClassInstanceMembers(XmlTypeMapping typeMap, object ob)
		{
			ReadMembers((ClassMap)typeMap.ObjectMap, ob, false, false);
		}

		private void ReadAttributeMembers(ClassMap map, object ob, bool isValueList)
		{
			XmlTypeMapMember defaultAnyAttributeMember = map.DefaultAnyAttributeMember;
			int length = 0;
			object list = null;
			while (base.Reader.MoveToNextAttribute())
			{
				XmlTypeMapMemberAttribute attribute = map.GetAttribute(base.Reader.LocalName, base.Reader.NamespaceURI);
				if (attribute != null)
				{
					SetMemberValue(attribute, ob, GetValueFromXmlString(base.Reader.Value, attribute.TypeData, attribute.MappedType), isValueList);
				}
				else if (IsXmlnsAttribute(base.Reader.Name))
				{
					if (map.NamespaceDeclarations != null)
					{
						XmlSerializerNamespaces xmlSerializerNamespaces = GetMemberValue(map.NamespaceDeclarations, ob, isValueList) as XmlSerializerNamespaces;
						if (xmlSerializerNamespaces == null)
						{
							xmlSerializerNamespaces = new XmlSerializerNamespaces();
							SetMemberValue(map.NamespaceDeclarations, ob, xmlSerializerNamespaces, isValueList);
						}
						if (base.Reader.Prefix == "xmlns")
						{
							xmlSerializerNamespaces.Add(base.Reader.LocalName, base.Reader.Value);
						}
						else
						{
							xmlSerializerNamespaces.Add(string.Empty, base.Reader.Value);
						}
					}
				}
				else if (defaultAnyAttributeMember != null)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)base.Document.ReadNode(base.Reader);
					ParseWsdlArrayType(xmlAttribute);
					AddListValue(defaultAnyAttributeMember.TypeData, ref list, length++, xmlAttribute, true);
				}
				else
				{
					ProcessUnknownAttribute(ob);
				}
			}
			if (defaultAnyAttributeMember != null)
			{
				list = ShrinkArray((Array)list, length, defaultAnyAttributeMember.TypeData.Type.GetElementType(), true);
				SetMemberValue(defaultAnyAttributeMember, ob, list, isValueList);
			}
			base.Reader.MoveToElement();
		}

		private void ReadMembers(ClassMap map, object ob, bool isValueList, bool readByOrder)
		{
			ReadAttributeMembers(map, ob, isValueList);
			if (!isValueList)
			{
				base.Reader.MoveToElement();
				if (base.Reader.IsEmptyElement)
				{
					SetListMembersDefaults(map, ob, isValueList);
					return;
				}
				base.Reader.ReadStartElement();
			}
			bool[] array = new bool[(map.ElementMembers != null) ? map.ElementMembers.Count : 0];
			bool flag = isValueList && _format == SerializationFormat.Encoded && map.ReturnMember != null;
			base.Reader.MoveToContent();
			int[] array2 = null;
			object[] array3 = null;
			object[] array4 = null;
			Fixup fixup = null;
			int num = 0;
			int num2 = ((!readByOrder) ? int.MaxValue : ((map.ElementMembers != null) ? map.ElementMembers.Count : 0));
			if (map.FlatLists != null)
			{
				array2 = new int[map.FlatLists.Count];
				array3 = new object[map.FlatLists.Count];
				foreach (XmlTypeMapMemberExpandable flatList in map.FlatLists)
				{
					if (IsReadOnly(flatList, flatList.TypeData, ob, isValueList))
					{
						array3[flatList.FlatArrayIndex] = flatList.GetValue(ob);
					}
					else if (flatList.TypeData.Type.IsArray)
					{
						array3[flatList.FlatArrayIndex] = InitializeList(flatList.TypeData);
					}
					else
					{
						object obj = flatList.GetValue(ob);
						if (obj == null)
						{
							obj = InitializeList(flatList.TypeData);
							SetMemberValue(flatList, ob, obj, isValueList);
						}
						array3[flatList.FlatArrayIndex] = obj;
					}
					if (flatList.ChoiceMember != null)
					{
						if (array4 == null)
						{
							array4 = new object[map.FlatLists.Count];
						}
						array4[flatList.FlatArrayIndex] = InitializeList(flatList.ChoiceTypeData);
					}
				}
			}
			if (_format == SerializationFormat.Encoded && map.ElementMembers != null)
			{
				FixupCallbackInfo fixupCallbackInfo = new FixupCallbackInfo(this, map, isValueList);
				fixup = new Fixup(ob, fixupCallbackInfo.FixupMembers, map.ElementMembers.Count);
				AddFixup(fixup);
			}
			while (base.Reader.NodeType != XmlNodeType.EndElement && num < num2)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo;
					if (readByOrder)
					{
						xmlTypeMapElementInfo = map.GetElement(num++);
					}
					else if (flag)
					{
						xmlTypeMapElementInfo = (XmlTypeMapElementInfo)((XmlTypeMapMemberElement)map.ReturnMember).ElementInfo[0];
						flag = false;
					}
					else
					{
						xmlTypeMapElementInfo = map.GetElement(base.Reader.LocalName, base.Reader.NamespaceURI);
					}
					if (xmlTypeMapElementInfo != null && !array[xmlTypeMapElementInfo.Member.Index])
					{
						if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberList))
						{
							if (_format == SerializationFormat.Encoded && xmlTypeMapElementInfo.MultiReferenceType)
							{
								object value = ReadReferencingElement(out fixup.Ids[xmlTypeMapElementInfo.Member.Index]);
								if (fixup.Ids[xmlTypeMapElementInfo.Member.Index] == null)
								{
									if (IsReadOnly(xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, ob, isValueList))
									{
										throw CreateReadOnlyCollectionException(xmlTypeMapElementInfo.TypeData.FullTypeName);
									}
									SetMemberValue(xmlTypeMapElementInfo.Member, ob, value, isValueList);
								}
								else if (!xmlTypeMapElementInfo.MappedType.TypeData.Type.IsArray)
								{
									if (IsReadOnly(xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, ob, isValueList))
									{
										value = GetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList);
									}
									else
									{
										value = CreateList(xmlTypeMapElementInfo.MappedType.TypeData.Type);
										SetMemberValue(xmlTypeMapElementInfo.Member, ob, value, isValueList);
									}
									AddFixup(new CollectionFixup(value, FillList, fixup.Ids[xmlTypeMapElementInfo.Member.Index]));
									fixup.Ids[xmlTypeMapElementInfo.Member.Index] = null;
								}
							}
							else if (IsReadOnly(xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, ob, isValueList))
							{
								ReadListElement(xmlTypeMapElementInfo.MappedType, xmlTypeMapElementInfo.IsNullable, GetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList), false);
							}
							else if (xmlTypeMapElementInfo.MappedType.TypeData.Type.IsArray)
							{
								object obj2 = ReadListElement(xmlTypeMapElementInfo.MappedType, xmlTypeMapElementInfo.IsNullable, null, true);
								if (obj2 != null || xmlTypeMapElementInfo.IsNullable)
								{
									SetMemberValue(xmlTypeMapElementInfo.Member, ob, obj2, isValueList);
								}
							}
							else
							{
								object obj3 = GetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList);
								if (obj3 == null)
								{
									obj3 = CreateList(xmlTypeMapElementInfo.MappedType.TypeData.Type);
									SetMemberValue(xmlTypeMapElementInfo.Member, ob, obj3, isValueList);
								}
								ReadListElement(xmlTypeMapElementInfo.MappedType, xmlTypeMapElementInfo.IsNullable, obj3, true);
							}
							array[xmlTypeMapElementInfo.Member.Index] = true;
						}
						else if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberFlatList))
						{
							XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = (XmlTypeMapMemberFlatList)xmlTypeMapElementInfo.Member;
							AddListValue(xmlTypeMapMemberFlatList.TypeData, ref array3[xmlTypeMapMemberFlatList.FlatArrayIndex], array2[xmlTypeMapMemberFlatList.FlatArrayIndex]++, ReadObjectElement(xmlTypeMapElementInfo), !IsReadOnly(xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, ob, isValueList));
							if (xmlTypeMapMemberFlatList.ChoiceMember != null)
							{
								AddListValue(xmlTypeMapMemberFlatList.ChoiceTypeData, ref array4[xmlTypeMapMemberFlatList.FlatArrayIndex], array2[xmlTypeMapMemberFlatList.FlatArrayIndex] - 1, xmlTypeMapElementInfo.ChoiceValue, true);
							}
						}
						else if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberAnyElement))
						{
							XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = (XmlTypeMapMemberAnyElement)xmlTypeMapElementInfo.Member;
							if (xmlTypeMapMemberAnyElement.TypeData.IsListType)
							{
								AddListValue(xmlTypeMapMemberAnyElement.TypeData, ref array3[xmlTypeMapMemberAnyElement.FlatArrayIndex], array2[xmlTypeMapMemberAnyElement.FlatArrayIndex]++, ReadXmlNode(xmlTypeMapMemberAnyElement.TypeData.ListItemTypeData, false), true);
							}
							else
							{
								SetMemberValue(xmlTypeMapMemberAnyElement, ob, ReadXmlNode(xmlTypeMapMemberAnyElement.TypeData, false), isValueList);
							}
						}
						else
						{
							if (xmlTypeMapElementInfo.Member.GetType() != typeof(XmlTypeMapMemberElement))
							{
								throw new InvalidOperationException("Unknown member type");
							}
							array[xmlTypeMapElementInfo.Member.Index] = true;
							if (_format == SerializationFormat.Encoded)
							{
								object obj4 = ((xmlTypeMapElementInfo.Member.TypeData.SchemaType == SchemaTypes.Primitive) ? ReadReferencingElement(xmlTypeMapElementInfo.Member.TypeData.XmlType, "http://www.w3.org/2001/XMLSchema", out fixup.Ids[xmlTypeMapElementInfo.Member.Index]) : ReadReferencingElement(out fixup.Ids[xmlTypeMapElementInfo.Member.Index]));
								if (xmlTypeMapElementInfo.MultiReferenceType)
								{
									if (fixup.Ids[xmlTypeMapElementInfo.Member.Index] == null)
									{
										SetMemberValue(xmlTypeMapElementInfo.Member, ob, obj4, isValueList);
									}
								}
								else if (obj4 != null)
								{
									SetMemberValue(xmlTypeMapElementInfo.Member, ob, obj4, isValueList);
								}
							}
							else
							{
								SetMemberValue(xmlTypeMapElementInfo.Member, ob, ReadObjectElement(xmlTypeMapElementInfo), isValueList);
								if (xmlTypeMapElementInfo.ChoiceValue != null)
								{
									XmlTypeMapMemberElement xmlTypeMapMemberElement = (XmlTypeMapMemberElement)xmlTypeMapElementInfo.Member;
									xmlTypeMapMemberElement.SetChoice(ob, xmlTypeMapElementInfo.ChoiceValue);
								}
							}
						}
					}
					else if (map.DefaultAnyElementMember != null)
					{
						XmlTypeMapMemberAnyElement defaultAnyElementMember = map.DefaultAnyElementMember;
						if (defaultAnyElementMember.TypeData.IsListType)
						{
							AddListValue(defaultAnyElementMember.TypeData, ref array3[defaultAnyElementMember.FlatArrayIndex], array2[defaultAnyElementMember.FlatArrayIndex]++, ReadXmlNode(defaultAnyElementMember.TypeData.ListItemTypeData, false), true);
						}
						else
						{
							SetMemberValue(defaultAnyElementMember, ob, ReadXmlNode(defaultAnyElementMember.TypeData, false), isValueList);
						}
					}
					else
					{
						ProcessUnknownElement(ob);
					}
				}
				else if ((base.Reader.NodeType == XmlNodeType.Text || base.Reader.NodeType == XmlNodeType.CDATA) && map.XmlTextCollector != null)
				{
					if (map.XmlTextCollector is XmlTypeMapMemberExpandable)
					{
						XmlTypeMapMemberExpandable xmlTypeMapMemberExpandable2 = (XmlTypeMapMemberExpandable)map.XmlTextCollector;
						XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList2 = xmlTypeMapMemberExpandable2 as XmlTypeMapMemberFlatList;
						TypeData typeData = ((xmlTypeMapMemberFlatList2 != null) ? xmlTypeMapMemberFlatList2.ListMap.FindTextElement().TypeData : xmlTypeMapMemberExpandable2.TypeData.ListItemTypeData);
						object value2 = ((typeData.Type != typeof(string)) ? ReadXmlNode(typeData, false) : base.Reader.ReadString());
						AddListValue(xmlTypeMapMemberExpandable2.TypeData, ref array3[xmlTypeMapMemberExpandable2.FlatArrayIndex], array2[xmlTypeMapMemberExpandable2.FlatArrayIndex]++, value2, true);
					}
					else
					{
						XmlTypeMapMemberElement xmlTypeMapMemberElement2 = (XmlTypeMapMemberElement)map.XmlTextCollector;
						XmlTypeMapElementInfo xmlTypeMapElementInfo2 = (XmlTypeMapElementInfo)xmlTypeMapMemberElement2.ElementInfo[0];
						if (xmlTypeMapElementInfo2.TypeData.Type == typeof(string))
						{
							SetMemberValue(xmlTypeMapMemberElement2, ob, ReadString((string)GetMemberValue(xmlTypeMapMemberElement2, ob, isValueList)), isValueList);
						}
						else
						{
							SetMemberValue(xmlTypeMapMemberElement2, ob, GetValueFromXmlString(base.Reader.ReadString(), xmlTypeMapElementInfo2.TypeData, xmlTypeMapElementInfo2.MappedType), isValueList);
						}
					}
				}
				else
				{
					UnknownNode(ob);
				}
				base.Reader.MoveToContent();
			}
			if (array3 != null)
			{
				foreach (XmlTypeMapMemberExpandable flatList2 in map.FlatLists)
				{
					object obj5 = array3[flatList2.FlatArrayIndex];
					if (flatList2.TypeData.Type.IsArray)
					{
						obj5 = ShrinkArray((Array)obj5, array2[flatList2.FlatArrayIndex], flatList2.TypeData.Type.GetElementType(), true);
					}
					if (!IsReadOnly(flatList2, flatList2.TypeData, ob, isValueList) && flatList2.TypeData.Type.IsArray)
					{
						SetMemberValue(flatList2, ob, obj5, isValueList);
					}
				}
			}
			if (array4 != null)
			{
				foreach (XmlTypeMapMemberExpandable flatList3 in map.FlatLists)
				{
					object obj6 = array4[flatList3.FlatArrayIndex];
					if (obj6 != null)
					{
						obj6 = ShrinkArray((Array)obj6, array2[flatList3.FlatArrayIndex], flatList3.ChoiceTypeData.Type.GetElementType(), true);
						XmlTypeMapMember.SetValue(ob, flatList3.ChoiceMember, obj6);
					}
				}
			}
			SetListMembersDefaults(map, ob, isValueList);
		}

		private void SetListMembersDefaults(ClassMap map, object ob, bool isValueList)
		{
			if (map.ListMembers == null)
			{
				return;
			}
			ArrayList listMembers = map.ListMembers;
			for (int i = 0; i < listMembers.Count; i++)
			{
				XmlTypeMapMember xmlTypeMapMember = (XmlTypeMapMember)listMembers[i];
				if (!IsReadOnly(xmlTypeMapMember, xmlTypeMapMember.TypeData, ob, isValueList) && GetMemberValue(xmlTypeMapMember, ob, isValueList) == null)
				{
					SetMemberValue(xmlTypeMapMember, ob, InitializeList(xmlTypeMapMember.TypeData), isValueList);
				}
			}
		}

		internal void FixupMembers(ClassMap map, object obfixup, bool isValueList)
		{
			Fixup fixup = (Fixup)obfixup;
			ICollection elementMembers = map.ElementMembers;
			string[] ids = fixup.Ids;
			foreach (XmlTypeMapMember item in elementMembers)
			{
				if (ids[item.Index] != null)
				{
					SetMemberValue(item, fixup.Source, GetTarget(ids[item.Index]), isValueList);
				}
			}
		}

		protected virtual void ProcessUnknownAttribute(object target)
		{
			UnknownNode(target);
		}

		protected virtual void ProcessUnknownElement(object target)
		{
			UnknownNode(target);
		}

		private bool IsReadOnly(XmlTypeMapMember member, TypeData memType, object ob, bool isValueList)
		{
			if (isValueList)
			{
				return !memType.HasPublicConstructor;
			}
			return member.IsReadOnly(ob.GetType()) || !memType.HasPublicConstructor;
		}

		private void SetMemberValue(XmlTypeMapMember member, object ob, object value, bool isValueList)
		{
			if (isValueList)
			{
				((object[])ob)[member.GlobalIndex] = value;
				return;
			}
			member.SetValue(ob, value);
			if (member.IsOptionalValueType)
			{
				member.SetValueSpecified(ob, true);
			}
		}

		private void SetMemberValueFromAttr(XmlTypeMapMember member, object ob, object value, bool isValueList)
		{
			if (member.TypeData.Type.IsEnum)
			{
				value = Enum.ToObject(member.TypeData.Type, value);
			}
			SetMemberValue(member, ob, value, isValueList);
		}

		private object GetMemberValue(XmlTypeMapMember member, object ob, bool isValueList)
		{
			if (isValueList)
			{
				return ((object[])ob)[member.GlobalIndex];
			}
			return member.GetValue(ob);
		}

		private object ReadObjectElement(XmlTypeMapElementInfo elem)
		{
			switch (elem.TypeData.SchemaType)
			{
			case SchemaTypes.XmlNode:
				return ReadXmlNode(elem.TypeData, true);
			case SchemaTypes.Primitive:
			case SchemaTypes.Enum:
				return ReadPrimitiveValue(elem);
			case SchemaTypes.Array:
				return ReadListElement(elem.MappedType, elem.IsNullable, null, true);
			case SchemaTypes.Class:
				return ReadObject(elem.MappedType, elem.IsNullable, true);
			case SchemaTypes.XmlSerializable:
			{
				object obj = Activator.CreateInstance(elem.TypeData.Type, true);
				return ReadSerializable((IXmlSerializable)obj);
			}
			default:
				throw new NotSupportedException("Invalid value type");
			}
		}

		private object ReadPrimitiveValue(XmlTypeMapElementInfo elem)
		{
			if (elem.TypeData.Type == typeof(XmlQualifiedName))
			{
				if (elem.IsNullable)
				{
					return ReadNullableQualifiedName();
				}
				return ReadElementQualifiedName();
			}
			if (elem.IsNullable)
			{
				return GetValueFromXmlString(ReadNullableString(), elem.TypeData, elem.MappedType);
			}
			return GetValueFromXmlString(base.Reader.ReadElementString(), elem.TypeData, elem.MappedType);
		}

		private object GetValueFromXmlString(string value, TypeData typeData, XmlTypeMapping typeMap)
		{
			if (typeData.SchemaType == SchemaTypes.Array)
			{
				return ReadListString(typeMap, value);
			}
			if (typeData.SchemaType == SchemaTypes.Enum)
			{
				return GetEnumValue(typeMap, value);
			}
			if (typeData.Type == typeof(XmlQualifiedName))
			{
				return ToXmlQualifiedName(value);
			}
			return XmlCustomFormatter.FromXmlString(typeData, value);
		}

		private object ReadListElement(XmlTypeMapping typeMap, bool isNullable, object list, bool canCreateInstance)
		{
			Type type = typeMap.TypeData.Type;
			ListMap listMap = (ListMap)typeMap.ObjectMap;
			if (type.IsArray && ReadNull())
			{
				return null;
			}
			if (list == null)
			{
				if (!canCreateInstance || !typeMap.TypeData.HasPublicConstructor)
				{
					throw CreateReadOnlyCollectionException(typeMap.TypeFullName);
				}
				list = CreateList(type);
			}
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				if (type.IsArray)
				{
					list = ShrinkArray((Array)list, 0, type.GetElementType(), false);
				}
				return list;
			}
			int length = 0;
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			while (base.Reader.NodeType != XmlNodeType.EndElement)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo = listMap.FindElement(base.Reader.LocalName, base.Reader.NamespaceURI);
					if (xmlTypeMapElementInfo != null)
					{
						AddListValue(typeMap.TypeData, ref list, length++, ReadObjectElement(xmlTypeMapElementInfo), false);
					}
					else
					{
						UnknownNode(null);
					}
				}
				else
				{
					UnknownNode(null);
				}
				base.Reader.MoveToContent();
			}
			ReadEndElement();
			if (type.IsArray)
			{
				list = ShrinkArray((Array)list, length, type.GetElementType(), false);
			}
			return list;
		}

		private object ReadListString(XmlTypeMapping typeMap, string values)
		{
			Type type = typeMap.TypeData.Type;
			ListMap listMap = (ListMap)typeMap.ObjectMap;
			values = values.Trim();
			if (values == string.Empty)
			{
				return Array.CreateInstance(type.GetElementType(), 0);
			}
			string[] array = values.Split(' ');
			Array array2 = Array.CreateInstance(type.GetElementType(), array.Length);
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)listMap.ItemInfo[0];
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(GetValueFromXmlString(array[i], xmlTypeMapElementInfo.TypeData, xmlTypeMapElementInfo.MappedType), i);
			}
			return array2;
		}

		private void AddListValue(TypeData listType, ref object list, int index, object value, bool canCreateInstance)
		{
			Type type = listType.Type;
			if (type.IsArray)
			{
				list = EnsureArrayIndex((Array)list, index, type.GetElementType());
				((Array)list).SetValue(value, index);
				return;
			}
			if (list == null)
			{
				if (!canCreateInstance)
				{
					throw CreateReadOnlyCollectionException(type.FullName);
				}
				list = Activator.CreateInstance(type, true);
			}
			MethodInfo method = type.GetMethod("Add", new Type[1] { listType.ListItemType });
			method.Invoke(list, new object[1] { value });
		}

		private object CreateInstance(Type type)
		{
			return Activator.CreateInstance(type, empty_array);
		}

		private object CreateList(Type listType)
		{
			if (listType.IsArray)
			{
				return EnsureArrayIndex(null, 0, listType.GetElementType());
			}
			return Activator.CreateInstance(listType, true);
		}

		private object InitializeList(TypeData listType)
		{
			if (listType.Type.IsArray)
			{
				return null;
			}
			return Activator.CreateInstance(listType.Type, true);
		}

		private void FillList(object list, object items)
		{
			CopyEnumerableList(items, list);
		}

		private void CopyEnumerableList(object source, object dest)
		{
			if (dest == null)
			{
				throw CreateReadOnlyCollectionException(source.GetType().FullName);
			}
			object[] array = new object[1];
			MethodInfo method = dest.GetType().GetMethod("Add");
			foreach (object item in (IEnumerable)source)
			{
				array[0] = item;
				method.Invoke(dest, array);
			}
		}

		private object ReadXmlNodeElement(XmlTypeMapping typeMap, bool isNullable)
		{
			return ReadXmlNode(typeMap.TypeData, false);
		}

		private object ReadXmlNode(TypeData type, bool wrapped)
		{
			if (type.Type == typeof(XmlDocument))
			{
				return ReadXmlDocument(wrapped);
			}
			return ReadXmlNode(wrapped);
		}

		private object ReadPrimitiveElement(XmlTypeMapping typeMap, bool isNullable)
		{
			XmlQualifiedName xmlQualifiedName = GetXsiType();
			if (xmlQualifiedName == null)
			{
				xmlQualifiedName = new XmlQualifiedName(typeMap.XmlType, typeMap.Namespace);
			}
			return ReadTypedPrimitive(xmlQualifiedName);
		}

		private object ReadEnumElement(XmlTypeMapping typeMap, bool isNullable)
		{
			base.Reader.ReadStartElement();
			object enumValue = GetEnumValue(typeMap, base.Reader.ReadString());
			ReadEndElement();
			return enumValue;
		}

		private object GetEnumValue(XmlTypeMapping typeMap, string val)
		{
			if (val == null)
			{
				return null;
			}
			EnumMap enumMap = (EnumMap)typeMap.ObjectMap;
			string enumName = enumMap.GetEnumName(typeMap.TypeFullName, val);
			if (enumName == null)
			{
				throw CreateUnknownConstantException(val, typeMap.TypeData.Type);
			}
			return Enum.Parse(typeMap.TypeData.Type, enumName);
		}

		private object ReadXmlSerializableElement(XmlTypeMapping typeMap, bool isNullable)
		{
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName == typeMap.ElementName && base.Reader.NamespaceURI == typeMap.Namespace)
				{
					object obj = Activator.CreateInstance(typeMap.TypeData.Type, true);
					return ReadSerializable((IXmlSerializable)obj);
				}
				throw CreateUnknownNodeException();
			}
			UnknownNode(null);
			return null;
		}
	}
}
