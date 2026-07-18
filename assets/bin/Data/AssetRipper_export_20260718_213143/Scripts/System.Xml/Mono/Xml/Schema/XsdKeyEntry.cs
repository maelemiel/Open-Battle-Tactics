using System;
using System.Collections;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdKeyEntry
	{
		public int StartDepth;

		public int SelectorLineNumber;

		public int SelectorLinePosition;

		public bool SelectorHasLineInfo;

		public XsdKeyEntryFieldCollection KeyFields;

		public bool KeyRefFound;

		public XsdKeyTable OwnerSequence;

		private bool keyFound;

		public bool KeyFound
		{
			get
			{
				if (keyFound)
				{
					return true;
				}
				for (int i = 0; i < KeyFields.Count; i++)
				{
					XsdKeyEntryField xsdKeyEntryField = KeyFields[i];
					if (xsdKeyEntryField.FieldFound)
					{
						keyFound = true;
						return true;
					}
				}
				return false;
			}
		}

		public XsdKeyEntry(XsdKeyTable keyseq, int depth, IXmlLineInfo li)
		{
			Init(keyseq, depth, li);
		}

		private void Init(XsdKeyTable keyseq, int depth, IXmlLineInfo li)
		{
			OwnerSequence = keyseq;
			KeyFields = new XsdKeyEntryFieldCollection();
			for (int i = 0; i < keyseq.Selector.Fields.Length; i++)
			{
				KeyFields.Add(new XsdKeyEntryField(this, keyseq.Selector.Fields[i]));
			}
			StartDepth = depth;
			if (li != null && li.HasLineInfo())
			{
				SelectorHasLineInfo = true;
				SelectorLineNumber = li.LineNumber;
				SelectorLinePosition = li.LinePosition;
			}
		}

		public bool CompareIdentity(XsdKeyEntry other)
		{
			for (int i = 0; i < KeyFields.Count; i++)
			{
				XsdKeyEntryField xsdKeyEntryField = KeyFields[i];
				XsdKeyEntryField xsdKeyEntryField2 = other.KeyFields[i];
				if ((xsdKeyEntryField.IsXsiNil && !xsdKeyEntryField2.IsXsiNil) || (!xsdKeyEntryField.IsXsiNil && xsdKeyEntryField2.IsXsiNil))
				{
					return false;
				}
				if (!XmlSchemaUtil.AreSchemaDatatypeEqual(xsdKeyEntryField2.FieldType, xsdKeyEntryField2.Identity, xsdKeyEntryField.FieldType, xsdKeyEntryField.Identity))
				{
					return false;
				}
			}
			return true;
		}

		public void ProcessMatch(bool isAttribute, ArrayList qnameStack, object sender, XmlNameTable nameTable, string sourceUri, object schemaType, IXmlNamespaceResolver nsResolver, IXmlLineInfo li, int depth, string attrName, string attrNS, object attrValue, bool isXsiNil, ArrayList currentKeyFieldConsumers)
		{
			for (int i = 0; i < KeyFields.Count; i++)
			{
				XsdKeyEntryField xsdKeyEntryField = KeyFields[i];
				XsdIdentityPath xsdIdentityPath = xsdKeyEntryField.Matches(isAttribute, sender, nameTable, qnameStack, sourceUri, schemaType, nsResolver, li, depth, attrName, attrNS, attrValue);
				if (xsdIdentityPath == null)
				{
					continue;
				}
				if (xsdKeyEntryField.FieldFound)
				{
					if (!xsdKeyEntryField.Consuming)
					{
						throw new XmlSchemaValidationException("Two or more matching field was found.", sender, sourceUri, OwnerSequence.SourceSchemaIdentity, null);
					}
					xsdKeyEntryField.Consuming = false;
				}
				if (!xsdKeyEntryField.Consumed)
				{
					if (isXsiNil && !xsdKeyEntryField.SetIdentityField(Guid.Empty, true, XsdAnySimpleType.Instance, depth, li))
					{
						throw new XmlSchemaValidationException("Two or more identical field was found.", sender, sourceUri, OwnerSequence.SourceSchemaIdentity, null);
					}
					XmlSchemaComplexType xmlSchemaComplexType = schemaType as XmlSchemaComplexType;
					if (xmlSchemaComplexType != null && (xmlSchemaComplexType.ContentType == XmlSchemaContentType.Empty || xmlSchemaComplexType.ContentType == XmlSchemaContentType.ElementOnly) && schemaType != XmlSchemaComplexType.AnyType)
					{
						throw new XmlSchemaValidationException("Specified schema type is complex type, which is not allowed for identity constraints.", sender, sourceUri, OwnerSequence.SourceSchemaIdentity, null);
					}
					xsdKeyEntryField.FieldFound = true;
					xsdKeyEntryField.FieldFoundPath = xsdIdentityPath;
					xsdKeyEntryField.FieldFoundDepth = depth;
					xsdKeyEntryField.Consuming = true;
					if (li != null && li.HasLineInfo())
					{
						xsdKeyEntryField.FieldHasLineInfo = true;
						xsdKeyEntryField.FieldLineNumber = li.LineNumber;
						xsdKeyEntryField.FieldLinePosition = li.LinePosition;
					}
					currentKeyFieldConsumers.Add(xsdKeyEntryField);
				}
			}
		}
	}
}
