namespace System.Xml.Schema
{
	internal struct StringArrayValueType
	{
		private string[] value;

		public string[] Value
		{
			get
			{
				return value;
			}
		}

		public StringArrayValueType(string[] value)
		{
			this.value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is StringArrayValueType)
			{
				return (StringArrayValueType)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static bool operator ==(StringArrayValueType v1, StringArrayValueType v2)
		{
			return v1.Value == v2.Value;
		}

		public static bool operator !=(StringArrayValueType v1, StringArrayValueType v2)
		{
			return v1.Value != v2.Value;
		}
	}
}
