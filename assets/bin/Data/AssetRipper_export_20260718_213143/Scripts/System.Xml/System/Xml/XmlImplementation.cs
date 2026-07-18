using System.Globalization;

namespace System.Xml
{
	public class XmlImplementation
	{
		internal XmlNameTable InternalNameTable;

		public XmlImplementation()
			: this(new NameTable())
		{
		}

		public XmlImplementation(XmlNameTable nameTable)
		{
			InternalNameTable = nameTable;
		}

		public virtual XmlDocument CreateDocument()
		{
			return new XmlDocument(this);
		}

		public bool HasFeature(string strFeature, string strVersion)
		{
			if (string.Compare(strFeature, "xml", true, CultureInfo.InvariantCulture) == 0)
			{
				switch (strVersion)
				{
				case "1.0":
				case "2.0":
				case null:
					return true;
				}
			}
			return false;
		}
	}
}
