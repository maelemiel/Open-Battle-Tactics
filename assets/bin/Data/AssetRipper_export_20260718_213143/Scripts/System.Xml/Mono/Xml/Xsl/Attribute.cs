namespace Mono.Xml.Xsl
{
	internal struct Attribute
	{
		public string Prefix;

		public string Namespace;

		public string LocalName;

		public string Value;

		public Attribute(string prefix, string namespaceUri, string localName, string value)
		{
			Prefix = prefix;
			Namespace = namespaceUri;
			LocalName = localName;
			Value = value;
		}
	}
}
