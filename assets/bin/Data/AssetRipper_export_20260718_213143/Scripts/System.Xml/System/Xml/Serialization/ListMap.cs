namespace System.Xml.Serialization
{
	internal class ListMap : ObjectMap
	{
		private XmlTypeMapElementInfoList _itemInfo;

		private bool _gotNestedMapping;

		private XmlTypeMapping _nestedArrayMapping;

		private string _choiceMember;

		public bool IsMultiArray
		{
			get
			{
				return NestedArrayMapping != null;
			}
		}

		public string ChoiceMember
		{
			get
			{
				return _choiceMember;
			}
			set
			{
				_choiceMember = value;
			}
		}

		public XmlTypeMapping NestedArrayMapping
		{
			get
			{
				if (_gotNestedMapping)
				{
					return _nestedArrayMapping;
				}
				_gotNestedMapping = true;
				_nestedArrayMapping = ((XmlTypeMapElementInfo)_itemInfo[0]).MappedType;
				if (_nestedArrayMapping == null)
				{
					return null;
				}
				if (_nestedArrayMapping.TypeData.SchemaType != SchemaTypes.Array)
				{
					_nestedArrayMapping = null;
					return null;
				}
				foreach (XmlTypeMapElementInfo item in _itemInfo)
				{
					if (item.MappedType != _nestedArrayMapping)
					{
						_nestedArrayMapping = null;
						return null;
					}
				}
				return _nestedArrayMapping;
			}
		}

		public XmlTypeMapElementInfoList ItemInfo
		{
			get
			{
				return _itemInfo;
			}
			set
			{
				_itemInfo = value;
			}
		}

		public XmlTypeMapElementInfo FindElement(object ob, int index, object memberValue)
		{
			if (_itemInfo.Count == 1)
			{
				return (XmlTypeMapElementInfo)_itemInfo[0];
			}
			if (_choiceMember != null && index != -1)
			{
				Array array = (Array)XmlTypeMapMember.GetValue(ob, _choiceMember);
				if (array == null || index >= array.Length)
				{
					throw new InvalidOperationException("Invalid or missing choice enum value in member '" + _choiceMember + "'.");
				}
				object value = array.GetValue(index);
				foreach (XmlTypeMapElementInfo item in _itemInfo)
				{
					if (item.ChoiceValue != null && item.ChoiceValue.Equals(value))
					{
						return item;
					}
				}
			}
			else
			{
				if (memberValue == null)
				{
					return null;
				}
				Type type = memberValue.GetType();
				foreach (XmlTypeMapElementInfo item2 in _itemInfo)
				{
					if (item2.TypeData.Type == type)
					{
						return item2;
					}
				}
			}
			return null;
		}

		public XmlTypeMapElementInfo FindElement(string elementName, string ns)
		{
			foreach (XmlTypeMapElementInfo item in _itemInfo)
			{
				if (item.ElementName == elementName && item.Namespace == ns)
				{
					return item;
				}
			}
			return null;
		}

		public XmlTypeMapElementInfo FindTextElement()
		{
			foreach (XmlTypeMapElementInfo item in _itemInfo)
			{
				if (item.IsTextElement)
				{
					return item;
				}
			}
			return null;
		}

		public string GetSchemaArrayName()
		{
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)_itemInfo[0];
			if (xmlTypeMapElementInfo.MappedType != null)
			{
				return TypeTranslator.GetArrayName(xmlTypeMapElementInfo.MappedType.XmlType);
			}
			return TypeTranslator.GetArrayName(xmlTypeMapElementInfo.TypeData.XmlType);
		}

		public void GetArrayType(int itemCount, out string localName, out string ns)
		{
			string text = ((itemCount == -1) ? "[]" : ("[" + itemCount + "]"));
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)_itemInfo[0];
			if (xmlTypeMapElementInfo.TypeData.SchemaType == SchemaTypes.Array)
			{
				string localName2;
				((ListMap)xmlTypeMapElementInfo.MappedType.ObjectMap).GetArrayType(-1, out localName2, out ns);
				localName = localName2 + text;
			}
			else if (xmlTypeMapElementInfo.MappedType != null)
			{
				localName = xmlTypeMapElementInfo.MappedType.XmlType + text;
				ns = xmlTypeMapElementInfo.MappedType.Namespace;
			}
			else
			{
				localName = xmlTypeMapElementInfo.TypeData.XmlType + text;
				ns = xmlTypeMapElementInfo.DataTypeNamespace;
			}
		}

		public override bool Equals(object other)
		{
			ListMap listMap = other as ListMap;
			if (listMap == null)
			{
				return false;
			}
			if (_itemInfo.Count != listMap._itemInfo.Count)
			{
				return false;
			}
			for (int i = 0; i < _itemInfo.Count; i++)
			{
				if (!_itemInfo[i].Equals(listMap._itemInfo[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
