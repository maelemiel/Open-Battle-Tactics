using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class XslTransformProcessor
	{
		private XsltDebuggerWrapper debugger;

		private CompiledStylesheet compiledStyle;

		private XslStylesheet style;

		private Stack currentTemplateStack = new Stack();

		private XPathNavigator root;

		private XsltArgumentList args;

		private XmlResolver resolver;

		private string currentOutputUri;

		internal readonly XsltCompiledContext XPathContext;

		internal Hashtable globalVariableTable = new Hashtable();

		private Hashtable docCache;

		private Stack outputStack = new Stack();

		private StringBuilder avtSB;

		private Stack paramPassingCache = new Stack();

		private ArrayList nodesetStack = new ArrayList();

		private Stack variableStack = new Stack();

		private object[] currentStack;

		private Hashtable busyTable = new Hashtable();

		private static object busyObject = new object();

		public XsltDebuggerWrapper Debugger
		{
			get
			{
				return debugger;
			}
		}

		public CompiledStylesheet CompiledStyle
		{
			get
			{
				return compiledStyle;
			}
		}

		public XsltArgumentList Arguments
		{
			get
			{
				return args;
			}
		}

		public XPathNavigator Root
		{
			get
			{
				return root;
			}
		}

		public MSXslScriptManager ScriptManager
		{
			get
			{
				return compiledStyle.ScriptManager;
			}
		}

		public XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		public Outputter Out
		{
			get
			{
				return (Outputter)outputStack.Peek();
			}
		}

		public Hashtable Outputs
		{
			get
			{
				return compiledStyle.Outputs;
			}
		}

		public XslOutput Output
		{
			get
			{
				return Outputs[currentOutputUri] as XslOutput;
			}
		}

		public string CurrentOutputUri
		{
			get
			{
				return currentOutputUri;
			}
		}

		public bool InsideCDataElement
		{
			get
			{
				return XPathContext.IsCData;
			}
		}

		public XPathNodeIterator CurrentNodeset
		{
			get
			{
				return (XPathNodeIterator)nodesetStack[nodesetStack.Count - 1];
			}
		}

		public XPathNavigator CurrentNode
		{
			get
			{
				XPathNavigator current = CurrentNodeset.Current;
				if (current != null)
				{
					return current;
				}
				for (int num = nodesetStack.Count - 2; num >= 0; num--)
				{
					current = ((XPathNodeIterator)nodesetStack[num]).Current;
					if (current != null)
					{
						return current;
					}
				}
				return null;
			}
		}

		public int StackItemCount
		{
			get
			{
				if (currentStack == null)
				{
					return 0;
				}
				for (int i = 0; i < currentStack.Length; i++)
				{
					if (currentStack[i] == null)
					{
						return i;
					}
				}
				return currentStack.Length;
			}
		}

		public bool PreserveOutputWhitespace
		{
			get
			{
				return XPathContext.Whitespace;
			}
		}

		public XslTransformProcessor(CompiledStylesheet style, object debugger)
		{
			XPathContext = new XsltCompiledContext(this);
			compiledStyle = style;
			this.style = style.Style;
			if (debugger != null)
			{
				this.debugger = new XsltDebuggerWrapper(debugger);
			}
		}

		public void Process(XPathNavigator root, Outputter outputtter, XsltArgumentList args, XmlResolver resolver)
		{
			this.args = args;
			this.root = root;
			this.resolver = ((resolver == null) ? new XmlUrlResolver() : resolver);
			currentOutputUri = string.Empty;
			PushNodeset(new SelfIterator(root, XPathContext));
			CurrentNodeset.MoveNext();
			if (args != null)
			{
				foreach (XslGlobalVariable value in CompiledStyle.Variables.Values)
				{
					if (value is XslGlobalParam)
					{
						object param = args.GetParam(value.Name.Name, value.Name.Namespace);
						if (param != null)
						{
							((XslGlobalParam)value).Override(this, param);
						}
						value.Evaluate(this);
					}
				}
			}
			foreach (XslGlobalVariable value2 in CompiledStyle.Variables.Values)
			{
				if (args == null || !(value2 is XslGlobalParam))
				{
					value2.Evaluate(this);
				}
			}
			PopNodeset();
			PushOutput(outputtter);
			ApplyTemplates(new SelfIterator(root, XPathContext), XmlQualifiedName.Empty, null);
			PopOutput();
		}

		public XPathNavigator GetDocument(Uri uri)
		{
			XPathNavigator xPathNavigator;
			if (docCache != null)
			{
				xPathNavigator = docCache[uri] as XPathNavigator;
				if (xPathNavigator != null)
				{
					return xPathNavigator.Clone();
				}
			}
			else
			{
				docCache = new Hashtable();
			}
			XmlReader xmlReader = null;
			try
			{
				xmlReader = new XmlTextReader(uri.ToString(), (Stream)resolver.GetEntity(uri, null, null), root.NameTable);
				XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlReader);
				xmlValidatingReader.ValidationType = ValidationType.None;
				xPathNavigator = new XPathDocument(xmlValidatingReader, XmlSpace.Preserve).CreateNavigator();
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
			docCache[uri] = xPathNavigator.Clone();
			return xPathNavigator;
		}

		public void PushOutput(Outputter newOutput)
		{
			outputStack.Push(newOutput);
		}

		public Outputter PopOutput()
		{
			Outputter outputter = (Outputter)outputStack.Pop();
			outputter.Done();
			return outputter;
		}

		public StringBuilder GetAvtStringBuilder()
		{
			if (avtSB == null)
			{
				avtSB = new StringBuilder();
			}
			return avtSB;
		}

		public string ReleaseAvtStringBuilder()
		{
			string result = avtSB.ToString();
			avtSB.Length = 0;
			return result;
		}

		private Hashtable GetParams(ArrayList withParams)
		{
			if (withParams == null)
			{
				return null;
			}
			Hashtable hashtable;
			if (paramPassingCache.Count != 0)
			{
				hashtable = (Hashtable)paramPassingCache.Pop();
				hashtable.Clear();
			}
			else
			{
				hashtable = new Hashtable();
			}
			int count = withParams.Count;
			for (int i = 0; i < count; i++)
			{
				XslVariableInformation xslVariableInformation = (XslVariableInformation)withParams[i];
				hashtable.Add(xslVariableInformation.Name, xslVariableInformation.Evaluate(this));
			}
			return hashtable;
		}

		public void ApplyTemplates(XPathNodeIterator nodes, XmlQualifiedName mode, ArrayList withParams)
		{
			Hashtable hashtable = GetParams(withParams);
			while (NodesetMoveNext(nodes))
			{
				PushNodeset(nodes);
				XslTemplate xslTemplate = FindTemplate(CurrentNode, mode);
				currentTemplateStack.Push(xslTemplate);
				xslTemplate.Evaluate(this, hashtable);
				currentTemplateStack.Pop();
				PopNodeset();
			}
			if (hashtable != null)
			{
				paramPassingCache.Push(hashtable);
			}
		}

		public void CallTemplate(XmlQualifiedName name, ArrayList withParams)
		{
			Hashtable hashtable = GetParams(withParams);
			XslTemplate xslTemplate = FindTemplate(name);
			currentTemplateStack.Push(null);
			xslTemplate.Evaluate(this, hashtable);
			currentTemplateStack.Pop();
			if (hashtable != null)
			{
				paramPassingCache.Push(hashtable);
			}
		}

		public void ApplyImports()
		{
			XslTemplate xslTemplate = (XslTemplate)currentTemplateStack.Peek();
			if (xslTemplate == null)
			{
				throw new XsltException("Invalid context for apply-imports", null, CurrentNode);
			}
			XslTemplate xslTemplate2;
			for (int num = xslTemplate.Parent.Imports.Count - 1; num >= 0; num--)
			{
				XslStylesheet xslStylesheet = (XslStylesheet)xslTemplate.Parent.Imports[num];
				xslTemplate2 = xslStylesheet.Templates.FindMatch(CurrentNode, xslTemplate.Mode, this);
				if (xslTemplate2 != null)
				{
					currentTemplateStack.Push(xslTemplate2);
					xslTemplate2.Evaluate(this);
					currentTemplateStack.Pop();
					return;
				}
			}
			switch (CurrentNode.NodeType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Element:
				xslTemplate2 = ((!(xslTemplate.Mode == XmlQualifiedName.Empty)) ? new XslDefaultNodeTemplate(xslTemplate.Mode) : XslDefaultNodeTemplate.Instance);
				break;
			case XPathNodeType.Attribute:
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				xslTemplate2 = XslDefaultTextTemplate.Instance;
				break;
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				xslTemplate2 = XslEmptyTemplate.Instance;
				break;
			default:
				xslTemplate2 = XslEmptyTemplate.Instance;
				break;
			}
			currentTemplateStack.Push(xslTemplate2);
			xslTemplate2.Evaluate(this);
			currentTemplateStack.Pop();
		}

		internal void OutputLiteralNamespaceUriNodes(Hashtable nsDecls, ArrayList excludedPrefixes, string localPrefixInCopy)
		{
			if (nsDecls == null)
			{
				return;
			}
			foreach (DictionaryEntry nsDecl in nsDecls)
			{
				string text = (string)nsDecl.Key;
				string text2 = (string)nsDecl.Value;
				if (localPrefixInCopy == text || (localPrefixInCopy != null && text.Length == 0 && XPathContext.ElementNamespace.Length == 0))
				{
					continue;
				}
				bool flag = false;
				if (style.ExcludeResultPrefixes != null)
				{
					XmlQualifiedName[] excludeResultPrefixes = style.ExcludeResultPrefixes;
					foreach (XmlQualifiedName xmlQualifiedName in excludeResultPrefixes)
					{
						if (xmlQualifiedName.Namespace == text2)
						{
							flag = true;
						}
					}
				}
				if (flag || style.NamespaceAliases[text] != null)
				{
					continue;
				}
				switch (text2)
				{
				case "http://www.w3.org/XML/1998/namespace":
					if ("xml" == text)
					{
						continue;
					}
					break;
				case "http://www.w3.org/2000/xmlns/":
					if ("xmlns" == text)
					{
						continue;
					}
					break;
				case "http://www.w3.org/1999/XSL/Transform":
					continue;
				}
				if (excludedPrefixes == null || !excludedPrefixes.Contains(text))
				{
					Out.WriteNamespaceDecl(text, text2);
				}
			}
		}

		private XslTemplate FindTemplate(XPathNavigator node, XmlQualifiedName mode)
		{
			XslTemplate xslTemplate = style.Templates.FindMatch(CurrentNode, mode, this);
			if (xslTemplate != null)
			{
				return xslTemplate;
			}
			switch (node.NodeType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Element:
				if (mode == XmlQualifiedName.Empty)
				{
					return XslDefaultNodeTemplate.Instance;
				}
				return new XslDefaultNodeTemplate(mode);
			case XPathNodeType.Attribute:
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
				return XslDefaultTextTemplate.Instance;
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				return XslEmptyTemplate.Instance;
			default:
				return XslEmptyTemplate.Instance;
			}
		}

		private XslTemplate FindTemplate(XmlQualifiedName name)
		{
			XslTemplate xslTemplate = style.Templates.FindTemplate(name);
			if (xslTemplate != null)
			{
				return xslTemplate;
			}
			throw new XsltException("Could not resolve named template " + name, null, CurrentNode);
		}

		public void PushForEachContext()
		{
			currentTemplateStack.Push(null);
		}

		public void PopForEachContext()
		{
			currentTemplateStack.Pop();
		}

		public bool NodesetMoveNext()
		{
			return NodesetMoveNext(CurrentNodeset);
		}

		public bool NodesetMoveNext(XPathNodeIterator iter)
		{
			if (!iter.MoveNext())
			{
				return false;
			}
			if (iter.Current.NodeType == XPathNodeType.Whitespace && !XPathContext.PreserveWhitespace(iter.Current))
			{
				return NodesetMoveNext(iter);
			}
			return true;
		}

		public void PushNodeset(XPathNodeIterator itr)
		{
			BaseIterator baseIterator = itr as BaseIterator;
			baseIterator = ((baseIterator == null) ? new WrapperIterator(itr, null) : baseIterator);
			baseIterator.NamespaceManager = XPathContext;
			nodesetStack.Add(baseIterator);
		}

		public void PopNodeset()
		{
			nodesetStack.RemoveAt(nodesetStack.Count - 1);
		}

		public bool Matches(Pattern p, XPathNavigator n)
		{
			return p.Matches(n, XPathContext);
		}

		public object Evaluate(XPathExpression expr)
		{
			XPathNodeIterator currentNodeset = CurrentNodeset;
			BaseIterator baseIterator = (BaseIterator)currentNodeset;
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (baseIterator.NamespaceManager == null)
			{
				baseIterator.NamespaceManager = compiledExpression.NamespaceManager;
			}
			return compiledExpression.Evaluate(baseIterator);
		}

		public string EvaluateString(XPathExpression expr)
		{
			XPathNodeIterator currentNodeset = CurrentNodeset;
			return currentNodeset.Current.EvaluateString(expr, currentNodeset, XPathContext);
		}

		public bool EvaluateBoolean(XPathExpression expr)
		{
			XPathNodeIterator currentNodeset = CurrentNodeset;
			return currentNodeset.Current.EvaluateBoolean(expr, currentNodeset, XPathContext);
		}

		public double EvaluateNumber(XPathExpression expr)
		{
			XPathNodeIterator currentNodeset = CurrentNodeset;
			return currentNodeset.Current.EvaluateNumber(expr, currentNodeset, XPathContext);
		}

		public XPathNodeIterator Select(XPathExpression expr)
		{
			return CurrentNodeset.Current.Select(expr, XPathContext);
		}

		public XslAttributeSet ResolveAttributeSet(XmlQualifiedName name)
		{
			return CompiledStyle.ResolveAttributeSet(name);
		}

		public object GetStackItem(int slot)
		{
			return currentStack[slot];
		}

		public void SetStackItem(int slot, object o)
		{
			currentStack[slot] = o;
		}

		public void PushStack(int stackSize)
		{
			variableStack.Push(currentStack);
			currentStack = new object[stackSize];
		}

		public void PopStack()
		{
			currentStack = (object[])variableStack.Pop();
		}

		public void SetBusy(object o)
		{
			busyTable[o] = busyObject;
		}

		public void SetFree(object o)
		{
			busyTable.Remove(o);
		}

		public bool IsBusy(object o)
		{
			return busyTable[o] == busyObject;
		}

		public bool PushElementState(string prefix, string name, string ns, bool preserveWhitespace)
		{
			bool flag = IsCData(name, ns);
			XPathContext.PushScope();
			Outputter outputter = Out;
			bool flag2 = flag;
			XPathContext.IsCData = flag2;
			outputter.InsideCDataSection = flag2;
			XPathContext.WhitespaceHandling = true;
			XPathContext.ElementPrefix = prefix;
			XPathContext.ElementNamespace = ns;
			return flag;
		}

		private bool IsCData(string name, string ns)
		{
			for (int i = 0; i < Output.CDataSectionElements.Length; i++)
			{
				XmlQualifiedName xmlQualifiedName = Output.CDataSectionElements[i];
				if (xmlQualifiedName.Name == name && xmlQualifiedName.Namespace == ns)
				{
					return true;
				}
			}
			return false;
		}

		public void PopCDataState(bool isCData)
		{
			XPathContext.PopScope();
			Out.InsideCDataSection = XPathContext.IsCData;
		}
	}
}
