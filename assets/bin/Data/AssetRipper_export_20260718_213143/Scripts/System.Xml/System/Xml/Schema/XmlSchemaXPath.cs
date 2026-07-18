using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaXPath : XmlSchemaAnnotated
	{
		private string xpath;

		private XmlNamespaceManager nsmgr;

		internal bool isSelector;

		private XsdIdentityPath[] compiledExpression;

		private XsdIdentityPath currentPath;

		[XmlAttribute("xpath")]
		[DefaultValue("")]
		public string XPath
		{
			get
			{
				return xpath;
			}
			set
			{
				xpath = value;
			}
		}

		internal XsdIdentityPath[] CompiledExpression
		{
			get
			{
				return compiledExpression;
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			if (nsmgr == null)
			{
				nsmgr = new XmlNamespaceManager(new NameTable());
				if (base.Namespaces != null)
				{
					XmlQualifiedName[] array = base.Namespaces.ToArray();
					foreach (XmlQualifiedName xmlQualifiedName in array)
					{
						nsmgr.AddNamespace(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
					}
				}
			}
			currentPath = new XsdIdentityPath();
			ParseExpression(xpath, h, schema);
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		private void ParseExpression(string xpath, ValidationEventHandler h, XmlSchema schema)
		{
			ArrayList arrayList = new ArrayList();
			ParsePath(xpath, 0, arrayList, h, schema);
			compiledExpression = (XsdIdentityPath[])arrayList.ToArray(typeof(XsdIdentityPath));
		}

		private void ParsePath(string xpath, int pos, ArrayList paths, ValidationEventHandler h, XmlSchema schema)
		{
			pos = SkipWhitespace(xpath, pos);
			if (xpath.Length >= pos + 3 && xpath[pos] == '.')
			{
				int num = pos;
				pos++;
				pos = SkipWhitespace(xpath, pos);
				if (xpath.Length > pos + 2 && xpath.IndexOf("//", pos, 2) == pos)
				{
					currentPath.Descendants = true;
					pos += 2;
				}
				else
				{
					pos = num;
				}
			}
			ArrayList steps = new ArrayList();
			ParseStep(xpath, pos, steps, paths, h, schema);
		}

		private void ParseStep(string xpath, int pos, ArrayList steps, ArrayList paths, ValidationEventHandler h, XmlSchema schema)
		{
			pos = SkipWhitespace(xpath, pos);
			if (xpath.Length == pos)
			{
				error(h, "Empty xpath expression is specified");
				return;
			}
			XsdIdentityStep xsdIdentityStep = new XsdIdentityStep();
			switch (xpath[pos])
			{
			case '@':
				if (isSelector)
				{
					error(h, "Selector cannot include attribute axes.");
					currentPath = null;
					return;
				}
				pos++;
				xsdIdentityStep.IsAttribute = true;
				pos = SkipWhitespace(xpath, pos);
				if (xpath.Length > pos && xpath[pos] == '*')
				{
					pos++;
					xsdIdentityStep.IsAnyName = true;
					break;
				}
				goto default;
			case '.':
				pos++;
				xsdIdentityStep.IsCurrent = true;
				break;
			case '*':
				pos++;
				xsdIdentityStep.IsAnyName = true;
				break;
			case 'c':
				if (xpath.Length > pos + 5 && xpath.IndexOf("child", pos, 5) == pos)
				{
					int num2 = pos;
					pos += 5;
					pos = SkipWhitespace(xpath, pos);
					if (xpath.Length > pos && xpath[pos] == ':' && xpath[pos + 1] == ':')
					{
						pos += 2;
						if (xpath.Length > pos && xpath[pos] == '*')
						{
							pos++;
							xsdIdentityStep.IsAnyName = true;
							break;
						}
						pos = SkipWhitespace(xpath, pos);
					}
					else
					{
						pos = num2;
					}
				}
				goto default;
			case 'a':
				if (xpath.Length > pos + 9 && xpath.IndexOf("attribute", pos, 9) == pos)
				{
					int num = pos;
					pos += 9;
					pos = SkipWhitespace(xpath, pos);
					if (xpath.Length > pos && xpath[pos] == ':' && xpath[pos + 1] == ':')
					{
						if (isSelector)
						{
							error(h, "Selector cannot include attribute axes.");
							currentPath = null;
							return;
						}
						pos += 2;
						xsdIdentityStep.IsAttribute = true;
						if (xpath.Length > pos && xpath[pos] == '*')
						{
							pos++;
							xsdIdentityStep.IsAnyName = true;
							break;
						}
						pos = SkipWhitespace(xpath, pos);
					}
					else
					{
						pos = num;
					}
				}
				goto default;
			default:
			{
				int num3 = pos;
				while (xpath.Length > pos && XmlChar.IsNCNameChar(xpath[pos]))
				{
					pos++;
				}
				if (pos == num3)
				{
					error(h, "Invalid path format for a field.");
					currentPath = null;
					return;
				}
				if (xpath.Length == pos || xpath[pos] != ':')
				{
					xsdIdentityStep.Name = xpath.Substring(num3, pos - num3);
					break;
				}
				string text = xpath.Substring(num3, pos - num3);
				pos++;
				if (xpath.Length > pos && xpath[pos] == '*')
				{
					string text2 = nsmgr.LookupNamespace(text, false);
					if (text2 == null)
					{
						error(h, "Specified prefix '" + text + "' is not declared.");
						currentPath = null;
						return;
					}
					xsdIdentityStep.NsName = text2;
					pos++;
					break;
				}
				int num4 = pos;
				while (xpath.Length > pos && XmlChar.IsNCNameChar(xpath[pos]))
				{
					pos++;
				}
				xsdIdentityStep.Name = xpath.Substring(num4, pos - num4);
				string text3 = nsmgr.LookupNamespace(text, false);
				if (text3 == null)
				{
					error(h, "Specified prefix '" + text + "' is not declared.");
					currentPath = null;
					return;
				}
				xsdIdentityStep.Namespace = text3;
				break;
			}
			}
			if (!xsdIdentityStep.IsCurrent)
			{
				steps.Add(xsdIdentityStep);
			}
			pos = SkipWhitespace(xpath, pos);
			if (xpath.Length == pos)
			{
				currentPath.OrderedSteps = (XsdIdentityStep[])steps.ToArray(typeof(XsdIdentityStep));
				paths.Add(currentPath);
			}
			else if (xpath[pos] == '/')
			{
				pos++;
				if (xsdIdentityStep.IsAttribute)
				{
					error(h, "Unexpected xpath token after Attribute NameTest.");
					currentPath = null;
					return;
				}
				ParseStep(xpath, pos, steps, paths, h, schema);
				if (currentPath != null)
				{
					currentPath.OrderedSteps = (XsdIdentityStep[])steps.ToArray(typeof(XsdIdentityStep));
				}
			}
			else if (xpath[pos] == '|')
			{
				pos++;
				currentPath.OrderedSteps = (XsdIdentityStep[])steps.ToArray(typeof(XsdIdentityStep));
				paths.Add(currentPath);
				currentPath = new XsdIdentityPath();
				ParsePath(xpath, pos, paths, h, schema);
			}
			else
			{
				error(h, "Unexpected xpath token after NameTest.");
				currentPath = null;
			}
		}

		private int SkipWhitespace(string xpath, int pos)
		{
			bool flag = true;
			while (flag && xpath.Length > pos)
			{
				switch (xpath[pos])
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					pos++;
					break;
				default:
					flag = false;
					break;
				}
			}
			return pos;
		}

		internal static XmlSchemaXPath Read(XmlSchemaReader reader, ValidationEventHandler h, string name)
		{
			XmlSchemaXPath xmlSchemaXPath = new XmlSchemaXPath();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != name)
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContentRestriction.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaXPath.LineNumber = reader.LineNumber;
			xmlSchemaXPath.LinePosition = reader.LinePosition;
			xmlSchemaXPath.SourceUri = reader.BaseURI;
			XmlNamespaceManager namespaceManager = XmlSchemaUtil.GetParserContext(reader.Reader).NamespaceManager;
			if (namespaceManager != null)
			{
				xmlSchemaXPath.nsmgr = new XmlNamespaceManager(reader.NameTable);
				IEnumerator enumerator = namespaceManager.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text = enumerator.Current as string;
					switch (text)
					{
					case "xml":
					case "xmlns":
						continue;
					}
					xmlSchemaXPath.nsmgr.AddNamespace(text, namespaceManager.LookupNamespace(text, false));
				}
			}
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaXPath.Id = reader.Value;
				}
				else if (reader.Name == "xpath")
				{
					xmlSchemaXPath.xpath = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for " + name, null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaXPath);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaXPath;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != name)
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaXPath.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaXPath.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaXPath;
		}
	}
}
