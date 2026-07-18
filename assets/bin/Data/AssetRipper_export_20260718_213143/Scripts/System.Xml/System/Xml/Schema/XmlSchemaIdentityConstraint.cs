using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaIdentityConstraint : XmlSchemaAnnotated
	{
		private XmlSchemaObjectCollection fields;

		private string name;

		private XmlQualifiedName qName;

		private XmlSchemaXPath selector;

		private XsdIdentitySelector compiledSelector;

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

		[XmlElement("selector", typeof(XmlSchemaXPath))]
		public XmlSchemaXPath Selector
		{
			get
			{
				return selector;
			}
			set
			{
				selector = value;
			}
		}

		[XmlElement("field", typeof(XmlSchemaXPath))]
		public XmlSchemaObjectCollection Fields
		{
			get
			{
				return fields;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return qName;
			}
		}

		internal XsdIdentitySelector CompiledSelector
		{
			get
			{
				return compiledSelector;
			}
		}

		public XmlSchemaIdentityConstraint()
		{
			fields = new XmlSchemaObjectCollection();
			qName = XmlQualifiedName.Empty;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (Selector != null)
			{
				Selector.SetParent(this);
			}
			foreach (XmlSchemaObject field in Fields)
			{
				field.SetParent(this);
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
				qName = new XmlQualifiedName(Name, base.AncestorSchema.TargetNamespace);
				if (schema.NamedIdentities.Contains(qName))
				{
					XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = schema.NamedIdentities[qName] as XmlSchemaIdentityConstraint;
					error(h, string.Format("There is already same named identity constraint in this namespace. Existing item is at {0}({1},{2})", xmlSchemaIdentityConstraint.SourceUri, xmlSchemaIdentityConstraint.LineNumber, xmlSchemaIdentityConstraint.LinePosition));
				}
				else
				{
					schema.NamedIdentities.Add(qName, this);
				}
			}
			if (Selector == null)
			{
				error(h, "selector must be present");
			}
			else
			{
				Selector.isSelector = true;
				errorCount += Selector.Compile(h, schema);
				if (selector.errorCount == 0)
				{
					compiledSelector = new XsdIdentitySelector(Selector);
				}
			}
			if (errorCount > 0)
			{
				return errorCount;
			}
			if (Fields.Count == 0)
			{
				error(h, "atleast one field value must be present");
			}
			else
			{
				for (int i = 0; i < Fields.Count; i++)
				{
					XmlSchemaXPath xmlSchemaXPath = Fields[i] as XmlSchemaXPath;
					if (xmlSchemaXPath != null)
					{
						errorCount += xmlSchemaXPath.Compile(h, schema);
						if (xmlSchemaXPath.errorCount == 0)
						{
							compiledSelector.AddField(new XsdIdentityField(xmlSchemaXPath, i));
						}
					}
					else
					{
						error(h, string.Concat("Object of type ", Fields[i].GetType(), " is invalid in the Fields Collection"));
					}
				}
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}
	}
}
