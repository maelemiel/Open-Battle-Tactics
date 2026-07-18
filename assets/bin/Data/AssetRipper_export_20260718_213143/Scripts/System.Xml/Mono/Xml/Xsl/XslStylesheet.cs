using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class XslStylesheet
	{
		public const string XsltNamespace = "http://www.w3.org/1999/XSL/Transform";

		public const string MSXsltNamespace = "urn:schemas-microsoft-com:xslt";

		private ArrayList imports = new ArrayList();

		private Hashtable spaceControls = new Hashtable();

		private NameValueCollection namespaceAliases = new NameValueCollection();

		private Hashtable parameters = new Hashtable();

		private Hashtable keys = new Hashtable();

		private Hashtable variables = new Hashtable();

		private XslTemplateTable templates;

		private string baseURI;

		private string version;

		private XmlQualifiedName[] extensionElementPrefixes;

		private XmlQualifiedName[] excludeResultPrefixes;

		private ArrayList stylesheetNamespaces = new ArrayList();

		private Hashtable inProcessIncludes = new Hashtable();

		private bool countedSpaceControlExistence;

		private bool cachedHasSpaceControls;

		private static readonly XmlQualifiedName allMatchName = new XmlQualifiedName("*");

		private bool countedNamespaceAliases;

		private bool cachedHasNamespaceAliases;

		public XmlQualifiedName[] ExtensionElementPrefixes
		{
			get
			{
				return extensionElementPrefixes;
			}
		}

		public XmlQualifiedName[] ExcludeResultPrefixes
		{
			get
			{
				return excludeResultPrefixes;
			}
		}

		public ArrayList StylesheetNamespaces
		{
			get
			{
				return stylesheetNamespaces;
			}
		}

		public ArrayList Imports
		{
			get
			{
				return imports;
			}
		}

		public Hashtable SpaceControls
		{
			get
			{
				return spaceControls;
			}
		}

		public NameValueCollection NamespaceAliases
		{
			get
			{
				return namespaceAliases;
			}
		}

		public Hashtable Parameters
		{
			get
			{
				return parameters;
			}
		}

		public XslTemplateTable Templates
		{
			get
			{
				return templates;
			}
		}

		public string BaseURI
		{
			get
			{
				return baseURI;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
		}

		public bool HasSpaceControls
		{
			get
			{
				if (!countedSpaceControlExistence)
				{
					countedSpaceControlExistence = true;
					cachedHasSpaceControls = ComputeHasSpaceControls();
				}
				return cachedHasSpaceControls;
			}
		}

		public bool HasNamespaceAliases
		{
			get
			{
				if (!countedNamespaceAliases)
				{
					countedNamespaceAliases = true;
					if (namespaceAliases.Count > 0)
					{
						cachedHasNamespaceAliases = true;
					}
					else if (imports.Count == 0)
					{
						cachedHasNamespaceAliases = false;
					}
					else
					{
						for (int i = 0; i < imports.Count; i++)
						{
							if (((XslStylesheet)imports[i]).namespaceAliases.Count > 0)
							{
								countedNamespaceAliases = true;
							}
						}
						cachedHasNamespaceAliases = false;
					}
				}
				return cachedHasNamespaceAliases;
			}
		}

		internal void Compile(Compiler c)
		{
			c.PushStylesheet(this);
			templates = new XslTemplateTable(this);
			baseURI = c.Input.BaseURI;
			while (c.Input.NodeType != XPathNodeType.Element)
			{
				if (!c.Input.MoveToNext())
				{
					throw new XsltCompileException("Stylesheet root element must be either \"stylesheet\" or \"transform\" or any literal element", null, c.Input);
				}
			}
			if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
			{
				if (c.Input.GetAttribute("version", "http://www.w3.org/1999/XSL/Transform") == string.Empty)
				{
					throw new XsltCompileException("Mandatory global attribute version is missing", null, c.Input);
				}
				templates.Add(new XslTemplate(c));
			}
			else
			{
				if (c.Input.LocalName != "stylesheet" && c.Input.LocalName != "transform")
				{
					throw new XsltCompileException("Stylesheet root element must be either \"stylesheet\" or \"transform\" or any literal element", null, c.Input);
				}
				version = c.Input.GetAttribute("version", string.Empty);
				if (version == string.Empty)
				{
					throw new XsltCompileException("Mandatory attribute version is missing", null, c.Input);
				}
				extensionElementPrefixes = ParseMappedPrefixes(c.GetAttribute("extension-element-prefixes"), c.Input);
				excludeResultPrefixes = ParseMappedPrefixes(c.GetAttribute("exclude-result-prefixes"), c.Input);
				if (c.Input.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					do
					{
						if (!(c.Input.Value == "http://www.w3.org/1999/XSL/Transform"))
						{
							stylesheetNamespaces.Insert(0, new XmlQualifiedName(c.Input.Name, c.Input.Value));
						}
					}
					while (c.Input.MoveToNextNamespace(XPathNamespaceScope.Local));
					c.Input.MoveToParent();
				}
				ProcessTopLevelElements(c);
			}
			foreach (XslGlobalVariable value in variables.Values)
			{
				c.AddGlobalVariable(value);
			}
			foreach (ArrayList value2 in keys.Values)
			{
				for (int i = 0; i < value2.Count; i++)
				{
					c.AddKey((XslKey)value2[i]);
				}
			}
			c.PopStylesheet();
			inProcessIncludes = null;
		}

		private XmlQualifiedName[] ParseMappedPrefixes(string list, XPathNavigator nav)
		{
			if (list == null)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			string[] array = list.Split(XmlChar.WhitespaceChars);
			foreach (string text in array)
			{
				if (text.Length == 0)
				{
					continue;
				}
				if (text == "#default")
				{
					arrayList.Add(new XmlQualifiedName(string.Empty, string.Empty));
					continue;
				}
				string text2 = nav.GetNamespace(text);
				if (text2 != string.Empty)
				{
					arrayList.Add(new XmlQualifiedName(text, text2));
				}
			}
			return (XmlQualifiedName[])arrayList.ToArray(typeof(XmlQualifiedName));
		}

		private bool ComputeHasSpaceControls()
		{
			if (spaceControls.Count > 0 && HasStripSpace(spaceControls))
			{
				return true;
			}
			if (imports.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < imports.Count; i++)
			{
				XslStylesheet xslStylesheet = (XslStylesheet)imports[i];
				if (xslStylesheet.spaceControls.Count > 0 && HasStripSpace(xslStylesheet.spaceControls))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasStripSpace(IDictionary table)
		{
			foreach (int value in table.Values)
			{
				if (value == 1)
				{
					return true;
				}
			}
			return false;
		}

		public bool GetPreserveWhitespace(XPathNavigator nav)
		{
			if (!HasSpaceControls)
			{
				return true;
			}
			nav = nav.Clone();
			if (!nav.MoveToParent() || nav.NodeType != XPathNodeType.Element)
			{
				object defaultXmlSpace = GetDefaultXmlSpace();
				return defaultXmlSpace == null || (int)defaultXmlSpace == 2;
			}
			string localName = nav.LocalName;
			string namespaceURI = nav.NamespaceURI;
			XmlQualifiedName key = new XmlQualifiedName(localName, namespaceURI);
			object obj = spaceControls[key];
			if (obj == null)
			{
				for (int i = 0; i < imports.Count; i++)
				{
					obj = ((XslStylesheet)imports[i]).SpaceControls[key];
					if (obj != null)
					{
						break;
					}
				}
			}
			if (obj == null)
			{
				key = new XmlQualifiedName("*", namespaceURI);
				obj = spaceControls[key];
				if (obj == null)
				{
					for (int j = 0; j < imports.Count; j++)
					{
						obj = ((XslStylesheet)imports[j]).SpaceControls[key];
						if (obj != null)
						{
							break;
						}
					}
				}
			}
			if (obj == null)
			{
				obj = GetDefaultXmlSpace();
			}
			if (obj != null)
			{
				switch ((XmlSpace)(int)obj)
				{
				case XmlSpace.Preserve:
					return true;
				case XmlSpace.Default:
					return false;
				}
			}
			throw new SystemException("Mono BUG: should not reach here");
		}

		private object GetDefaultXmlSpace()
		{
			object obj = spaceControls[allMatchName];
			if (obj == null)
			{
				for (int i = 0; i < imports.Count; i++)
				{
					obj = ((XslStylesheet)imports[i]).SpaceControls[allMatchName];
					if (obj != null)
					{
						break;
					}
				}
			}
			return obj;
		}

		public string GetActualPrefix(string prefix)
		{
			if (!HasNamespaceAliases)
			{
				return prefix;
			}
			string text = namespaceAliases[prefix];
			if (text == null)
			{
				for (int i = 0; i < imports.Count; i++)
				{
					text = ((XslStylesheet)imports[i]).namespaceAliases[prefix];
					if (text != null)
					{
						break;
					}
				}
			}
			return (text == null) ? prefix : text;
		}

		private void StoreInclude(Compiler c)
		{
			XPathNavigator key = c.Input.Clone();
			c.PushInputDocument(c.Input.GetAttribute("href", string.Empty));
			inProcessIncludes[key] = c.Input;
			HandleImportsInInclude(c);
			c.PopInputDocument();
		}

		private void HandleImportsInInclude(Compiler c)
		{
			if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
			{
				if (c.Input.GetAttribute("version", "http://www.w3.org/1999/XSL/Transform") == string.Empty)
				{
					throw new XsltCompileException("Mandatory global attribute version is missing", null, c.Input);
				}
			}
			else if (!c.Input.MoveToFirstChild())
			{
				c.Input.MoveToRoot();
			}
			else
			{
				HandleIncludesImports(c);
			}
		}

		private void HandleInclude(Compiler c)
		{
			XPathNavigator xPathNavigator = null;
			foreach (XPathNavigator key in inProcessIncludes.Keys)
			{
				if (key.IsSamePosition(c.Input))
				{
					xPathNavigator = (XPathNavigator)inProcessIncludes[key];
					break;
				}
			}
			if (xPathNavigator == null)
			{
				throw new Exception("Should not happen. Current input is " + c.Input.BaseURI + " / " + c.Input.Name + ", " + inProcessIncludes.Count);
			}
			if (xPathNavigator.NodeType == XPathNodeType.Root)
			{
				return;
			}
			c.PushInputDocument(xPathNavigator);
			while (c.Input.NodeType != XPathNodeType.Element && c.Input.MoveToNext())
			{
			}
			if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform" && c.Input.NodeType == XPathNodeType.Element)
			{
				templates.Add(new XslTemplate(c));
			}
			else
			{
				do
				{
					if (c.Input.NodeType == XPathNodeType.Element)
					{
						HandleTopLevelElement(c);
					}
				}
				while (c.Input.MoveToNext());
			}
			c.Input.MoveToParent();
			c.PopInputDocument();
		}

		private void HandleImport(Compiler c, string href)
		{
			c.PushInputDocument(href);
			XslStylesheet xslStylesheet = new XslStylesheet();
			xslStylesheet.Compile(c);
			imports.Add(xslStylesheet);
			c.PopInputDocument();
		}

		private void HandleTopLevelElement(Compiler c)
		{
			XPathNavigator input = c.Input;
			switch (input.NamespaceURI)
			{
			case "http://www.w3.org/1999/XSL/Transform":
				switch (input.LocalName)
				{
				case "include":
					HandleInclude(c);
					break;
				case "preserve-space":
					AddSpaceControls(c.ParseQNameListAttribute("elements"), XmlSpace.Preserve, input);
					break;
				case "strip-space":
					AddSpaceControls(c.ParseQNameListAttribute("elements"), XmlSpace.Default, input);
					break;
				case "namespace-alias":
					break;
				case "attribute-set":
					c.AddAttributeSet(new XslAttributeSet(c));
					break;
				case "key":
				{
					XslKey xslKey = new XslKey(c);
					if (keys[xslKey.Name] == null)
					{
						keys[xslKey.Name] = new ArrayList();
					}
					((ArrayList)keys[xslKey.Name]).Add(xslKey);
					break;
				}
				case "output":
					c.CompileOutput();
					break;
				case "decimal-format":
					c.CompileDecimalFormat();
					break;
				case "template":
					templates.Add(new XslTemplate(c));
					break;
				case "variable":
				{
					XslGlobalVariable xslGlobalVariable = new XslGlobalVariable(c);
					variables[xslGlobalVariable.Name] = xslGlobalVariable;
					break;
				}
				case "param":
				{
					XslGlobalParam xslGlobalParam = new XslGlobalParam(c);
					variables[xslGlobalParam.Name] = xslGlobalParam;
					break;
				}
				default:
					if (version == "1.0")
					{
						throw new XsltCompileException("Unrecognized top level element after imports", null, c.Input);
					}
					break;
				}
				break;
			case "urn:schemas-microsoft-com:xslt":
				switch (input.LocalName)
				{
				case "script":
					c.ScriptManager.AddScript(c);
					break;
				}
				break;
			}
		}

		private XPathNavigator HandleIncludesImports(Compiler c)
		{
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element)
				{
					if (c.Input.LocalName != "import" || c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
					{
						break;
					}
					HandleImport(c, c.GetAttribute("href"));
				}
			}
			while (c.Input.MoveToNext());
			XPathNavigator xPathNavigator = c.Input.Clone();
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element && !(c.Input.LocalName != "include") && !(c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform"))
				{
					StoreInclude(c);
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveTo(xPathNavigator);
			return xPathNavigator;
		}

		private void ProcessTopLevelElements(Compiler c)
		{
			if (!c.Input.MoveToFirstChild())
			{
				return;
			}
			XPathNavigator other = HandleIncludesImports(c);
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element && !(c.Input.LocalName != "namespace-alias") && !(c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform"))
				{
					string text = c.GetAttribute("stylesheet-prefix", string.Empty);
					if (text == "#default")
					{
						text = string.Empty;
					}
					string text2 = c.GetAttribute("result-prefix", string.Empty);
					if (text2 == "#default")
					{
						text2 = string.Empty;
					}
					namespaceAliases.Set(text, text2);
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveTo(other);
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element)
				{
					HandleTopLevelElement(c);
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveToParent();
		}

		private void AddSpaceControls(XmlQualifiedName[] names, XmlSpace result, XPathNavigator styleElem)
		{
			foreach (XmlQualifiedName key in names)
			{
				spaceControls[key] = result;
			}
		}
	}
}
