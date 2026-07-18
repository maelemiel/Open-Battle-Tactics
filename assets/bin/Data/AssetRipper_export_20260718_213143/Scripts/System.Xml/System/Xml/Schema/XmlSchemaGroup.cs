using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaGroup : XmlSchemaAnnotated
	{
		private const string xmlname = "group";

		private string name;

		private XmlSchemaGroupBase particle;

		private XmlQualifiedName qualifiedName;

		private bool isCircularDefinition;

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[XmlElement("all", typeof(XmlSchemaAll))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				return particle;
			}
			set
			{
				particle = value;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return qualifiedName;
			}
		}

		internal bool IsCircularDefinition
		{
			get
			{
				return isCircularDefinition;
			}
		}

		public XmlSchemaGroup()
		{
			qualifiedName = XmlQualifiedName.Empty;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (Particle != null)
			{
				Particle.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			if (Name == null)
			{
				error(h, "Required attribute name must be present");
			}
			else if (!XmlSchemaUtil.CheckNCName(name))
			{
				error(h, "attribute name must be NCName");
			}
			else
			{
				qualifiedName = new XmlQualifiedName(Name, base.AncestorSchema.TargetNamespace);
			}
			if (Particle == null)
			{
				error(h, "Particle is required");
			}
			else
			{
				if (Particle.MaxOccursString != null)
				{
					Particle.error(h, "MaxOccurs must not be present when the Particle is a child of Group");
				}
				if (Particle.MinOccursString != null)
				{
					Particle.error(h, "MinOccurs must not be present when the Particle is a child of Group");
				}
				Particle.Compile(h, schema);
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			if (Particle != null)
			{
				Particle.parentIsGroupDefinition = true;
				try
				{
					Particle.CheckRecursion(0, h, schema);
				}
				catch (XmlSchemaException ex)
				{
					XmlSchemaObject.error(h, ex.Message, ex);
					isCircularDefinition = true;
					return errorCount;
				}
				errorCount += Particle.Validate(h, schema);
				Particle.ValidateUniqueParticleAttribution(new XmlSchemaObjectTable(), new ArrayList(), h, schema);
				Particle.ValidateUniqueTypeAttribution(new XmlSchemaObjectTable(), h, schema);
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaGroup Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaGroup xmlSchemaGroup = new XmlSchemaGroup();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "group")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaGroup.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaGroup.LineNumber = reader.LineNumber;
			xmlSchemaGroup.LinePosition = reader.LinePosition;
			xmlSchemaGroup.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaGroup.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaGroup.name = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for group", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaGroup);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaGroup;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "group")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaGroup.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaGroup.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "all")
					{
						num = 3;
						XmlSchemaAll xmlSchemaAll = XmlSchemaAll.Read(reader, h);
						if (xmlSchemaAll != null)
						{
							xmlSchemaGroup.Particle = xmlSchemaAll;
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 3;
						XmlSchemaChoice xmlSchemaChoice = XmlSchemaChoice.Read(reader, h);
						if (xmlSchemaChoice != null)
						{
							xmlSchemaGroup.Particle = xmlSchemaChoice;
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 3;
						XmlSchemaSequence xmlSchemaSequence = XmlSchemaSequence.Read(reader, h);
						if (xmlSchemaSequence != null)
						{
							xmlSchemaGroup.Particle = xmlSchemaSequence;
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaGroup;
		}
	}
}
