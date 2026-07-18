using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaAny : XmlSchemaParticle
	{
		private const string xmlname = "any";

		private static XmlSchemaAny anyTypeContent;

		private string nameSpace;

		private XmlSchemaContentProcessing processing;

		private XsdWildcard wildcard;

		internal static XmlSchemaAny AnyTypeContent
		{
			get
			{
				if (anyTypeContent == null)
				{
					anyTypeContent = new XmlSchemaAny();
					anyTypeContent.MaxOccursString = "unbounded";
					anyTypeContent.MinOccurs = 0m;
					anyTypeContent.CompileOccurence(null, null);
					anyTypeContent.Namespace = "##any";
					anyTypeContent.wildcard.HasValueAny = true;
					anyTypeContent.wildcard.ResolvedNamespaces = new StringCollection();
					XsdWildcard xsdWildcard = anyTypeContent.wildcard;
					XmlSchemaContentProcessing xmlSchemaContentProcessing = XmlSchemaContentProcessing.Lax;
					anyTypeContent.ProcessContents = xmlSchemaContentProcessing;
					xsdWildcard.ResolvedProcessing = xmlSchemaContentProcessing;
					anyTypeContent.wildcard.SkipCompile = true;
				}
				return anyTypeContent;
			}
		}

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

		public XmlSchemaAny()
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
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			wildcard.TargetNamespace = base.AncestorSchema.TargetNamespace;
			if (wildcard.TargetNamespace == null)
			{
				wildcard.TargetNamespace = string.Empty;
			}
			CompileOccurence(h, schema);
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

		internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			if (OptimizedParticle != null)
			{
				return OptimizedParticle;
			}
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			CopyInfo(xmlSchemaAny);
			xmlSchemaAny.CompileOccurence(null, null);
			xmlSchemaAny.wildcard = wildcard;
			OptimizedParticle = xmlSchemaAny;
			xmlSchemaAny.Namespace = Namespace;
			xmlSchemaAny.ProcessContents = ProcessContents;
			xmlSchemaAny.Annotation = base.Annotation;
			xmlSchemaAny.UnhandledAttributes = base.UnhandledAttributes;
			return OptimizedParticle;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return errorCount;
		}

		internal override bool ParticleEquals(XmlSchemaParticle other)
		{
			XmlSchemaAny xmlSchemaAny = other as XmlSchemaAny;
			if (xmlSchemaAny == null)
			{
				return false;
			}
			if (HasValueAny != xmlSchemaAny.HasValueAny || HasValueLocal != xmlSchemaAny.HasValueLocal || HasValueOther != xmlSchemaAny.HasValueOther || HasValueTargetNamespace != xmlSchemaAny.HasValueTargetNamespace || ResolvedProcessContents != xmlSchemaAny.ResolvedProcessContents || base.ValidatedMaxOccurs != xmlSchemaAny.ValidatedMaxOccurs || base.ValidatedMinOccurs != xmlSchemaAny.ValidatedMinOccurs || ResolvedNamespaces.Count != xmlSchemaAny.ResolvedNamespaces.Count)
			{
				return false;
			}
			for (int i = 0; i < ResolvedNamespaces.Count; i++)
			{
				if (ResolvedNamespaces[i] != xmlSchemaAny.ResolvedNamespaces[i])
				{
					return false;
				}
			}
			return true;
		}

		internal bool ExamineAttributeWildcardIntersection(XmlSchemaAny other, ValidationEventHandler h, XmlSchema schema)
		{
			return wildcard.ExamineAttributeWildcardIntersection(other, h, schema);
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			if (xmlSchemaAny == null)
			{
				if (raiseError)
				{
					error(h, "Invalid particle derivation by restriction was found.");
				}
				return false;
			}
			if (!ValidateOccurenceRangeOK(baseParticle, h, schema, raiseError))
			{
				return false;
			}
			return wildcard.ValidateWildcardSubset(xmlSchemaAny.wildcard, h, schema, raiseError);
		}

		internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaAny nsName in nsNames)
			{
				if (!ExamineAttributeWildcardIntersection(nsName, h, schema))
				{
					error(h, "Ambiguous -any- particle was found.");
				}
			}
			nsNames.Add(this);
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal bool ValidateWildcardAllowsNamespaceName(string ns, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			return wildcard.ValidateWildcardAllowsNamespaceName(ns, h, schema, raiseError);
		}

		internal static XmlSchemaAny Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "any")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAny.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAny.LineNumber = reader.LineNumber;
			xmlSchemaAny.LinePosition = reader.LinePosition;
			xmlSchemaAny.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAny.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaAny.MaxOccursString = reader.Value;
					}
					catch (Exception innerException)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for maxOccurs", innerException);
					}
				}
				else if (reader.Name == "minOccurs")
				{
					try
					{
						xmlSchemaAny.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if (reader.Name == "namespace")
				{
					xmlSchemaAny.nameSpace = reader.Value;
				}
				else if (reader.Name == "processContents")
				{
					Exception innerExcpetion;
					xmlSchemaAny.processing = XmlSchemaUtil.ReadProcessingAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for processContents", innerExcpetion);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for any", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAny);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAny;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "any")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAny.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAny.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAny;
		}
	}
}
