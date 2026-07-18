using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdIdentityField
	{
		private XsdIdentityPath[] fieldPaths;

		private int index;

		public XsdIdentityPath[] Paths
		{
			get
			{
				return fieldPaths;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public XsdIdentityField(XmlSchemaXPath field, int index)
		{
			this.index = index;
			fieldPaths = field.CompiledExpression;
		}
	}
}
