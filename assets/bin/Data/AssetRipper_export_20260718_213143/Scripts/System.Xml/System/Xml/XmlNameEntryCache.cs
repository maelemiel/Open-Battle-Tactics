using System.Collections;

namespace System.Xml
{
	internal class XmlNameEntryCache
	{
		private Hashtable table = new Hashtable();

		private XmlNameTable nameTable;

		private XmlNameEntry dummy = new XmlNameEntry(string.Empty, string.Empty, string.Empty);

		private char[] cacheBuffer;

		public XmlNameEntryCache(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
		}

		public string GetAtomizedPrefixedName(string prefix, string local)
		{
			if (prefix == null || prefix.Length == 0)
			{
				return local;
			}
			if (cacheBuffer == null)
			{
				cacheBuffer = new char[20];
			}
			if (cacheBuffer.Length < prefix.Length + local.Length + 1)
			{
				cacheBuffer = new char[Math.Max(prefix.Length + local.Length + 1, cacheBuffer.Length << 1)];
			}
			prefix.CopyTo(0, cacheBuffer, 0, prefix.Length);
			cacheBuffer[prefix.Length] = ':';
			local.CopyTo(0, cacheBuffer, prefix.Length + 1, local.Length);
			return nameTable.Add(cacheBuffer, 0, prefix.Length + local.Length + 1);
		}

		public XmlNameEntry Add(string prefix, string local, string ns, bool atomic)
		{
			if (!atomic)
			{
				prefix = nameTable.Add(prefix);
				local = nameTable.Add(local);
				ns = nameTable.Add(ns);
			}
			XmlNameEntry xmlNameEntry = GetInternal(prefix, local, ns, true);
			if (xmlNameEntry == null)
			{
				xmlNameEntry = new XmlNameEntry(prefix, local, ns);
				table[xmlNameEntry] = xmlNameEntry;
			}
			return xmlNameEntry;
		}

		public XmlNameEntry Get(string prefix, string local, string ns, bool atomic)
		{
			return GetInternal(prefix, local, ns, atomic);
		}

		private XmlNameEntry GetInternal(string prefix, string local, string ns, bool atomic)
		{
			if (!atomic && (nameTable.Get(prefix) == null || nameTable.Get(local) == null || nameTable.Get(ns) == null))
			{
				return null;
			}
			dummy.Update(prefix, local, ns);
			return table[dummy] as XmlNameEntry;
		}
	}
}
