namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberExpandable : XmlTypeMapMemberElement
	{
		private int _flatArrayIndex;

		public int FlatArrayIndex
		{
			get
			{
				return _flatArrayIndex;
			}
			set
			{
				_flatArrayIndex = value;
			}
		}
	}
}
