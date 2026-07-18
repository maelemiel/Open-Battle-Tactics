namespace System.Xml.Serialization
{
	public class XmlMembersMapping : XmlMapping
	{
		private bool _hasWrapperElement;

		private XmlMemberMapping[] _mapping;

		public int Count
		{
			get
			{
				return _mapping.Length;
			}
		}

		public XmlMemberMapping this[int index]
		{
			get
			{
				return _mapping[index];
			}
		}

		public string TypeName
		{
			[System.MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public string TypeNamespace
		{
			[System.MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		internal bool HasWrapperElement
		{
			get
			{
				return _hasWrapperElement;
			}
		}

		internal XmlMembersMapping()
		{
		}

		internal XmlMembersMapping(XmlMemberMapping[] mapping)
			: this(string.Empty, null, false, false, mapping)
		{
		}

		internal XmlMembersMapping(string elementName, string ns, XmlMemberMapping[] mapping)
			: this(elementName, ns, true, false, mapping)
		{
		}

		internal XmlMembersMapping(string elementName, string ns, bool hasWrapperElement, bool writeAccessors, XmlMemberMapping[] mapping)
			: base(elementName, ns)
		{
			_hasWrapperElement = hasWrapperElement;
			_mapping = mapping;
			ClassMap classMap = new ClassMap
			{
				IgnoreMemberNamespace = writeAccessors
			};
			foreach (XmlMemberMapping xmlMemberMapping in mapping)
			{
				classMap.AddMember(xmlMemberMapping.TypeMapMember);
			}
			base.ObjectMap = classMap;
		}
	}
}
