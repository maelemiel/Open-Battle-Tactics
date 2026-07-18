using System.Text;

namespace System.Xml.Serialization
{
	internal class SoapTypeSerializationSource : SerializationSource
	{
		private string attributeOverridesHash;

		private Type type;

		public SoapTypeSerializationSource(Type type, SoapAttributeOverrides attributeOverrides, string namspace, Type[] includedTypes)
			: base(namspace, includedTypes)
		{
			if (attributeOverrides != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				attributeOverrides.AddKeyHash(stringBuilder);
				attributeOverridesHash = stringBuilder.ToString();
			}
			this.type = type;
		}

		public override bool Equals(object o)
		{
			SoapTypeSerializationSource soapTypeSerializationSource = o as SoapTypeSerializationSource;
			if (soapTypeSerializationSource == null)
			{
				return false;
			}
			if (!type.Equals(soapTypeSerializationSource.type))
			{
				return false;
			}
			if (attributeOverridesHash != soapTypeSerializationSource.attributeOverridesHash)
			{
				return false;
			}
			return BaseEquals(soapTypeSerializationSource);
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();
		}
	}
}
