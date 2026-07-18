using System.Text;

namespace System.Xml.Serialization
{
	internal class XmlTypeSerializationSource : SerializationSource
	{
		private string attributeOverridesHash;

		private Type type;

		private string rootHash;

		public XmlTypeSerializationSource(Type type, XmlRootAttribute root, XmlAttributeOverrides attributeOverrides, string namspace, Type[] includedTypes)
			: base(namspace, includedTypes)
		{
			if (attributeOverrides != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				attributeOverrides.AddKeyHash(stringBuilder);
				attributeOverridesHash = stringBuilder.ToString();
			}
			if (root != null)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				root.AddKeyHash(stringBuilder2);
				rootHash = stringBuilder2.ToString();
			}
			this.type = type;
		}

		public override bool Equals(object o)
		{
			XmlTypeSerializationSource xmlTypeSerializationSource = o as XmlTypeSerializationSource;
			if (xmlTypeSerializationSource == null)
			{
				return false;
			}
			if (!type.Equals(xmlTypeSerializationSource.type))
			{
				return false;
			}
			if (rootHash != xmlTypeSerializationSource.rootHash)
			{
				return false;
			}
			if (attributeOverridesHash != xmlTypeSerializationSource.attributeOverridesHash)
			{
				return false;
			}
			return BaseEquals(xmlTypeSerializationSource);
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();
		}
	}
}
