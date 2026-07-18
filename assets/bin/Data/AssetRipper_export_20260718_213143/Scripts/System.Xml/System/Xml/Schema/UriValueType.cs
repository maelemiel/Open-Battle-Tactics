using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	internal struct UriValueType
	{
		private XmlSchemaUri value;

		public XmlSchemaUri Value
		{
			get
			{
				return value;
			}
		}

		public UriValueType(XmlSchemaUri value)
		{
			this.value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is UriValueType)
			{
				return (UriValueType)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public static bool operator ==(UriValueType v1, UriValueType v2)
		{
			return v1.Value == v2.Value;
		}

		public static bool operator !=(UriValueType v1, UriValueType v2)
		{
			return v1.Value != v2.Value;
		}
	}
}
