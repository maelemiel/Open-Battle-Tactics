using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdKeyEntryField
	{
		private XsdKeyEntry entry;

		private XsdIdentityField field;

		public bool FieldFound;

		public int FieldLineNumber;

		public int FieldLinePosition;

		public bool FieldHasLineInfo;

		public XsdAnySimpleType FieldType;

		public object Identity;

		public bool IsXsiNil;

		public int FieldFoundDepth;

		public XsdIdentityPath FieldFoundPath;

		public bool Consuming;

		public bool Consumed;

		public XsdIdentityField Field
		{
			get
			{
				return field;
			}
		}

		public XsdKeyEntryField(XsdKeyEntry entry, XsdIdentityField field)
		{
			this.entry = entry;
			this.field = field;
		}

		public bool SetIdentityField(object identity, bool isXsiNil, XsdAnySimpleType type, int depth, IXmlLineInfo li)
		{
			FieldFoundDepth = depth;
			Identity = identity;
			IsXsiNil = isXsiNil;
			FieldFound |= isXsiNil;
			FieldType = type;
			Consuming = false;
			Consumed = true;
			if (li != null && li.HasLineInfo())
			{
				FieldHasLineInfo = true;
				FieldLineNumber = li.LineNumber;
				FieldLinePosition = li.LinePosition;
			}
			if (!(entry.OwnerSequence.SourceSchemaIdentity is XmlSchemaKeyref))
			{
				for (int i = 0; i < entry.OwnerSequence.FinishedEntries.Count; i++)
				{
					XsdKeyEntry other = entry.OwnerSequence.FinishedEntries[i];
					if (entry.CompareIdentity(other))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal XsdIdentityPath Matches(bool matchesAttr, object sender, XmlNameTable nameTable, ArrayList qnameStack, string sourceUri, object schemaType, IXmlNamespaceResolver nsResolver, IXmlLineInfo lineInfo, int depth, string attrName, string attrNS, object attrValue)
		{
			XsdIdentityPath xsdIdentityPath = null;
			for (int i = 0; i < field.Paths.Length; i++)
			{
				XsdIdentityPath xsdIdentityPath2 = field.Paths[i];
				bool isAttribute = xsdIdentityPath2.IsAttribute;
				if (matchesAttr != isAttribute)
				{
					continue;
				}
				if (xsdIdentityPath2.IsAttribute)
				{
					XsdIdentityStep xsdIdentityStep = xsdIdentityPath2.OrderedSteps[xsdIdentityPath2.OrderedSteps.Length - 1];
					bool flag = false;
					if (xsdIdentityStep.IsAnyName || xsdIdentityStep.NsName != null)
					{
						if (xsdIdentityStep.IsAnyName || attrNS == xsdIdentityStep.NsName)
						{
							flag = true;
						}
					}
					else if (xsdIdentityStep.Name == attrName && xsdIdentityStep.Namespace == attrNS)
					{
						flag = true;
					}
					if (!flag || entry.StartDepth + (xsdIdentityPath2.OrderedSteps.Length - 1) != depth - 1)
					{
						continue;
					}
					xsdIdentityPath = xsdIdentityPath2;
				}
				if (FieldFound && depth > FieldFoundDepth && FieldFoundPath == xsdIdentityPath2)
				{
					continue;
				}
				if (xsdIdentityPath2.OrderedSteps.Length == 0)
				{
					if (depth == entry.StartDepth)
					{
						return xsdIdentityPath2;
					}
				}
				else
				{
					if (depth - entry.StartDepth < xsdIdentityPath2.OrderedSteps.Length - 1)
					{
						continue;
					}
					int num = xsdIdentityPath2.OrderedSteps.Length;
					if (isAttribute)
					{
						num--;
					}
					if ((xsdIdentityPath2.Descendants && depth < entry.StartDepth + num) || (!xsdIdentityPath2.Descendants && depth != entry.StartDepth + num))
					{
						continue;
					}
					for (num--; num >= 0; num--)
					{
						XsdIdentityStep xsdIdentityStep = xsdIdentityPath2.OrderedSteps[num];
						if (!xsdIdentityStep.IsCurrent && !xsdIdentityStep.IsAnyName)
						{
							XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)qnameStack[entry.StartDepth + num + ((!isAttribute) ? 1 : 0)];
							if ((xsdIdentityStep.NsName == null || !(xmlQualifiedName.Namespace == xsdIdentityStep.NsName)) && ((!(xsdIdentityStep.Name == "*") && !(xsdIdentityStep.Name == xmlQualifiedName.Name)) || !(xsdIdentityStep.Namespace == xmlQualifiedName.Namespace)))
							{
								break;
							}
						}
					}
					if (num < 0 && !matchesAttr)
					{
						return xsdIdentityPath2;
					}
				}
			}
			if (xsdIdentityPath != null)
			{
				FillAttributeFieldValue(sender, nameTable, sourceUri, schemaType, nsResolver, attrValue, lineInfo, depth);
				if (Identity != null)
				{
					return xsdIdentityPath;
				}
			}
			return null;
		}

		private void FillAttributeFieldValue(object sender, XmlNameTable nameTable, string sourceUri, object schemaType, IXmlNamespaceResolver nsResolver, object identity, IXmlLineInfo lineInfo, int depth)
		{
			if (FieldFound)
			{
				throw new XmlSchemaValidationException(string.Format("The key value was already found as '{0}'{1}.", Identity, (!FieldHasLineInfo) ? string.Empty : string.Format(CultureInfo.InvariantCulture, " at line {0}, position {1}", FieldLineNumber, FieldLinePosition)), sender, sourceUri, entry.OwnerSequence.SourceSchemaIdentity, null);
			}
			XmlSchemaDatatype xmlSchemaDatatype = schemaType as XmlSchemaDatatype;
			XmlSchemaSimpleType xmlSchemaSimpleType = schemaType as XmlSchemaSimpleType;
			if (xmlSchemaDatatype == null && xmlSchemaSimpleType != null)
			{
				xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
			}
			try
			{
				if (!SetIdentityField(identity, false, xmlSchemaDatatype as XsdAnySimpleType, depth, lineInfo))
				{
					throw new XmlSchemaValidationException("Two or more identical field was found.", sender, sourceUri, entry.OwnerSequence.SourceSchemaIdentity, null);
				}
				Consuming = true;
				FieldFound = true;
			}
			catch (Exception innerException)
			{
				throw new XmlSchemaValidationException("Failed to read typed value.", sender, sourceUri, entry.OwnerSequence.SourceSchemaIdentity, innerException);
			}
		}
	}
}
