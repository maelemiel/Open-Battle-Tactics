namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberElement : XmlTypeMapMember
	{
		private XmlTypeMapElementInfoList _elementInfo;

		private string _choiceMember;

		private bool _isTextCollector;

		private TypeData _choiceTypeData;

		public XmlTypeMapElementInfoList ElementInfo
		{
			get
			{
				if (_elementInfo == null)
				{
					_elementInfo = new XmlTypeMapElementInfoList();
				}
				return _elementInfo;
			}
			set
			{
				_elementInfo = value;
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

		public TypeData ChoiceTypeData
		{
			get
			{
				return _choiceTypeData;
			}
			set
			{
				_choiceTypeData = value;
			}
		}

		public bool IsXmlTextCollector
		{
			get
			{
				return _isTextCollector;
			}
			set
			{
				_isTextCollector = value;
			}
		}

		public override bool RequiresNullable
		{
			get
			{
				foreach (XmlTypeMapElementInfo item in ElementInfo)
				{
					if (item.IsNullable)
					{
						return true;
					}
				}
				return false;
			}
		}

		public XmlTypeMapElementInfo FindElement(object ob, object memberValue)
		{
			if (_elementInfo.Count == 1)
			{
				return (XmlTypeMapElementInfo)_elementInfo[0];
			}
			if (_choiceMember != null)
			{
				object value = XmlTypeMapMember.GetValue(ob, _choiceMember);
				foreach (XmlTypeMapElementInfo item in _elementInfo)
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
					return (XmlTypeMapElementInfo)_elementInfo[0];
				}
				foreach (XmlTypeMapElementInfo item2 in _elementInfo)
				{
					if (item2.TypeData.Type.IsInstanceOfType(memberValue))
					{
						return item2;
					}
				}
			}
			return null;
		}

		public void SetChoice(object ob, object choice)
		{
			XmlTypeMapMember.SetValue(ob, _choiceMember, choice);
		}
	}
}
