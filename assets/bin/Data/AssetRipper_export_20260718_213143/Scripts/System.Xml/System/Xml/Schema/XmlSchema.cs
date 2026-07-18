using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	[XmlRoot("schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
	public class XmlSchema : XmlSchemaObject
	{
		public const string Namespace = "http://www.w3.org/2001/XMLSchema";

		public const string InstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		internal const string XdtNamespace = "http://www.w3.org/2003/11/xpath-datatypes";

		private const string xmlname = "schema";

		private XmlSchemaForm attributeFormDefault;

		private XmlSchemaObjectTable attributeGroups;

		private XmlSchemaObjectTable attributes;

		private XmlSchemaDerivationMethod blockDefault;

		private XmlSchemaForm elementFormDefault;

		private XmlSchemaObjectTable elements;

		private XmlSchemaDerivationMethod finalDefault;

		private XmlSchemaObjectTable groups;

		private string id;

		private XmlSchemaObjectCollection includes;

		private XmlSchemaObjectCollection items;

		private XmlSchemaObjectTable notations;

		private XmlSchemaObjectTable schemaTypes;

		private string targetNamespace;

		private XmlAttribute[] unhandledAttributes;

		private string version;

		private XmlSchemaSet schemas;

		private XmlNameTable nameTable;

		internal bool missedSubComponents;

		private XmlSchemaObjectCollection compilationItems;

		[XmlAttribute("attributeFormDefault")]
		[DefaultValue(XmlSchemaForm.None)]
		public XmlSchemaForm AttributeFormDefault
		{
			get
			{
				return attributeFormDefault;
			}
			set
			{
				attributeFormDefault = value;
			}
		}

		[XmlAttribute("blockDefault")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		public XmlSchemaDerivationMethod BlockDefault
		{
			get
			{
				return blockDefault;
			}
			set
			{
				blockDefault = value;
			}
		}

		[DefaultValue(XmlSchemaDerivationMethod.None)]
		[XmlAttribute("finalDefault")]
		public XmlSchemaDerivationMethod FinalDefault
		{
			get
			{
				return finalDefault;
			}
			set
			{
				finalDefault = value;
			}
		}

		[DefaultValue(XmlSchemaForm.None)]
		[XmlAttribute("elementFormDefault")]
		public XmlSchemaForm ElementFormDefault
		{
			get
			{
				return elementFormDefault;
			}
			set
			{
				elementFormDefault = value;
			}
		}

		[XmlAttribute("targetNamespace", DataType = "anyURI")]
		public string TargetNamespace
		{
			get
			{
				return targetNamespace;
			}
			set
			{
				targetNamespace = value;
			}
		}

		[XmlAttribute("version", DataType = "token")]
		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		[XmlElement("redefine", typeof(XmlSchemaRedefine))]
		[XmlElement("import", typeof(XmlSchemaImport))]
		[XmlElement("include", typeof(XmlSchemaInclude))]
		public XmlSchemaObjectCollection Includes
		{
			get
			{
				return includes;
			}
		}

		[XmlElement("group", typeof(XmlSchemaGroup))]
		[XmlElement("notation", typeof(XmlSchemaNotation))]
		[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		[XmlElement("complexType", typeof(XmlSchemaComplexType))]
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup))]
		public XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
			}
		}

		[XmlIgnore]
		public bool IsCompiled
		{
			get
			{
				return CompilationId != Guid.Empty;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable Attributes
		{
			get
			{
				return attributes;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable AttributeGroups
		{
			get
			{
				return attributeGroups;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable SchemaTypes
		{
			get
			{
				return schemaTypes;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable Elements
		{
			get
			{
				return elements;
			}
		}

		[XmlAttribute("id", DataType = "ID")]
		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] UnhandledAttributes
		{
			get
			{
				if (unhandledAttributeList != null)
				{
					unhandledAttributes = (XmlAttribute[])unhandledAttributeList.ToArray(typeof(XmlAttribute));
					unhandledAttributeList = null;
				}
				return unhandledAttributes;
			}
			set
			{
				unhandledAttributes = value;
				unhandledAttributeList = null;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable Groups
		{
			get
			{
				return groups;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable Notations
		{
			get
			{
				return notations;
			}
		}

		internal XmlSchemaObjectTable NamedIdentities
		{
			get
			{
				return schemas.NamedIdentities;
			}
		}

		internal XmlSchemaSet Schemas
		{
			get
			{
				return schemas;
			}
		}

		internal Hashtable IDCollection
		{
			get
			{
				return schemas.IDCollection;
			}
		}

		public XmlSchema()
		{
			attributeFormDefault = XmlSchemaForm.None;
			blockDefault = XmlSchemaDerivationMethod.None;
			elementFormDefault = XmlSchemaForm.None;
			finalDefault = XmlSchemaDerivationMethod.None;
			includes = new XmlSchemaObjectCollection();
			isCompiled = false;
			items = new XmlSchemaObjectCollection();
			attributeGroups = new XmlSchemaObjectTable();
			attributes = new XmlSchemaObjectTable();
			elements = new XmlSchemaObjectTable();
			groups = new XmlSchemaObjectTable();
			notations = new XmlSchemaObjectTable();
			schemaTypes = new XmlSchemaObjectTable();
		}

		[Obsolete("Use XmlSchemaSet.Compile() instead.")]
		public void Compile(ValidationEventHandler handler)
		{
			Compile(handler, new XmlUrlResolver());
		}

		[Obsolete("Use XmlSchemaSet.Compile() instead.")]
		public void Compile(ValidationEventHandler handler, XmlResolver resolver)
		{
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			if (handler != null)
			{
				xmlSchemaSet.ValidationEventHandler += handler;
			}
			xmlSchemaSet.XmlResolver = resolver;
			xmlSchemaSet.Add(this);
			xmlSchemaSet.Compile();
		}

		internal void CompileSubset(ValidationEventHandler handler, XmlSchemaSet col, XmlResolver resolver)
		{
			Hashtable handledUris = new Hashtable();
			CompileSubset(handler, col, resolver, handledUris);
		}

		internal void CompileSubset(ValidationEventHandler handler, XmlSchemaSet col, XmlResolver resolver, Hashtable handledUris)
		{
			if (base.SourceUri != null && base.SourceUri.Length > 0)
			{
				if (handledUris.Contains(base.SourceUri))
				{
					return;
				}
				handledUris.Add(base.SourceUri, base.SourceUri);
			}
			DoCompile(handler, handledUris, col, resolver);
		}

		private void SetParent()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].SetParent(this);
			}
			for (int j = 0; j < Includes.Count; j++)
			{
				Includes[j].SetParent(this);
			}
		}

		private void DoCompile(ValidationEventHandler handler, Hashtable handledUris, XmlSchemaSet col, XmlResolver resolver)
		{
			SetParent();
			CompilationId = col.CompilationId;
			schemas = col;
			if (!schemas.Contains(this))
			{
				schemas.Add(this);
			}
			attributeGroups.Clear();
			attributes.Clear();
			elements.Clear();
			groups.Clear();
			notations.Clear();
			schemaTypes.Clear();
			if (BlockDefault != XmlSchemaDerivationMethod.All)
			{
				if ((BlockDefault & XmlSchemaDerivationMethod.List) != XmlSchemaDerivationMethod.Empty)
				{
					error(handler, "list is not allowed in blockDefault attribute");
				}
				if ((BlockDefault & XmlSchemaDerivationMethod.Union) != XmlSchemaDerivationMethod.Empty)
				{
					error(handler, "union is not allowed in blockDefault attribute");
				}
			}
			if (FinalDefault != XmlSchemaDerivationMethod.All && (FinalDefault & XmlSchemaDerivationMethod.Substitution) != XmlSchemaDerivationMethod.Empty)
			{
				error(handler, "substitution is not allowed in finalDefault attribute");
			}
			XmlSchemaUtil.CompileID(Id, this, col.IDCollection, handler);
			if (TargetNamespace != null)
			{
				if (TargetNamespace.Length == 0)
				{
					error(handler, "The targetNamespace attribute cannot have have empty string as its value.");
				}
				if (!XmlSchemaUtil.CheckAnyUri(TargetNamespace))
				{
					error(handler, TargetNamespace + " is not a valid value for targetNamespace attribute of schema");
				}
			}
			if (!XmlSchemaUtil.CheckNormalizedString(Version))
			{
				error(handler, Version + "is not a valid value for version attribute of schema");
			}
			compilationItems = new XmlSchemaObjectCollection();
			for (int i = 0; i < Items.Count; i++)
			{
				compilationItems.Add(Items[i]);
			}
			for (int j = 0; j < Includes.Count; j++)
			{
				ProcessExternal(handler, handledUris, resolver, Includes[j] as XmlSchemaExternal, col);
			}
			for (int k = 0; k < compilationItems.Count; k++)
			{
				XmlSchemaObject xmlSchemaObject = compilationItems[k];
				if (xmlSchemaObject is XmlSchemaAnnotation)
				{
					int num = ((XmlSchemaAnnotation)xmlSchemaObject).Compile(handler, this);
					errorCount += num;
				}
				else if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)xmlSchemaObject;
					int num2 = xmlSchemaAttribute.Compile(handler, this);
					errorCount += num2;
					if (num2 == 0)
					{
						XmlSchemaUtil.AddToTable(Attributes, xmlSchemaAttribute, xmlSchemaAttribute.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaAttributeGroup)
				{
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)xmlSchemaObject;
					int num3 = xmlSchemaAttributeGroup.Compile(handler, this);
					errorCount += num3;
					if (num3 == 0)
					{
						XmlSchemaUtil.AddToTable(AttributeGroups, xmlSchemaAttributeGroup, xmlSchemaAttributeGroup.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)xmlSchemaObject;
					int num4 = xmlSchemaComplexType.Compile(handler, this);
					errorCount += num4;
					if (num4 == 0)
					{
						XmlSchemaUtil.AddToTable(schemaTypes, xmlSchemaComplexType, xmlSchemaComplexType.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaSimpleType)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)xmlSchemaObject;
					xmlSchemaSimpleType.islocal = false;
					int num5 = xmlSchemaSimpleType.Compile(handler, this);
					errorCount += num5;
					if (num5 == 0)
					{
						XmlSchemaUtil.AddToTable(SchemaTypes, xmlSchemaSimpleType, xmlSchemaSimpleType.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaObject;
					xmlSchemaElement.parentIsSchema = true;
					int num6 = xmlSchemaElement.Compile(handler, this);
					errorCount += num6;
					if (num6 == 0)
					{
						XmlSchemaUtil.AddToTable(Elements, xmlSchemaElement, xmlSchemaElement.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaGroup)
				{
					XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)xmlSchemaObject;
					int num7 = xmlSchemaGroup.Compile(handler, this);
					errorCount += num7;
					if (num7 == 0)
					{
						XmlSchemaUtil.AddToTable(Groups, xmlSchemaGroup, xmlSchemaGroup.QualifiedName, handler);
					}
				}
				else if (xmlSchemaObject is XmlSchemaNotation)
				{
					XmlSchemaNotation xmlSchemaNotation = (XmlSchemaNotation)xmlSchemaObject;
					int num8 = xmlSchemaNotation.Compile(handler, this);
					errorCount += num8;
					if (num8 == 0)
					{
						XmlSchemaUtil.AddToTable(Notations, xmlSchemaNotation, xmlSchemaNotation.QualifiedName, handler);
					}
				}
				else
				{
					ValidationHandler.RaiseValidationEvent(handler, null, string.Format("Object of Type {0} is not valid in Item Property of Schema", xmlSchemaObject.GetType().Name), null, this, null, XmlSeverityType.Error);
				}
			}
		}

		private string GetResolvedUri(XmlResolver resolver, string relativeUri)
		{
			Uri baseUri = null;
			if (base.SourceUri != null && base.SourceUri != string.Empty)
			{
				baseUri = new Uri(base.SourceUri);
			}
			Uri uri = resolver.ResolveUri(baseUri, relativeUri);
			return (!(uri != null)) ? string.Empty : uri.OriginalString;
		}

		private void ProcessExternal(ValidationEventHandler handler, Hashtable handledUris, XmlResolver resolver, XmlSchemaExternal ext, XmlSchemaSet col)
		{
			if (ext == null)
			{
				error(handler, string.Format("Object of Type {0} is not valid in Includes Property of XmlSchema", ext.GetType().Name));
				return;
			}
			XmlSchemaImport xmlSchemaImport = ext as XmlSchemaImport;
			if (ext.SchemaLocation == null && xmlSchemaImport == null)
			{
				return;
			}
			XmlSchema xmlSchema = null;
			if (ext.SchemaLocation != null)
			{
				Stream stream = null;
				string text = null;
				if (resolver != null)
				{
					text = GetResolvedUri(resolver, ext.SchemaLocation);
					if (handledUris.Contains(text))
					{
						return;
					}
					handledUris.Add(text, text);
					try
					{
						stream = resolver.GetEntity(new Uri(text), null, typeof(Stream)) as Stream;
					}
					catch (Exception)
					{
						warn(handler, "Could not resolve schema location URI: " + text);
						stream = null;
					}
				}
				XmlSchemaRedefine xmlSchemaRedefine = ext as XmlSchemaRedefine;
				if (xmlSchemaRedefine != null)
				{
					for (int i = 0; i < xmlSchemaRedefine.Items.Count; i++)
					{
						XmlSchemaObject xmlSchemaObject = xmlSchemaRedefine.Items[i];
						xmlSchemaObject.isRedefinedComponent = true;
						xmlSchemaObject.isRedefineChild = true;
						if (xmlSchemaObject is XmlSchemaType || xmlSchemaObject is XmlSchemaGroup || xmlSchemaObject is XmlSchemaAttributeGroup)
						{
							compilationItems.Add(xmlSchemaObject);
						}
						else
						{
							error(handler, "Redefinition is only allowed to simpleType, complexType, group and attributeGroup.");
						}
					}
				}
				if (stream == null)
				{
					missedSubComponents = true;
					return;
				}
				XmlTextReader xmlTextReader = null;
				try
				{
					xmlTextReader = new XmlTextReader(text, stream, nameTable);
					xmlSchema = Read(xmlTextReader, handler);
				}
				finally
				{
					if (xmlTextReader != null)
					{
						xmlTextReader.Close();
					}
				}
				xmlSchema.schemas = schemas;
				xmlSchema.SetParent();
				ext.Schema = xmlSchema;
			}
			if (xmlSchemaImport != null)
			{
				if (ext.Schema == null && ext.SchemaLocation == null)
				{
					foreach (XmlSchema item in col.Schemas())
					{
						if (item.TargetNamespace == xmlSchemaImport.Namespace)
						{
							xmlSchema = item;
							xmlSchema.schemas = schemas;
							xmlSchema.SetParent();
							ext.Schema = xmlSchema;
							break;
						}
					}
					if (xmlSchema == null)
					{
						return;
					}
				}
				else if (xmlSchema != null)
				{
					if (TargetNamespace == xmlSchema.TargetNamespace)
					{
						error(handler, "Target namespace must be different from that of included schema.");
						return;
					}
					if (xmlSchema.TargetNamespace != xmlSchemaImport.Namespace)
					{
						error(handler, "Attribute namespace and its importing schema's target namespace must be the same.");
						return;
					}
				}
			}
			else if (xmlSchema != null)
			{
				if (TargetNamespace == null && xmlSchema.TargetNamespace != null)
				{
					error(handler, "Target namespace is required to include a schema which has its own target namespace");
					return;
				}
				if (TargetNamespace != null && xmlSchema.TargetNamespace == null)
				{
					xmlSchema.TargetNamespace = TargetNamespace;
				}
			}
			if (xmlSchema != null)
			{
				AddExternalComponentsTo(xmlSchema, compilationItems, handler, handledUris, resolver, col);
			}
		}

		private void AddExternalComponentsTo(XmlSchema s, XmlSchemaObjectCollection items, ValidationEventHandler handler, Hashtable handledUris, XmlResolver resolver, XmlSchemaSet col)
		{
			foreach (XmlSchemaExternal include in s.Includes)
			{
				ProcessExternal(handler, handledUris, resolver, include, col);
			}
			foreach (XmlSchemaObject item in s.Items)
			{
				items.Add(item);
			}
		}

		internal bool IsNamespaceAbsent(string ns)
		{
			return !schemas.Contains(ns);
		}

		internal XmlSchemaAttribute FindAttribute(XmlQualifiedName name)
		{
			foreach (XmlSchema item in schemas.Schemas())
			{
				XmlSchemaAttribute xmlSchemaAttribute = item.Attributes[name] as XmlSchemaAttribute;
				if (xmlSchemaAttribute != null)
				{
					return xmlSchemaAttribute;
				}
			}
			return null;
		}

		internal XmlSchemaAttributeGroup FindAttributeGroup(XmlQualifiedName name)
		{
			foreach (XmlSchema item in schemas.Schemas())
			{
				XmlSchemaAttributeGroup xmlSchemaAttributeGroup = item.AttributeGroups[name] as XmlSchemaAttributeGroup;
				if (xmlSchemaAttributeGroup != null)
				{
					return xmlSchemaAttributeGroup;
				}
			}
			return null;
		}

		internal XmlSchemaElement FindElement(XmlQualifiedName name)
		{
			foreach (XmlSchema item in schemas.Schemas())
			{
				XmlSchemaElement xmlSchemaElement = item.Elements[name] as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					return xmlSchemaElement;
				}
			}
			return null;
		}

		internal XmlSchemaType FindSchemaType(XmlQualifiedName name)
		{
			foreach (XmlSchema item in schemas.Schemas())
			{
				XmlSchemaType xmlSchemaType = item.SchemaTypes[name] as XmlSchemaType;
				if (xmlSchemaType != null)
				{
					return xmlSchemaType;
				}
			}
			return null;
		}

		internal void Validate(ValidationEventHandler handler)
		{
			ValidationId = CompilationId;
			foreach (XmlSchemaAttribute value in Attributes.Values)
			{
				errorCount += value.Validate(handler, this);
			}
			foreach (XmlSchemaAttributeGroup value2 in AttributeGroups.Values)
			{
				errorCount += value2.Validate(handler, this);
			}
			foreach (XmlSchemaType value3 in SchemaTypes.Values)
			{
				errorCount += value3.Validate(handler, this);
			}
			foreach (XmlSchemaElement value4 in Elements.Values)
			{
				errorCount += value4.Validate(handler, this);
			}
			foreach (XmlSchemaGroup value5 in Groups.Values)
			{
				errorCount += value5.Validate(handler, this);
			}
			foreach (XmlSchemaNotation value6 in Notations.Values)
			{
				errorCount += value6.Validate(handler, this);
			}
			if (errorCount == 0)
			{
				isCompiled = true;
			}
			errorCount = 0;
		}

		public static XmlSchema Read(TextReader reader, ValidationEventHandler validationEventHandler)
		{
			return Read(new XmlTextReader(reader), validationEventHandler);
		}

		public static XmlSchema Read(Stream stream, ValidationEventHandler validationEventHandler)
		{
			return Read(new XmlTextReader(stream), validationEventHandler);
		}

		public static XmlSchema Read(XmlReader rdr, ValidationEventHandler validationEventHandler)
		{
			XmlSchemaReader xmlSchemaReader = new XmlSchemaReader(rdr, validationEventHandler);
			if (xmlSchemaReader.ReadState == ReadState.Initial)
			{
				xmlSchemaReader.ReadNextElement();
			}
			int depth = xmlSchemaReader.Depth;
			do
			{
				XmlNodeType nodeType = xmlSchemaReader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (xmlSchemaReader.LocalName == "schema")
					{
						XmlSchema xmlSchema = new XmlSchema();
						xmlSchema.nameTable = rdr.NameTable;
						xmlSchema.LineNumber = xmlSchemaReader.LineNumber;
						xmlSchema.LinePosition = xmlSchemaReader.LinePosition;
						xmlSchema.SourceUri = xmlSchemaReader.BaseURI;
						ReadAttributes(xmlSchema, xmlSchemaReader, validationEventHandler);
						xmlSchemaReader.MoveToElement();
						if (!xmlSchemaReader.IsEmptyElement)
						{
							ReadContent(xmlSchema, xmlSchemaReader, validationEventHandler);
						}
						else
						{
							rdr.Skip();
						}
						return xmlSchema;
					}
					XmlSchemaObject.error(validationEventHandler, "The root element must be schema", null);
				}
				else
				{
					XmlSchemaObject.error(validationEventHandler, "This should never happen. XmlSchema.Read 1 ", null);
				}
			}
			while (xmlSchemaReader.Depth > depth && xmlSchemaReader.ReadNextElement());
			throw new XmlSchemaException("The top level schema must have namespace http://www.w3.org/2001/XMLSchema", null);
		}

		private static void ReadAttributes(XmlSchema schema, XmlSchemaReader reader, ValidationEventHandler h)
		{
			reader.MoveToElement();
			while (reader.MoveToNextAttribute())
			{
				Exception innerExcpetion;
				switch (reader.Name)
				{
				case "attributeFormDefault":
					schema.attributeFormDefault = XmlSchemaUtil.ReadFormAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for attributeFormDefault.", innerExcpetion);
					}
					break;
				case "blockDefault":
					schema.blockDefault = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerExcpetion, "blockDefault", XmlSchemaUtil.ElementBlockAllowed);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, innerExcpetion.Message, innerExcpetion);
					}
					break;
				case "elementFormDefault":
					schema.elementFormDefault = XmlSchemaUtil.ReadFormAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for elementFormDefault.", innerExcpetion);
					}
					break;
				case "finalDefault":
					schema.finalDefault = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerExcpetion, "finalDefault", XmlSchemaUtil.FinalAllowed);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, innerExcpetion.Message, innerExcpetion);
					}
					break;
				case "id":
					schema.id = reader.Value;
					break;
				case "targetNamespace":
					schema.targetNamespace = reader.Value;
					break;
				case "version":
					schema.version = reader.Value;
					break;
				default:
					if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
					{
						XmlSchemaObject.error(h, reader.Name + " attribute is not allowed in schema element", null);
					}
					else
					{
						XmlSchemaUtil.ReadUnhandledAttribute(reader, schema);
					}
					break;
				}
			}
		}

		private static void ReadContent(XmlSchema schema, XmlSchemaReader reader, ValidationEventHandler h)
		{
			reader.MoveToElement();
			if (reader.LocalName != "schema" && reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" && reader.NodeType != XmlNodeType.Element)
			{
				XmlSchemaObject.error(h, "UNREACHABLE CODE REACHED: Method: Schema.ReadContent, " + reader.LocalName + ", " + reader.NamespaceURI, null);
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "schema")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchema.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1)
				{
					if (reader.LocalName == "include")
					{
						XmlSchemaInclude xmlSchemaInclude = XmlSchemaInclude.Read(reader, h);
						if (xmlSchemaInclude != null)
						{
							schema.includes.Add(xmlSchemaInclude);
						}
						continue;
					}
					if (reader.LocalName == "import")
					{
						XmlSchemaImport xmlSchemaImport = XmlSchemaImport.Read(reader, h);
						if (xmlSchemaImport != null)
						{
							schema.includes.Add(xmlSchemaImport);
						}
						continue;
					}
					if (reader.LocalName == "redefine")
					{
						XmlSchemaRedefine xmlSchemaRedefine = XmlSchemaRedefine.Read(reader, h);
						if (xmlSchemaRedefine != null)
						{
							schema.includes.Add(xmlSchemaRedefine);
						}
						continue;
					}
					if (reader.LocalName == "annotation")
					{
						XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
						if (xmlSchemaAnnotation != null)
						{
							schema.items.Add(xmlSchemaAnnotation);
						}
						continue;
					}
				}
				if (num <= 2)
				{
					num = 2;
					if (reader.LocalName == "simpleType")
					{
						XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
						if (xmlSchemaSimpleType != null)
						{
							schema.items.Add(xmlSchemaSimpleType);
						}
						continue;
					}
					if (reader.LocalName == "complexType")
					{
						XmlSchemaComplexType xmlSchemaComplexType = XmlSchemaComplexType.Read(reader, h);
						if (xmlSchemaComplexType != null)
						{
							schema.items.Add(xmlSchemaComplexType);
						}
						continue;
					}
					if (reader.LocalName == "group")
					{
						XmlSchemaGroup xmlSchemaGroup = XmlSchemaGroup.Read(reader, h);
						if (xmlSchemaGroup != null)
						{
							schema.items.Add(xmlSchemaGroup);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						XmlSchemaAttributeGroup xmlSchemaAttributeGroup = XmlSchemaAttributeGroup.Read(reader, h);
						if (xmlSchemaAttributeGroup != null)
						{
							schema.items.Add(xmlSchemaAttributeGroup);
						}
						continue;
					}
					if (reader.LocalName == "element")
					{
						XmlSchemaElement xmlSchemaElement = XmlSchemaElement.Read(reader, h);
						if (xmlSchemaElement != null)
						{
							schema.items.Add(xmlSchemaElement);
						}
						continue;
					}
					if (reader.LocalName == "attribute")
					{
						XmlSchemaAttribute xmlSchemaAttribute = XmlSchemaAttribute.Read(reader, h);
						if (xmlSchemaAttribute != null)
						{
							schema.items.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "notation")
					{
						XmlSchemaNotation xmlSchemaNotation = XmlSchemaNotation.Read(reader, h);
						if (xmlSchemaNotation != null)
						{
							schema.items.Add(xmlSchemaNotation);
						}
						continue;
					}
					if (reader.LocalName == "annotation")
					{
						XmlSchemaAnnotation xmlSchemaAnnotation2 = XmlSchemaAnnotation.Read(reader, h);
						if (xmlSchemaAnnotation2 != null)
						{
							schema.items.Add(xmlSchemaAnnotation2);
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
		}

		public void Write(Stream stream)
		{
			Write(stream, null);
		}

		public void Write(TextWriter writer)
		{
			Write(writer, null);
		}

		public void Write(XmlWriter writer)
		{
			Write(writer, null);
		}

		public void Write(Stream stream, XmlNamespaceManager namespaceManager)
		{
			Write(new XmlTextWriter(stream, null), namespaceManager);
		}

		public void Write(TextWriter writer, XmlNamespaceManager namespaceManager)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			xmlTextWriter.Formatting = Formatting.Indented;
			Write(xmlTextWriter, namespaceManager);
		}

		public void Write(XmlWriter writer, XmlNamespaceManager namespaceManager)
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			if (namespaceManager != null)
			{
				foreach (string item in namespaceManager)
				{
					if (item != "xml" && item != "xmlns")
					{
						xmlSerializerNamespaces.Add(item, namespaceManager.LookupNamespace(item));
					}
				}
			}
			if (base.Namespaces != null && base.Namespaces.Count > 0)
			{
				XmlQualifiedName[] array = base.Namespaces.ToArray();
				XmlQualifiedName[] array2 = array;
				foreach (XmlQualifiedName xmlQualifiedName in array2)
				{
					xmlSerializerNamespaces.Add(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
				}
				string text2 = string.Empty;
				bool flag = true;
				int num = 1;
				while (flag)
				{
					flag = false;
					XmlQualifiedName[] array3 = array;
					foreach (XmlQualifiedName xmlQualifiedName2 in array3)
					{
						if (xmlQualifiedName2.Name == text2)
						{
							text2 = "q" + num;
							flag = true;
							break;
						}
					}
					num++;
				}
				xmlSerializerNamespaces.Add(text2, "http://www.w3.org/2001/XMLSchema");
			}
			if (xmlSerializerNamespaces.Count == 0)
			{
				xmlSerializerNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
				if (TargetNamespace != null && TargetNamespace.Length != 0)
				{
					xmlSerializerNamespaces.Add("tns", TargetNamespace);
				}
			}
			XmlSchemaSerializer xmlSchemaSerializer = new XmlSchemaSerializer();
			XmlSerializerNamespaces xmlSerializerNamespaces2 = base.Namespaces;
			try
			{
				base.Namespaces = null;
				xmlSchemaSerializer.Serialize(writer, this, xmlSerializerNamespaces);
			}
			finally
			{
				base.Namespaces = xmlSerializerNamespaces2;
			}
			writer.Flush();
		}
	}
}
