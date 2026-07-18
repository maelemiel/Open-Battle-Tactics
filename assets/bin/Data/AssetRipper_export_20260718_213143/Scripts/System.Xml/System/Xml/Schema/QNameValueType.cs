namespace System.Xml.Schema
{
	internal struct QNameValueType
	{
		private XmlQualifiedName value;

		public XmlQualifiedName Value
		{
			get
			{
				return value;
			}
		}

		public QNameValueType(XmlQualifiedName value)
		{
			this.value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is QNameValueType)
			{
				return (QNameValueType)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static bool operator ==(QNameValueType v1, QNameValueType v2)
		{
			return v1.Value == v2.Value;
		}

		public static bool operator !=(QNameValueType v1, QNameValueType v2)
		{
			return v1.Value != v2.Value;
		}
	}
}
