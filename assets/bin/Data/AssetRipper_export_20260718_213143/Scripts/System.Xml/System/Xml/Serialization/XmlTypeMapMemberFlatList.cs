namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberFlatList : XmlTypeMapMemberExpandable
	{
		private ListMap _listMap;

		public ListMap ListMap
		{
			get
			{
				return _listMap;
			}
			set
			{
				_listMap = value;
			}
		}
	}
}
