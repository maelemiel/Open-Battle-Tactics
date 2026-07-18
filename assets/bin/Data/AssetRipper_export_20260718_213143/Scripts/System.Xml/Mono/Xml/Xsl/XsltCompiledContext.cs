using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XsltCompiledContext : XsltContext
	{
		private class XsltContextInfo
		{
			public bool IsCData;

			public bool PreserveWhitespace = true;

			public string ElementPrefix;

			public string ElementNamespace;

			public void Clear()
			{
				IsCData = false;
				PreserveWhitespace = true;
				ElementPrefix = (ElementNamespace = null);
			}
		}

		private Hashtable keyNameCache = new Hashtable();

		private Hashtable keyIndexTables = new Hashtable();

		private Hashtable patternNavCaches = new Hashtable();

		private XslTransformProcessor p;

		private XsltContextInfo[] scopes;

		private int scopeAt;

		public XslTransformProcessor Processor
		{
			get
			{
				return p;
			}
		}

		public override string DefaultNamespace
		{
			get
			{
				return string.Empty;
			}
		}

		public override bool Whitespace
		{
			get
			{
				return WhitespaceHandling;
			}
		}

		public bool IsCData
		{
			get
			{
				return scopes[scopeAt].IsCData;
			}
			set
			{
				scopes[scopeAt].IsCData = value;
			}
		}

		public bool WhitespaceHandling
		{
			get
			{
				return scopes[scopeAt].PreserveWhitespace;
			}
			set
			{
				scopes[scopeAt].PreserveWhitespace = value;
			}
		}

		public string ElementPrefix
		{
			get
			{
				return scopes[scopeAt].ElementPrefix;
			}
			set
			{
				scopes[scopeAt].ElementPrefix = value;
			}
		}

		public string ElementNamespace
		{
			get
			{
				return scopes[scopeAt].ElementNamespace;
			}
			set
			{
				scopes[scopeAt].ElementNamespace = value;
			}
		}

		public XsltCompiledContext(XslTransformProcessor p)
			: base(new NameTable())
		{
			this.p = p;
			scopes = new XsltContextInfo[10];
			for (int i = 0; i < 10; i++)
			{
				scopes[i] = new XsltContextInfo();
			}
		}

		public XPathNavigator GetNavCache(Pattern p, XPathNavigator node)
		{
			XPathNavigator xPathNavigator = patternNavCaches[p] as XPathNavigator;
			if (xPathNavigator == null || !xPathNavigator.MoveTo(node))
			{
				xPathNavigator = node.Clone();
				patternNavCaches[p] = xPathNavigator;
			}
			return xPathNavigator;
		}

		public object EvaluateKey(IStaticXsltContext staticContext, BaseIterator iter, Expression nameExpr, Expression valueExpr)
		{
			XmlQualifiedName keyName = GetKeyName(staticContext, iter, nameExpr);
			KeyIndexTable indexTable = GetIndexTable(keyName);
			return indexTable.Evaluate(iter, valueExpr);
		}

		public bool MatchesKey(XPathNavigator nav, IStaticXsltContext staticContext, string name, string value)
		{
			XmlQualifiedName name2 = XslNameUtil.FromString(name, staticContext);
			KeyIndexTable indexTable = GetIndexTable(name2);
			return indexTable.Matches(nav, value, this);
		}

		private XmlQualifiedName GetKeyName(IStaticXsltContext staticContext, BaseIterator iter, Expression nameExpr)
		{
			XmlQualifiedName xmlQualifiedName = null;
			if (nameExpr.HasStaticValue)
			{
				xmlQualifiedName = (XmlQualifiedName)keyNameCache[nameExpr];
				if (xmlQualifiedName == null)
				{
					xmlQualifiedName = XslNameUtil.FromString(nameExpr.EvaluateString(iter), staticContext);
					keyNameCache[nameExpr] = xmlQualifiedName;
				}
			}
			else
			{
				xmlQualifiedName = XslNameUtil.FromString(nameExpr.EvaluateString(iter), this);
			}
			return xmlQualifiedName;
		}

		private KeyIndexTable GetIndexTable(XmlQualifiedName name)
		{
			KeyIndexTable keyIndexTable = keyIndexTables[name] as KeyIndexTable;
			if (keyIndexTable == null)
			{
				keyIndexTable = new KeyIndexTable(this, p.CompiledStyle.ResolveKey(name));
				keyIndexTables[name] = keyIndexTable;
			}
			return keyIndexTable;
		}

		public override string LookupNamespace(string prefix)
		{
			throw new InvalidOperationException("we should never get here");
		}

		internal override IXsltContextFunction ResolveFunction(XmlQualifiedName name, XPathResultType[] argTypes)
		{
			string text = name.Namespace;
			if (text == null)
			{
				return null;
			}
			object obj = null;
			if (p.Arguments != null)
			{
				obj = p.Arguments.GetExtensionObject(text);
			}
			bool isScript = false;
			if (obj == null)
			{
				obj = p.ScriptManager.GetExtensionObject(text);
				if (obj == null)
				{
					return null;
				}
				isScript = true;
			}
			MethodInfo methodInfo = FindBestMethod(obj.GetType(), name.Name, argTypes, isScript);
			if (methodInfo != null)
			{
				return new XsltExtensionFunction(obj, methodInfo, p.CurrentNode);
			}
			return null;
		}

		private MethodInfo FindBestMethod(Type t, string name, XPathResultType[] argTypes, bool isScript)
		{
			MethodInfo[] methods = t.GetMethods((BindingFlags)(((!isScript) ? 16 : 48) | 4 | 8));
			if (methods.Length == 0)
			{
				return null;
			}
			if (argTypes == null)
			{
				return methods[0];
			}
			int num = 0;
			int num2 = argTypes.Length;
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].Name == name && methods[i].GetParameters().Length == num2)
				{
					methods[num++] = methods[i];
				}
			}
			int num3 = num;
			switch (num3)
			{
			case 0:
				return null;
			case 1:
				return methods[0];
			default:
			{
				num = 0;
				for (int j = 0; j < num3; j++)
				{
					bool flag = true;
					ParameterInfo[] parameters = methods[j].GetParameters();
					for (int k = 0; k < parameters.Length; k++)
					{
						XPathResultType xPathResultType = argTypes[k];
						if (xPathResultType != XPathResultType.Any)
						{
							XPathResultType xPathType = XPFuncImpl.GetXPathType(parameters[k].ParameterType, p.CurrentNode);
							if (xPathType != xPathResultType && xPathType != XPathResultType.Any)
							{
								flag = false;
								break;
							}
							if (xPathType == XPathResultType.Any && xPathResultType != XPathResultType.NodeSet && parameters[k].ParameterType != typeof(object))
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						return methods[j];
					}
				}
				return null;
			}
			}
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			throw new InvalidOperationException("shouldn't get here");
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
		{
			throw new InvalidOperationException("XsltCompiledContext exception: shouldn't get here.");
		}

		internal override IXsltContextVariable ResolveVariable(XmlQualifiedName q)
		{
			return p.CompiledStyle.ResolveVariable(q);
		}

		public override int CompareDocument(string baseUri, string nextBaseUri)
		{
			return baseUri.GetHashCode().CompareTo(nextBaseUri.GetHashCode());
		}

		public override bool PreserveWhitespace(XPathNavigator nav)
		{
			return p.CompiledStyle.Style.GetPreserveWhitespace(nav);
		}

		private void ExtendScope()
		{
			XsltContextInfo[] sourceArray = scopes;
			scopes = new XsltContextInfo[scopeAt * 2 + 1];
			if (scopeAt > 0)
			{
				Array.Copy(sourceArray, 0, scopes, 0, scopeAt);
			}
		}

		public override bool PopScope()
		{
			base.PopScope();
			if (scopeAt == -1)
			{
				return false;
			}
			scopeAt--;
			return true;
		}

		public override void PushScope()
		{
			base.PushScope();
			scopeAt++;
			if (scopeAt == scopes.Length)
			{
				ExtendScope();
			}
			if (scopes[scopeAt] == null)
			{
				scopes[scopeAt] = new XsltContextInfo();
			}
			else
			{
				scopes[scopeAt].Clear();
			}
		}
	}
}
