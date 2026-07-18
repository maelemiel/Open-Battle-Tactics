namespace System.Xml.Serialization
{
	public class SoapSchemaMember
	{
		private string memberName;

		private XmlQualifiedName memberType = XmlQualifiedName.Empty;

		public string MemberName
		{
			get
			{
				if (memberName == null)
				{
					return string.Empty;
				}
				return memberName;
			}
			set
			{
				memberName = value;
			}
		}

		public XmlQualifiedName MemberType
		{
			get
			{
				return memberType;
			}
			set
			{
				memberType = value;
			}
		}
	}
}
