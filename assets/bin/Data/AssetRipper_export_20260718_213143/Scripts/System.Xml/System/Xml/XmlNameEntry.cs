namespace System.Xml
{
	internal class XmlNameEntry
	{
		public string Prefix;

		public string LocalName;

		public string NS;

		public int Hash;

		private string prefixed_name_cache;

		public XmlNameEntry(string prefix, string local, string ns)
		{
			Update(prefix, local, ns);
		}

		public void Update(string prefix, string local, string ns)
		{
			Prefix = prefix;
			LocalName = local;
			NS = ns;
			Hash = local.GetHashCode() + ((prefix.Length > 0) ? prefix.GetHashCode() : 0);
		}

		public override bool Equals(object other)
		{
			XmlNameEntry xmlNameEntry = other as XmlNameEntry;
			return xmlNameEntry != null && xmlNameEntry.Hash == Hash && object.ReferenceEquals(xmlNameEntry.LocalName, LocalName) && object.ReferenceEquals(xmlNameEntry.NS, NS) && object.ReferenceEquals(xmlNameEntry.Prefix, Prefix);
		}

		public override int GetHashCode()
		{
			return Hash;
		}

		public string GetPrefixedName(XmlNameEntryCache owner)
		{
			if (prefixed_name_cache == null)
			{
				prefixed_name_cache = owner.GetAtomizedPrefixedName(Prefix, LocalName);
			}
			return prefixed_name_cache;
		}
	}
}
