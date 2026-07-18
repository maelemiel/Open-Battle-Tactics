using System.Collections;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdKeyTable
	{
		public readonly bool alwaysTrue = true;

		private XsdIdentitySelector selector;

		private XmlSchemaIdentityConstraint source;

		private XmlQualifiedName qname;

		private XmlQualifiedName refKeyName;

		public XsdKeyEntryCollection Entries = new XsdKeyEntryCollection();

		public XsdKeyEntryCollection FinishedEntries = new XsdKeyEntryCollection();

		public int StartDepth;

		public XsdKeyTable ReferencedKey;

		public XmlQualifiedName QualifiedName
		{
			get
			{
				return qname;
			}
		}

		public XmlQualifiedName RefKeyName
		{
			get
			{
				return refKeyName;
			}
		}

		public XmlSchemaIdentityConstraint SourceSchemaIdentity
		{
			get
			{
				return source;
			}
		}

		public XsdIdentitySelector Selector
		{
			get
			{
				return selector;
			}
		}

		public XsdKeyTable(XmlSchemaIdentityConstraint source)
		{
			Reset(source);
		}

		public void Reset(XmlSchemaIdentityConstraint source)
		{
			this.source = source;
			selector = source.CompiledSelector;
			qname = source.QualifiedName;
			XmlSchemaKeyref xmlSchemaKeyref = source as XmlSchemaKeyref;
			if (xmlSchemaKeyref != null)
			{
				refKeyName = xmlSchemaKeyref.Refer;
			}
			StartDepth = 0;
		}

		public XsdIdentityPath SelectorMatches(ArrayList qnameStack, int depth)
		{
			for (int i = 0; i < Selector.Paths.Length; i++)
			{
				XsdIdentityPath xsdIdentityPath = Selector.Paths[i];
				if (depth == StartDepth)
				{
					if (xsdIdentityPath.OrderedSteps.Length == 0)
					{
						return xsdIdentityPath;
					}
				}
				else
				{
					if (depth - StartDepth < xsdIdentityPath.OrderedSteps.Length - 1)
					{
						continue;
					}
					int num = xsdIdentityPath.OrderedSteps.Length;
					if (xsdIdentityPath.OrderedSteps[num - 1].IsAttribute)
					{
						num--;
					}
					if ((xsdIdentityPath.Descendants && depth < StartDepth + num) || (!xsdIdentityPath.Descendants && depth != StartDepth + num))
					{
						continue;
					}
					num--;
					int num2 = 0;
					while (0 <= num)
					{
						XsdIdentityStep xsdIdentityStep = xsdIdentityPath.OrderedSteps[num];
						if (!xsdIdentityStep.IsAnyName)
						{
							XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)qnameStack[qnameStack.Count - num2 - 1];
							if ((xsdIdentityStep.NsName == null || !(xmlQualifiedName.Namespace == xsdIdentityStep.NsName)) && (!(xsdIdentityStep.Name == xmlQualifiedName.Name) || !(xsdIdentityStep.Namespace == xmlQualifiedName.Namespace)) && alwaysTrue)
							{
								break;
							}
						}
						num2++;
						num--;
					}
					if (num < 0)
					{
						return xsdIdentityPath;
					}
				}
			}
			return null;
		}
	}
}
