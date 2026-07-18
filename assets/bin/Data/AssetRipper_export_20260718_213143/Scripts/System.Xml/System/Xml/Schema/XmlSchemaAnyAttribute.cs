using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaAnyAttribute : XmlSchemaAnnotated
	{
		private const string xmlname = "anyAttribute";

		private string nameSpace;

		private XmlSchemaContentProcessing processing;

		private XsdWildcard wildcard;

		[XmlAttribute("namespace")]
		public string Namespace
		{
			get
			{
				return nameSpace;
			}
			set
			{
				nameSpace = value;
			}
		}

		[XmlAttribute("processContents")]
		[DefaultValue(XmlSchemaContentProcessing.None)]
		public XmlSchemaContentProcessing ProcessContents
		{
			get
			{
				return processing;
			}
			set
			{
				processing = value;
			}
		}

		internal bool HasValueAny
		{
			get
			{
				return wildcard.HasValueAny;
			}
		}

		internal bool HasValueLocal
		{
			get
			{
				return wildcard.HasValueLocal;
			}
		}

		internal bool HasValueOther
		{
			get
			{
				return wildcard.HasValueOther;
			}
		}

		internal bool HasValueTargetNamespace
		{
			get
			{
				return wildcard.HasValueTargetNamespace;
			}
		}

		internal StringCollection ResolvedNamespaces
		{
			get
			{
				return wildcard.ResolvedNamespaces;
			}
		}

		internal XmlSchemaContentProcessing ResolvedProcessContents
		{
			get
			{
				return wildcard.ResolvedProcessing;
			}
		}

		internal string TargetNamespace
		{
			get
			{
				return wildcard.TargetNamespace;
			}
		}

		public XmlSchemaAnyAttribute()
		{
			wildcard = new XsdWildcard(this);
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			wildcard.TargetNamespace = base.AncestorSchema.TargetNamespace;
			if (wildcard.TargetNamespace == null)
			{
				wildcard.TargetNamespace = string.Empty;
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			wildcard.Compile(Namespace, h, schema);
			if (processing == XmlSchemaContentProcessing.None)
			{
				wildcard.ResolvedProcessing = XmlSchemaContentProcessing.Strict;
			}
			else
			{
				wildcard.ResolvedProcessing = processing;
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return errorCount;
		}

		internal void ValidateWildcardSubset(XmlSchemaAnyAttribute other, ValidationEventHandler h, XmlSchema schema)
		{
			wildcard.ValidateWildcardSubset(other.wildcard, h, schema);
		}

		internal bool ValidateWildcardAllowsNamespaceName(string ns, XmlSchema schema)
		{
			return wildcard.ValidateWildcardAllowsNamespaceName(ns, null, schema, false);
		}

		internal static XmlSchemaAnyAttribute Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = new XmlSchemaAnyAttribute();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "anyAttribute")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAnyAttribute.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAnyAttribute.LineNumber = reader.LineNumber;
			xmlSchemaAnyAttribute.LinePosition = reader.LinePosition;
			xmlSchemaAnyAttribute.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAnyAttribute.Id = reader.Value;
				}
				else if (reader.Name == "namespace")
				{
					xmlSchemaAnyAttribute.nameSpace = reader.Value;
				}
				else if (reader.Name == "processContents")
				{
					Exception innerExcpetion;
					xmlSchemaAnyAttribute.processing = XmlSchemaUtil.ReadProcessingAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for processContents", innerExcpetion);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for anyAttribute", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAnyAttribute);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAnyAttribute;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "anyAttribute")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAnyAttribute.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAnyAttribute.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAnyAttribute;
		}
	}
}
