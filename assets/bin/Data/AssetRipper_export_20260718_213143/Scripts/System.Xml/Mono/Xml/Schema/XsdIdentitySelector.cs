using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdIdentitySelector
	{
		private XsdIdentityPath[] selectorPaths;

		private ArrayList fields = new ArrayList();

		private XsdIdentityField[] cachedFields;

		public XsdIdentityPath[] Paths
		{
			get
			{
				return selectorPaths;
			}
		}

		public XsdIdentityField[] Fields
		{
			get
			{
				if (cachedFields == null)
				{
					cachedFields = fields.ToArray(typeof(XsdIdentityField)) as XsdIdentityField[];
				}
				return cachedFields;
			}
		}

		public XsdIdentitySelector(XmlSchemaXPath selector)
		{
			selectorPaths = selector.CompiledExpression;
		}

		public void AddField(XsdIdentityField field)
		{
			cachedFields = null;
			fields.Add(field);
		}
	}
}
