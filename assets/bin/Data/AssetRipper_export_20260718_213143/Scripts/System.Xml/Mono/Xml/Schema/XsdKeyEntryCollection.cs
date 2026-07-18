using System.Collections;

namespace Mono.Xml.Schema
{
	internal class XsdKeyEntryCollection : CollectionBase
	{
		public XsdKeyEntry this[int i]
		{
			get
			{
				return (XsdKeyEntry)base.List[i];
			}
			set
			{
				base.List[i] = value;
			}
		}

		public void Add(XsdKeyEntry entry)
		{
			base.List.Add(entry);
		}
	}
}
