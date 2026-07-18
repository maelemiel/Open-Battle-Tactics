using System;
using System.Collections;
using System.IO;
using System.Security.Policy;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class Compiler : IStaticXsltContext
	{
		public const string XsltNamespace = "http://www.w3.org/1999/XSL/Transform";

		private ArrayList inputStack = new ArrayList();

		private XPathNavigator currentInput;

		private Stack styleStack = new Stack();

		private XslStylesheet currentStyle;

		private Hashtable keys = new Hashtable();

		private Hashtable globalVariables = new Hashtable();

		private Hashtable attrSets = new Hashtable();

		private XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());

		private XmlResolver res;

		private Evidence evidence;

		private XslStylesheet rootStyle;

		private Hashtable outputs = new Hashtable();

		private bool keyCompilationMode;

		private string stylesheetVersion;

		private XsltDebuggerWrapper debugger;

		private MSXslScriptManager msScripts = new MSXslScriptManager();

		internal XPathParser xpathParser;

		internal XsltPatternParser patternParser;

		private VariableScope curVarScope;

		private Hashtable decimalFormats = new Hashtable();

		public XsltDebuggerWrapper Debugger
		{
			get
			{
				return debugger;
			}
		}

		public MSXslScriptManager ScriptManager
		{
			get
			{
				return msScripts;
			}
		}

		public bool KeyCompilationMode
		{
			get
			{
				return keyCompilationMode;
			}
			set
			{
				keyCompilationMode = value;
			}
		}

		internal Evidence Evidence
		{
			get
			{
				return evidence;
			}
		}

		public XPathNavigator Input
		{
			get
			{
				return currentInput;
			}
		}

		public XslStylesheet CurrentStylesheet
		{
			get
			{
				return currentStyle;
			}
		}

		public VariableScope CurrentVariableScope
		{
			get
			{
				return curVarScope;
			}
		}

		public Compiler(object debugger)
		{
			if (debugger != null)
			{
				this.debugger = new XsltDebuggerWrapper(debugger);
			}
		}

		Expression IStaticXsltContext.TryGetVariable(string nm)
		{
			if (curVarScope == null)
			{
				return null;
			}
			XslLocalVariable xslLocalVariable = curVarScope.ResolveStatic(XslNameUtil.FromString(nm, Input));
			if (xslLocalVariable == null)
			{
				return null;
			}
			return new XPathVariableBinding(xslLocalVariable);
		}

		Expression IStaticXsltContext.TryGetFunction(XmlQualifiedName name, FunctionArguments args)
		{
			string text = LookupNamespace(name.Namespace);
			if (text == "urn:schemas-microsoft-com:xslt" && name.Name == "node-set")
			{
				return new MSXslNodeSet(args);
			}
			if (text != string.Empty)
			{
				return null;
			}
			switch (name.Name)
			{
			case "current":
				return new XsltCurrent(args);
			case "unparsed-entity-uri":
				return new XsltUnparsedEntityUri(args);
			case "element-available":
				return new XsltElementAvailable(args, this);
			case "system-property":
				return new XsltSystemProperty(args, this);
			case "function-available":
				return new XsltFunctionAvailable(args, this);
			case "generate-id":
				return new XsltGenerateId(args);
			case "format-number":
				return new XsltFormatNumber(args, this);
			case "key":
				if (KeyCompilationMode)
				{
					throw new XsltCompileException("Cannot use key() function inside key definition", null, Input);
				}
				return new XsltKey(args, this);
			case "document":
				return new XsltDocument(args, this);
			default:
				return null;
			}
		}

		XmlQualifiedName IStaticXsltContext.LookupQName(string s)
		{
			return XslNameUtil.FromString(s, Input);
		}

		public void CheckExtraAttributes(string element, params string[] validNames)
		{
			if (!Input.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				if (Input.NamespaceURI.Length > 0)
				{
					continue;
				}
				bool flag = false;
				foreach (string text in validNames)
				{
					if (Input.LocalName == text)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					throw new XsltCompileException(string.Format("Invalid attribute '{0}' on element '{1}'", Input.LocalName, element), null, Input);
				}
			}
			while (Input.MoveToNextAttribute());
			Input.MoveToParent();
		}

		public CompiledStylesheet Compile(XPathNavigator nav, XmlResolver res, Evidence evidence)
		{
			xpathParser = new XPathParser(this);
			patternParser = new XsltPatternParser(this);
			this.res = res;
			if (res == null)
			{
				this.res = new XmlUrlResolver();
			}
			this.evidence = evidence;
			if (nav.NodeType == XPathNodeType.Root && !nav.MoveToFirstChild())
			{
				throw new XsltCompileException("Stylesheet root element must be either \"stylesheet\" or \"transform\" or any literal element", null, nav);
			}
			while (nav.NodeType != XPathNodeType.Element)
			{
				nav.MoveToNext();
			}
			stylesheetVersion = nav.GetAttribute("version", (!(nav.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")) ? string.Empty : "http://www.w3.org/1999/XSL/Transform");
			outputs[string.Empty] = new XslOutput(string.Empty, stylesheetVersion);
			PushInputDocument(nav);
			if (nav.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
			{
				do
				{
					nsMgr.AddNamespace(nav.LocalName, nav.Value);
				}
				while (nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
				nav.MoveToParent();
			}
			try
			{
				rootStyle = new XslStylesheet();
				rootStyle.Compile(this);
			}
			catch (XsltCompileException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new XsltCompileException("XSLT compile error. " + ex2.Message, ex2, Input);
			}
			return new CompiledStylesheet(rootStyle, globalVariables, attrSets, nsMgr, keys, outputs, decimalFormats, msScripts);
		}

		public void PushStylesheet(XslStylesheet style)
		{
			if (currentStyle != null)
			{
				styleStack.Push(currentStyle);
			}
			currentStyle = style;
		}

		public void PopStylesheet()
		{
			if (styleStack.Count == 0)
			{
				currentStyle = null;
			}
			else
			{
				currentStyle = (XslStylesheet)styleStack.Pop();
			}
		}

		public void PushInputDocument(string url)
		{
			Uri baseUri = ((!(Input.BaseURI == string.Empty)) ? new Uri(Input.BaseURI) : null);
			Uri uri = res.ResolveUri(baseUri, url);
			string url2 = ((!(uri != null)) ? string.Empty : uri.ToString());
			using (Stream stream = (Stream)res.GetEntity(uri, null, typeof(Stream)))
			{
				if (stream == null)
				{
					throw new XsltCompileException("Can not access URI " + uri.ToString(), null, Input);
				}
				XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(new XmlTextReader(url2, stream, nsMgr.NameTable));
				xmlValidatingReader.ValidationType = ValidationType.None;
				XPathNavigator xPathNavigator = new XPathDocument(xmlValidatingReader, XmlSpace.Preserve).CreateNavigator();
				xmlValidatingReader.Close();
				xPathNavigator.MoveToFirstChild();
				while (xPathNavigator.NodeType != XPathNodeType.Element && xPathNavigator.MoveToNext())
				{
				}
				PushInputDocument(xPathNavigator);
			}
		}

		public void PushInputDocument(XPathNavigator nav)
		{
			IXmlLineInfo xmlLineInfo = currentInput as IXmlLineInfo;
			bool flag = xmlLineInfo != null && !xmlLineInfo.HasLineInfo();
			for (int i = 0; i < inputStack.Count; i++)
			{
				XPathNavigator xPathNavigator = (XPathNavigator)inputStack[i];
				if (xPathNavigator.BaseURI == nav.BaseURI)
				{
					throw new XsltCompileException(null, currentInput.BaseURI, flag ? xmlLineInfo.LineNumber : 0, flag ? xmlLineInfo.LinePosition : 0);
				}
			}
			if (currentInput != null)
			{
				inputStack.Add(currentInput);
			}
			currentInput = nav;
		}

		public void PopInputDocument()
		{
			int index = inputStack.Count - 1;
			currentInput = (XPathNavigator)inputStack[index];
			inputStack.RemoveAt(index);
		}

		public XmlQualifiedName ParseQNameAttribute(string localName)
		{
			return ParseQNameAttribute(localName, string.Empty);
		}

		public XmlQualifiedName ParseQNameAttribute(string localName, string ns)
		{
			return XslNameUtil.FromString(Input.GetAttribute(localName, ns), Input);
		}

		public XmlQualifiedName[] ParseQNameListAttribute(string localName)
		{
			return ParseQNameListAttribute(localName, string.Empty);
		}

		public XmlQualifiedName[] ParseQNameListAttribute(string localName, string ns)
		{
			string attribute = GetAttribute(localName, ns);
			if (attribute == null)
			{
				return null;
			}
			string[] array = attribute.Split(' ', '\r', '\n', '\t');
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length != 0)
				{
					num++;
				}
			}
			XmlQualifiedName[] array2 = new XmlQualifiedName[num];
			int j = 0;
			int num2 = 0;
			for (; j < array.Length; j++)
			{
				if (array[j].Length != 0)
				{
					array2[num2++] = XslNameUtil.FromString(array[j], Input);
				}
			}
			return array2;
		}

		public bool ParseYesNoAttribute(string localName, bool defaultVal)
		{
			return ParseYesNoAttribute(localName, string.Empty, defaultVal);
		}

		public bool ParseYesNoAttribute(string localName, string ns, bool defaultVal)
		{
			switch (GetAttribute(localName, ns))
			{
			case null:
				return defaultVal;
			case "yes":
				return true;
			case "no":
				return false;
			default:
				throw new XsltCompileException("Invalid value for " + localName, null, Input);
			}
		}

		public string GetAttribute(string localName)
		{
			return GetAttribute(localName, string.Empty);
		}

		public string GetAttribute(string localName, string ns)
		{
			if (!Input.MoveToAttribute(localName, ns))
			{
				return null;
			}
			string value = Input.Value;
			Input.MoveToParent();
			return value;
		}

		public XslAvt ParseAvtAttribute(string localName)
		{
			return ParseAvtAttribute(localName, string.Empty);
		}

		public XslAvt ParseAvtAttribute(string localName, string ns)
		{
			return ParseAvt(GetAttribute(localName, ns));
		}

		public void AssertAttribute(string localName)
		{
			AssertAttribute(localName, string.Empty);
		}

		public void AssertAttribute(string localName, string ns)
		{
			if (Input.GetAttribute(localName, ns) == null)
			{
				throw new XsltCompileException("Was expecting the " + localName + " attribute", null, Input);
			}
		}

		public XslAvt ParseAvt(string s)
		{
			if (s == null)
			{
				return null;
			}
			return new XslAvt(s, this);
		}

		public Pattern CompilePattern(string pattern, XPathNavigator loc)
		{
			if (pattern == null || pattern == string.Empty)
			{
				return null;
			}
			Pattern pattern2 = Pattern.Compile(pattern, this);
			if (pattern2 == null)
			{
				throw new XsltCompileException(string.Format("Invalid pattern '{0}'", pattern), null, loc);
			}
			return pattern2;
		}

		internal CompiledExpression CompileExpression(string expression)
		{
			return CompileExpression(expression, false);
		}

		internal CompiledExpression CompileExpression(string expression, bool isKey)
		{
			if (expression == null || expression == string.Empty)
			{
				return null;
			}
			Expression expr = xpathParser.Compile(expression);
			if (isKey)
			{
				expr = new ExprKeyContainer(expr);
			}
			return new CompiledExpression(expression, expr);
		}

		public XslOperation CompileTemplateContent()
		{
			return CompileTemplateContent(XPathNodeType.All, false);
		}

		public XslOperation CompileTemplateContent(XPathNodeType parentType)
		{
			return CompileTemplateContent(parentType, false);
		}

		public XslOperation CompileTemplateContent(XPathNodeType parentType, bool xslForEach)
		{
			return new XslTemplateContent(this, parentType, xslForEach);
		}

		public void AddGlobalVariable(XslGlobalVariable var)
		{
			globalVariables[var.Name] = var;
		}

		public void AddKey(XslKey key)
		{
			if (keys[key.Name] == null)
			{
				keys[key.Name] = new ArrayList();
			}
			((ArrayList)keys[key.Name]).Add(key);
		}

		public void AddAttributeSet(XslAttributeSet set)
		{
			XslAttributeSet xslAttributeSet = attrSets[set.Name] as XslAttributeSet;
			if (xslAttributeSet != null)
			{
				xslAttributeSet.Merge(set);
				attrSets[set.Name] = xslAttributeSet;
			}
			else
			{
				attrSets[set.Name] = set;
			}
		}

		public void PushScope()
		{
			curVarScope = new VariableScope(curVarScope);
		}

		public VariableScope PopScope()
		{
			curVarScope.giveHighTideToParent();
			VariableScope result = curVarScope;
			curVarScope = curVarScope.Parent;
			return result;
		}

		public int AddVariable(XslLocalVariable v)
		{
			if (curVarScope == null)
			{
				throw new XsltCompileException("Not initialized variable", null, Input);
			}
			return curVarScope.AddVariable(v);
		}

		public bool IsExtensionNamespace(string nsUri)
		{
			if (nsUri == "http://www.w3.org/1999/XSL/Transform")
			{
				return true;
			}
			XPathNavigator xPathNavigator = Input.Clone();
			XPathNavigator xPathNavigator2 = xPathNavigator.Clone();
			do
			{
				bool flag = xPathNavigator.NamespaceURI == "http://www.w3.org/1999/XSL/Transform";
				xPathNavigator2.MoveTo(xPathNavigator);
				if (!xPathNavigator.MoveToFirstAttribute())
				{
					continue;
				}
				do
				{
					if (!(xPathNavigator.LocalName == "extension-element-prefixes") || !(xPathNavigator.NamespaceURI == ((!flag) ? "http://www.w3.org/1999/XSL/Transform" : string.Empty)))
					{
						continue;
					}
					string[] array = xPathNavigator.Value.Split(' ');
					foreach (string text in array)
					{
						if (xPathNavigator2.GetNamespace((!(text == "#default")) ? text : string.Empty) == nsUri)
						{
							return true;
						}
					}
				}
				while (xPathNavigator.MoveToNextAttribute());
				xPathNavigator.MoveToParent();
			}
			while (xPathNavigator.MoveToParent());
			return false;
		}

		public Hashtable GetNamespacesToCopy()
		{
			Hashtable hashtable = new Hashtable();
			XPathNavigator xPathNavigator = Input.Clone();
			XPathNavigator xPathNavigator2 = xPathNavigator.Clone();
			if (xPathNavigator.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
			{
				do
				{
					if (xPathNavigator.Value != "http://www.w3.org/1999/XSL/Transform" && !hashtable.Contains(xPathNavigator.Name))
					{
						hashtable.Add(xPathNavigator.Name, xPathNavigator.Value);
					}
				}
				while (xPathNavigator.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
				xPathNavigator.MoveToParent();
			}
			do
			{
				bool flag = xPathNavigator.NamespaceURI == "http://www.w3.org/1999/XSL/Transform";
				xPathNavigator2.MoveTo(xPathNavigator);
				if (!xPathNavigator.MoveToFirstAttribute())
				{
					continue;
				}
				do
				{
					if ((!(xPathNavigator.LocalName == "extension-element-prefixes") && !(xPathNavigator.LocalName == "exclude-result-prefixes")) || !(xPathNavigator.NamespaceURI == ((!flag) ? "http://www.w3.org/1999/XSL/Transform" : string.Empty)))
					{
						continue;
					}
					string[] array = xPathNavigator.Value.Split(' ');
					foreach (string text in array)
					{
						string text2 = ((!(text == "#default")) ? text : string.Empty);
						if ((string)hashtable[text2] == xPathNavigator2.GetNamespace(text2))
						{
							hashtable.Remove(text2);
						}
					}
				}
				while (xPathNavigator.MoveToNextAttribute());
				xPathNavigator.MoveToParent();
			}
			while (xPathNavigator.MoveToParent());
			return hashtable;
		}

		public void CompileDecimalFormat()
		{
			XmlQualifiedName xmlQualifiedName = ParseQNameAttribute("name");
			try
			{
				if (xmlQualifiedName.Name != string.Empty)
				{
					XmlConvert.VerifyNCName(xmlQualifiedName.Name);
				}
			}
			catch (XmlException innerException)
			{
				throw new XsltCompileException("Invalid qualified name", innerException, Input);
			}
			XslDecimalFormat xslDecimalFormat = new XslDecimalFormat(this);
			if (decimalFormats.Contains(xmlQualifiedName))
			{
				((XslDecimalFormat)decimalFormats[xmlQualifiedName]).CheckSameAs(xslDecimalFormat);
			}
			else
			{
				decimalFormats[xmlQualifiedName] = xslDecimalFormat;
			}
		}

		public string LookupNamespace(string prefix)
		{
			if (prefix == string.Empty || prefix == null)
			{
				return string.Empty;
			}
			XPathNavigator xPathNavigator = Input;
			if (Input.NodeType == XPathNodeType.Attribute)
			{
				xPathNavigator = Input.Clone();
				xPathNavigator.MoveToParent();
			}
			return xPathNavigator.GetNamespace(prefix);
		}

		public void CompileOutput()
		{
			XPathNavigator input = Input;
			string attribute = input.GetAttribute("href", string.Empty);
			XslOutput xslOutput = outputs[attribute] as XslOutput;
			if (xslOutput == null)
			{
				xslOutput = new XslOutput(attribute, stylesheetVersion);
				outputs.Add(attribute, xslOutput);
			}
			xslOutput.Fill(input);
		}
	}
}
